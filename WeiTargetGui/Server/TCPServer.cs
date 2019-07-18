using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WeiTargetGui.Server
{
    class TCPServer
    {
        //Static configuration - local communication only
        public const string ip = "localhost";
        public const int port = 8080;
        TcpListener listener;
        Thread serverThread;
        ServerRequestHandler requestHandler;
        public TCPServer()
        {
            requestHandler = new ServerRequestHandler();
            serverThread = new Thread(new ThreadStart(listen));
            serverThread.Start();
        }

        public void listen()
        {
            try
            {
                //TCPListener that will be listening to the wanted socket 
                //listener = new TcpListener(IPAddress.Parse(ip), port);
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                //Logger.LogHelper.Log(Logger.Logger.LogTarget.File, "Server Started" + "\n");
                while (true)
                {

                    //Block program until a client will connect 
                    //Logger.LogHelper.Log(Logger.Logger.LogTarget.File, "Waiting for incoming client connections .... " + "\n");
                    TcpClient client = listener.AcceptTcpClient();

                    //Logger.LogHelper.Log(Logger.Logger.LogTarget.File, "Accepted the connection .... " + "\n");
                    Console.Out.WriteLine("Client CONNECTED");

                    //Create a new thread for the new client so the server could continue listening for incoming connections
                    Thread clientThread = new Thread(ProcessClientRequests);
                    clientThread.Start(client);

                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
            finally
            {
                if (listener != null)
                {
                    listener.Stop();
                }
            }
        }


        private void ProcessClientRequests(object argument)
        {
            TcpClient client = (TcpClient)argument;
            try
            {
                /*get the incoming data through a network stream*/
                NetworkStream nwStream = client.GetStream();
                byte[] buffer = new byte[client.ReceiveBufferSize];

                /*read incoming stream*/
                int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

                /*convert the data received into a string*/
                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                //Logger.LogHelper.Log(Logger.Logger.LogTarget.File, "Received : " + dataReceived + "\n");
                Console.Out.WriteLine("CLIENT MESSAGE: " + dataReceived + "\n");

                string clientAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                requestHandler.HandleClientRequest(dataReceived);

                nwStream.Write(buffer, 0, bytesRead);
                client.Close();
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
        }







    }
}
