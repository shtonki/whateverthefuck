using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using whateverthefuck.src.util;

namespace whateverthefuckserver.network
{
    class WhateverServerConnection
    {
        TcpListener server = null;
        List<TcpClient> ActiveClients = new List<TcpClient>();

        public void StartListening()
        {
            Thread listenThread = new Thread(ListenThread);
            listenThread.Start();

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
                Logging.Log(String.Format("SocketException: {0}", e), Logging.LoggingLevel.Error);
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
