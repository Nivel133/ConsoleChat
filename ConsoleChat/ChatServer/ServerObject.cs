using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    public class ServerObject
    {
        private static TcpListener tcpListener;
        private List<ClientObject> clients = new List<ClientObject>();

        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }

        protected internal void RemoveConnection(string id)
        {
            ClientObject client = clients.Find(c => c.Id == id);
            if (client != null)
                clients.Remove(client);
        }

        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8080);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    System.Threading.Thread clientThread = new System.Threading.Thread(new System.Threading.ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        protected internal void BroadcastMessage(string message, string senderId)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            foreach (ClientObject client in clients)
            {
                if (client.Id != senderId)
                {
                    client.Stream.Write(data, 0, data.Length);
                }
            }
        }

        protected internal void Disconnect()
        {
            if (tcpListener != null)
                tcpListener.Stop();

            foreach (ClientObject client in clients)
            {
                client.Close();
            }
            Environment.Exit(0);
        }
    }
}

