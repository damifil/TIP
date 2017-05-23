using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientAplication
{
    class Client
    {
        private TcpClient _client;

        private static StreamReader _sReader;
        private static StreamWriter _sWriter;

        private Boolean _isConnected;
        private DiffieHelman diffieHelman;
        Random rnd = new Random();
        internal string ipAddres;
        internal int portnumber;
        public Client(string ipAddress, int portNum)
        {
            _client = new TcpClient();
            _client.Connect(ipAddress, portNum);

            HandleCommunication();
            ipAddres = ipAddress;
            portnumber = portNum;
        }

        public void destroyfunction()
        {
            Console.WriteLine("tutaj1");
            _client.GetStream().Close();
            Console.WriteLine("tutaj2");

            _client.Close();
            Console.WriteLine("tutaj3");

            _client.Dispose();
        }


        public void HandleCommunication()
        {
            _sReader = new StreamReader(_client.GetStream(), Encoding.ASCII);
            _sWriter = new StreamWriter(_client.GetStream(), Encoding.ASCII);

            _isConnected = true;
            String sData = null;
            diffieHelman = new DiffieHelman();
            diffieHelman.CreateDH(_sReader, _sWriter, diffieHelman);
            Console.WriteLine("create");
            /*  while (_isConnected) {
                  Console.WriteLine("create");
                  //utworzenie polacenia szyfrowanego


              //komunikacja z serwerem szyfrowana
              Console.WriteLine("po create");
                  //wysylanie wiadomosci sData to string ktory ma zostac wyslany (komunikat SEND)
                  diffieHelman.sendMessage(sData, diffieHelman, _sWriter, "SEND");

                  // nasluchiwanie otrzymywania danych z serwera (po otrzymaniu paczki zwraca stringa
                  String sDataIncomming = diffieHelman.reciveMessage(_sReader, diffieHelman);
                  Console.WriteLine("Otrzymane dane z serwera: " + sDataIncomming);

                  //w przypadku gdy klient wysyla exit zamykamy polaczenie (wylaczamy watek
                  if (sData == "EXIT")
                  {
                      _isConnected = false;
                  }
              }*/
        }

        public string sendMessage(string content)
        {
           
            _isConnected = true;
            String sData = null;
            
            diffieHelman.sendMessage1(content, diffieHelman, _sWriter);
            String sDataIncomming = diffieHelman.reciveMessage(_sReader, diffieHelman);
            return sDataIncomming;
        }
    }
}
