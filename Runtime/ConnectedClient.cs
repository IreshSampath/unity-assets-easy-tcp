using System.Net.Sockets;

namespace GAG.EasyTCP
{
    public class ConnectedClient
    {
        public TcpClient Client;
        public int Id;              // Auto-incremented ID or assigned by client request
        public string Name;         // Optional: You can set this later after handshake

        public ConnectedClient(TcpClient client, int id, string name = null)
        {
            Client = client;
            Id = id;
            Name = name ?? $"Client_{id}";
        }
    }
}
