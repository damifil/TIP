using HashLib;
using Ozeki.Network;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TIPySerwer.Models;

namespace TIPySerwer
{


    class TcpServer
    {
        private TcpListener _server;
        private Boolean _isRunning;
        private DiffieHelman diffieHelman;
        public TcpServer(int port)
        {
            _server = new TcpListener(IPAddress.Any, port);
            _server.Start();

            _isRunning = true;

            LoopClients();
        }

        public void LoopClients()
        {
            while (_isRunning)
            {
                // wait for client connection
                TcpClient newClient = _server.AcceptTcpClient();

                // client found.
                // create a thread to handle communication
                Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                t.Start(newClient);
            }
        }

        public void HandleClient(object obj)
        {
            Random rnd = new Random();
            TcpClient client = (TcpClient)obj;
            diffieHelman = new DiffieHelman();
            StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);
            StreamReader sReader = new StreamReader(client.GetStream(), Encoding.ASCII);
            Boolean bClientConnected = true;
            String sData = null;

              //interesujaca nas obecnie klasa tutaj cala logika wspolpracy serwera z klientem bedzie umiesczona
            while (bClientConnected)
            {
                //nasluchiwanie komunikatu
                sData = sReader.ReadLine();

                //ify dotyczace komunikatow
                if (sData == "CREATE")
                {
                    //utworzenie DH
                    diffieHelman.createDH(diffieHelman, sReader, sWriter);
                }
                //if(sData =="LOGIN")
                //if(sData== "REGISTER")
                //itd. w zaleznosci od potrzeb
                if (sData == "send")
                {
                    sData = diffieHelman.messageRecive(sReader, diffieHelman);
                    Console.WriteLine("Otrzymana wiadomosc: " + sData);
                    Console.WriteLine("Wiadomosc zwrotna: ");
                    sData = Console.ReadLine();

                    //funnkcja odpowiedzialna za wysylanie do klieenta chwilowo nie przewidzialem by klient wiadomosci odroznial podlug komunikatow
                    //tylko po prostu je odbiera
                    diffieHelman.sendMessage(sData, diffieHelman, sWriter);
                }

                if (sData == "exit")
                {
                    bClientConnected = false;
                }
            }

        }
    }



    class Program
    {
        static void Main(string[] args)
        {
            //numer portu na ktorym bedzie nasluchiwac i uruchomienie serwera wielowatkowego
            //TcpServer server = new TcpServer(5555);

            /* var myPBX = new PBX(NetworkAddressHelper.GetLocalIP().ToString(), 20000, 20500);
             myPBX.Start();

             Console.ReadLine();
             myPBX.Stop();*/

            
            //testDawid.drugiMejn(); // by nie zasmiecac tutaj swoimi testami :) 
        }

        public static void listen(string communication)
        {
            string[] fragmentCommunication = communication.Split(' ');
            bool flag;
            string contentToSend;                // wiadomosc ktora wyslemy do uzytkownika
            switch (fragmentCommunication[0])
            {
                case "REGISTER":  // dodanie uzytkownika do bazy danych
                    flag = UserManager.AddUser(fragmentCommunication[1], fragmentCommunication[2]);
                    break;
                case "CHPASS":    // zmiana hasla
                    flag = UserManager.ChangePassword(fragmentCommunication[1], fragmentCommunication[2]);
                    break;
                case "ADDFRIEND":  // dodanie znajomego
                    flag = UserManager.AddFriend(fragmentCommunication[1], fragmentCommunication[2]);
                    break;
                case "DELFRIEND":  // usuniecie znajomego
                    break;
                case "SRCH":       // szukanie uzytkowika
                    contentToSend = UserManager.SearchUser(fragmentCommunication[1], fragmentCommunication[2]);
                    break;
                case "ISONLINE":   // aktualizacja statniej aktywnosci 
                    flag = UserManager.UpdateActivityUser(fragmentCommunication[1]);
                    break;
                case "GETHISTORY":  // uzyskanie hisorii rozmow z konkretnym uzytkownikiem
                    contentToSend = UserManager.GetCallsConcreteUser(fragmentCommunication[1], fragmentCommunication[2]);
                    break;
                default:
                    flag = false;
                    break;
                
            }
        }
    
    }
}
