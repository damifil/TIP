using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    


class Client
    {
        private TcpClient _client;

        private StreamReader _sReader;
        private StreamWriter _sWriter;

        private Boolean _isConnected;
        private DiffieHelman diffieHelman;
        Random rnd = new Random();

        public Client(String ipAddress, int portNum)
        {
            _client = new TcpClient();
            _client.Connect(ipAddress, portNum);

            HandleCommunication();
            
            _client.GetStream().Close();
            _client.Close();
            _client.Dispose();
        }



        public void HandleCommunication()
        {
            _sReader = new StreamReader(_client.GetStream(), Encoding.ASCII);
            _sWriter = new StreamWriter(_client.GetStream(), Encoding.ASCII);

            _isConnected = true;
            String sData = null;
            diffieHelman = new DiffieHelman();


            //utworzenie polacenia szyfrowanego
            diffieHelman.CreateDH(_sReader, _sWriter, diffieHelman);

            //komunikacja z serwerem szyfrowana
            while (_isConnected)
            {
                //pomocnicze
                Console.Write("wartosc do wyslania: ");
                sData = Console.ReadLine();
                //wysylanie wiadomosci sData to string ktory ma zostac wyslany (komunikat SEND)
                diffieHelman.sendMessage(sData, diffieHelman, _sWriter,"send");
                
                // nasluchiwanie otrzymywania danych z serwera (po otrzymaniu paczki zwraca stringa
                String sDataIncomming = diffieHelman.reciveMessage(_sReader, diffieHelman);
                Console.WriteLine("Otrzymane dane z serwera: "+sDataIncomming);

                //w przypadku gdy klient wysyla exit zamykamy polaczenie (wylaczamy watek
                if(sData=="exit")
                {
                    _isConnected = false;
                }
            }
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Multi-Threaded TCP Server Demo");
            Console.WriteLine("Provide IP:");
            String ip = Console.ReadLine();

            Console.WriteLine("Provide Port:");
            int port = Int32.Parse(Console.ReadLine());

            Client client = new Client(ip, port);

        }
    }
}
