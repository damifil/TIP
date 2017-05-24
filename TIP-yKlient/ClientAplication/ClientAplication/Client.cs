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
        private TcpClient client;

        private static StreamReader streamReader;
        private static StreamWriter streamWriter;

        private Boolean isConnected;
        private DiffieHelman diffieHelman;
        Random rnd = new Random();
        internal string ipAddres;
        internal int portnumber;
        public Client(string ipAddress, int portNum)
        {
            client = new TcpClient();
            client.Connect(ipAddress, portNum);

            HandleCommunication();
            ipAddres = ipAddress;
            portnumber = portNum;
        }

        public void destroyfunction()
        {
            client.GetStream().Close();
            client.Close();
            client.Dispose();
        }


        public void HandleCommunication()
        {
            streamReader = new StreamReader(client.GetStream(), Encoding.ASCII);
            streamWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);

            isConnected = true;
            diffieHelman = new DiffieHelman();
            diffieHelman.CreateDH(streamReader, streamWriter, diffieHelman);
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
           
            isConnected = true;
            
            diffieHelman.sendMessage1(content, diffieHelman, streamWriter);
            String sDataIncomming = diffieHelman.reciveMessage(streamReader, diffieHelman);
            return sDataIncomming;
        }
    }
}
