using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
namespace CommandLine.DNC
{
    public class PeerMessage
    {
        public Guid MessageUUID { get; set; }
        public Guid PeerUUID { get; set; }
        public MessageType Type { get; set; }
        public byte[] MessageSignature { get; set; }
        public DateTime TimeStamp = DateTime.Now;
        public PeerMessage() { }
        public PeerMessage(MessageType type) {
            this.Type = type;
        }

        public bool CheckSignature(byte[] sentHash)
        {
            MessageSignature = null;
            var retVal = JsonConvert.SerializeObject(this);
            SHA256 sHA256 = SHA256.Create();
            byte[] bytes = sHA256.ComputeHash(Encoding.UTF8.GetBytes(retVal));
            MessageSignature = bytes;
            if (sentHash == bytes) return true;
            else return false;

        }
        public string ToJSON()
        {
            MessageSignature = null;
            var retVal = JsonConvert.SerializeObject(this);
            SHA256 sHA256 = SHA256.Create();
            byte[] bytes = sHA256.ComputeHash(Encoding.UTF8.GetBytes(retVal));
            MessageSignature = bytes;
            return JsonConvert.SerializeObject(this);
        }
        public static PeerMessage FromJSON(string text)
        {
            //ToDo: Every once in awhile the server sends messages too quickly and they are written on top of each other - concatenated rather than one at a time
            try
            {
                return JsonConvert.DeserializeObject<PeerMessage>(text);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
            return new PeerMessage(MessageType.BadMessage);
        }
    }

    public enum MessageType
    {
        Connect,
        Files,
        Peers,
        BadMessage,
        Pulse,
        Ok

    }
}