using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace whateverthefuckserver
{
    class WhateverServerConnection
    {
        TcpListener server = null;
        List<TcpClient> ActiveClients = new List<TcpClient>();

        public void StartListening()
        {
            Thread listenThread = new Thread(ListenThread);
            listenThread.Start();
            Console.WriteLine("Listening to connections...");

            while (true)
            {
                foreach (var client in ActiveClients)
                {
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes("big nerd ");
                    NetworkStream stream = client.GetStream();
                    stream.Write(msg, 0, msg.Length);
                }

                Thread.Sleep(1000);
            }
        }

        private void AddClient(TcpClient client)
        {
            ActiveClients.Add(client);
            Console.WriteLine("we adding client now");
        }

        private void ListenThread()
        {
            try
            {
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Any;

                server = new TcpListener(localAddr, port);
                server.Start();

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();

                    AddClient(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
