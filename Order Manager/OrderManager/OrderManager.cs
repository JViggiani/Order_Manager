using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Order_Manager.Exchanges;
using Order_Manager.FIX;
using Order_Manager.Orders;


namespace Order_Manager.Order_Manager
{
    public sealed class OrderManager : IOrderManager  //subject
    {
        //Singleton implementation//
        private static readonly OrderManager instance = new OrderManager();

        public OrderManager()
        {
        }
        public static OrderManager Instance
        {
            get
            {
                return instance;
            }
        }
        //End of singleton implementation//

        

        private class StateObject
        {
            // Client  socket.  
            public Socket workSocket = null;
            // Size of receive buffer.  
            public const int BufferSize = 1024;
            // Receive buffer.  
            public byte[] buffer = new byte[BufferSize];
            // Received data string.  
            public StringBuilder sb = new StringBuilder();
        }

        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        //Port currently being searched to find new connections
        private int currentPort = 11000;

        //List of active sockets
        private List<StateObject> connectedUsers = new List<StateObject>();

        private string userId = "ORDERMANAGER";

        public void begin()
        {
            Thread newConnections = new Thread(new ThreadStart(this.startListening));
            newConnections.Name = "NEWCONNECTIONS";
            newConnections.Start();


        }

        private void startListening()
        {
            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {

                // Establish the local endpoint for the socket.  
                // The DNS name of the computer  
                // running the listener is "host.contoso.com".  
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, this.currentPort);

                // Create a TCP/IP socket.  
                Socket listener = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                listener.Bind(localEndPoint);
                listener.Listen(100);

                //JOSH gameplan: Have the listener listen for new ports. When it finds one, add the socket / stateobject to a list. Update the port number =+ 1
                // Have a separate thread scan the sockets on the list while true. If it has data available, then do socket.BeginAccept(...acceptcallback...) 
                // Change the method to just add the FIX msg to a queue. Then have another thread handling the queue

                //Problem: program stops listening on socket 11000. 
                    // We accept some message, read the data, then destroy the socket and recreated it in the while loop
                    // Even if we move the socket binding and listening outside, we get a new exception.. 
                    //

                while (true)
                {
                    Console.WriteLine(this.userId + ": Now listening on port " + this.currentPort);

                    // Set the event to nonsignaled state.  
                    allDone.Reset(); //JOSH in the new list/queue structure this could prevent pairing threads from running 

                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine(this.userId + ": Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);
                    Console.WriteLine(this.userId + ": New connection found");

                    //currentPort += 1;

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read   
                // more data.  
                content = state.sb.ToString();
                FixMessage fixMessageRecieved = new FixMessage(content);
                if (content.IndexOf("10=") > -1)
                {
                    // All the data has been read from the   
                    // client. Display it on the console.  
                    Console.WriteLine(this.userId + ": Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);

                    // Echo the message back to the client.  
                    Send(handler, content);
                }
                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        protected void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine(this.userId + ": Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);  //JOSH is this the problem?
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        private void listenForTraders()
        {
            throw new NotImplementedException();
        }

        private void listenForExchange()
        {
            throw new NotImplementedException();
        }

        public void finishTrading()
        {
            throw new NotImplementedException();
        }

        public void freezeTrading()
        {
            throw new NotImplementedException();
        }

        public void update(Exchange exchange)
        {
            throw new NotImplementedException();
        }

        private void sendToExchange(Order order)
        {

        }

    }
}
