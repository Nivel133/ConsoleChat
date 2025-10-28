using System;
using System.Threading;

namespace ChatServer
{
    class Program
    {
        static ServerObject server;      // сервер
        static Thread listenThread;      // поток для прослушивания

        static void Main(string[] args)
        {
            try
            {
                server = new ServerObject();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start();   // запускаем поток для прослушивания подключений
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                server?.Disconnect();
            }
        }
    }
}

