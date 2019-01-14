using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CommandLine.DNC
{
    public class Communication
    {
        Random rnd = new Random();
        public List<PeerConnection> ConnectedPeers = new List<PeerConnection>();
        public bool RunServer = true;
        public int Port { get; set; }
        public void ListenForPeers()
        {
            if (Port == 0) Port = 3003;
            try
            {

                /* Initializes the Listener */
                //---listen at the specified IP and port no.---
                IPAddress localAdd = IPAddress.Parse("127.0.0.1");
                TcpListener listener = new TcpListener(localAdd, Port);
                Console.WriteLine($"Server Listening on port {Port}");

                /* Start Listeneting at the specified port */
                try
                {
                    listener.Start();
                }
                catch (Exception err)
                {
                    Console.WriteLine("Unable to start peer listener - " + err.Message);
                    return;
                }

                while (RunServer)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    var peerConnection = new PeerConnection();
                    client.ReceiveBufferSize = 204800;
                    client.SendBufferSize = 204800;
                    peerConnection.Client = client;
                    Console.WriteLine("Server Connection accepted." + client.Client.RemoteEndPoint);
                    ConnectedPeers.Add(peerConnection);
                    Thread thread = new Thread(() => {
                        peerConnection.ListenForPeerData();
                    });
                    thread.Start();
                }
                /* clean up */

                listener.Stop();

            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }
        public static void ConnectToPeer(Peer peer)
        {
            try
            {
                Console.WriteLine(peer);
                var peerConnection = new PeerConnection();
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
                peerConnection.Client = new TcpClient(peer.Host, peer.Port);

                Thread thread = new Thread(() => {
                    peerConnection.ListenForPeerData();
                });
                thread.Start();
                peerConnection.SendMessage(new PeerMessage() { Type = MessageType.Connect, MessageUUID = Guid.NewGuid() });
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

        }
        public void HandleNetworkData()
        {

        }
       
    }
    public class Peer
    {
        public Guid UUID { get; set; }
        public String Host { get; set; }
        public int Port { get; set; }
    }
}
