using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;

namespace ChatClient
{
    class Program
    {
        static string userName;
        private const string host = "127.0.0.1";  // адрес сервера (localhost)
        private const int port = 8080;           // порт сервера
        static TcpClient client;
        static NetworkStream stream;

        static void Main(string[] args)
        {
            Console.Write("Введите свое имя: ");
            userName = Console.ReadLine();
            client = new TcpClient();
            try
            {
                // Устанавливаем подключение
                client.Connect(host, port);
                stream = client.GetStream();
                Console.WriteLine($"Добро пожаловать, {userName}");

                // Отправляем имя на сервер
                string message = userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

                // Запускаем поток для получения сообщений от сервера
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start();
                Console.WriteLine("Теперь вы можете отправлять сообщения. Введите сообщение и нажмите Enter:");
                // Входим в цикл отправки сообщений
                SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка подключения: " + ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        // Отправка сообщений на сервер
        static void SendMessage()
        {
            try
            {
                while (true)
                {
                    string message = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(message))
                        continue;
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при отправке: " + ex.Message);
            }
        }

        // Получение сообщений от сервера
        static void ReceiveMessage()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[64];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    Console.WriteLine(message);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Соединение прервано! Нажмите Enter для выхода.");
                Console.ReadLine();
                Disconnect();
            }
        }

        // Отключение от сервера
        static void Disconnect()
        {
            if (stream != null) stream.Close();
            if (client != null) client.Close();
            Environment.Exit(0);
        }
    }
}

