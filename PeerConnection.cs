using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
namespace CommandLine.DNC
{
    /// <summary>
    /// This pertains to the outgoing connections TO a peer.
    /// </summary>
    public class PeerConnection
    {
        public Peer Peer { get; set; }
        public TcpClient Client { get; set; }
        public bool DisconnectClient(){
            Client.Close();
            Client.Dispose();
            return true;
        }

        public void ListenForPeerData(){

            while (Client.Connected){

                if (Client.Available > 0)
                {
                    var stream = Client.GetStream();
                    NetworkStream nwStream = Client.GetStream();
                    byte[] bytesToRead = new byte[Client.ReceiveBufferSize];
                    int bytesRead = nwStream.Read(bytesToRead, 0, Client.ReceiveBufferSize);
                    var messagetext = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                    string[] filters = { "||" };
                    var messages = messagetext.Split(filters, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var message in messages)
                    {
                        PeerMessage returnedMessage = PeerMessage.FromJSON(message);
                        HandlePeerMessage(returnedMessage);
                    }


                }
            }
            // Close everything.
            try {
                DisconnectClient();
            }
            catch (Exception err){
                Console.WriteLine(err);
            }

        }
        public void SendMessage(PeerMessage message)
        {
            if (Client.Connected)
            {
                NetworkStream nwStream = Client.GetStream();
                byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes("||" + message.ToJSON());
                //---send the text---
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                nwStream.Flush();
            }
        }
        public void HandlePeerMessage(PeerMessage message){
            Console.WriteLine($"Message Recieved: {message.ToJSON()}");
        }

    }
}
