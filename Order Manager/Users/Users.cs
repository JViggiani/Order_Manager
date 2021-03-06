﻿using Order_Manager.FIX;
using Order_Manager.Orders;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Order_Manager.Users
{
    public abstract class User : IUsers
    {
        protected string userId { get; set; }
        protected int msgSeqNum { get; set; }

        protected Queue<Fix> orderQueue;

        public void begin()
        {
            Thread socketHandler = new Thread(new ThreadStart(StartClient));
            socketHandler.Name = this.userId + "_thread";
            socketHandler.Start();

            /*
            OrderFactory orderFactory = new OrderFactory();
            Order testorder = orderFactory.getOrder(new FixMessage("a"));
            */
        }

        // State object for receiving data from remote device.  
        private class StateObject
        {
            // Client socket.  
            public Socket workSocket = null;
            // Size of receive buffer.  
            public const int BufferSize = 256;
            // Receive buffer.  
            public byte[] buffer = new byte[BufferSize];
            // Received data string.  
            public StringBuilder sb = new StringBuilder();
        }

        // The port number for the remote device.  
        private const int port = 11000;

        //The socket to connect to the order manager.
        private Socket orderSocket;

        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        // The response from the remote device.  
        private static String response = String.Empty;

        protected void StartClient()    //JOSH should this be static?
        {
            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // The name of the   
                // remote device is "Josh-PC".  
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                orderSocket = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                orderSocket.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), 
                    orderSocket);
                connectDone.WaitOne();

                //Send a logon FIX message and handle the heartbeat signals
                logonAndBeginHeartbeat(5000);

                // Release the socket.  
                orderSocket.Shutdown(SocketShutdown.Both);
                orderSocket.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                Console.WriteLine(this.userId + ": Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        protected static void Receive(Socket client)
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);     //JOSH exception thrown
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        //JOSH note there is something wrong with this. It has two threads going throug it at once..
        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.  
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        protected void Send(Socket client, Fix fixMsg)
        {
            fixMsg.wrapMessageHeaderTrailer(this.userId, client.AddressFamily.ToString(), this.msgSeqNum++, DateTime.Now.ToString());
            
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(fixMsg.getFixString());

            // Begin sending the data to the remote device.  
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Console.WriteLine(this.userId + ": Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        
        private void startHeartbeat(int interval)
        {
            try
            {
                FixMessage heartbeat = new FixMessage("");
                while (orderSocket.Connected)
                {
                    System.Threading.Thread.Sleep(interval);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        
        private void logonAndBeginHeartbeat(int heartbeatInterval)
        {
            // Send logon data to the remote device.  
            FixLogon logonMsg = new FixLogon("5000");
            Send(orderSocket, logonMsg);
            sendDone.WaitOne();

            // Receive the response from the remote device.  
            Receive(orderSocket);
            receiveDone.WaitOne();

            // Write the response to the console.  
            Console.WriteLine(this.userId + ": Response received : {0}", response);

            //while true, send heartbeat signals and listen for a response. if response, send another
            //after 3 failures, disconnect
            FixHeartbeat heartbeat = new FixHeartbeat();

            while(this.orderSocket.Connected) //JOSH todo change this to "while we have recieved a response in the last x seconds - 
            {
                System.Threading.Thread.Sleep(5000);

                Send(orderSocket, heartbeat);

                Receive(orderSocket);
                receiveDone.WaitOne();

                // Write the response to the console.  
                Console.WriteLine(this.userId + ": Response received : {0}", response);
            }
        }
    }
}
