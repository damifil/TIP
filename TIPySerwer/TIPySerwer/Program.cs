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
        private TcpListener server;
        private Boolean isRunning;
       
        public TcpServer(int port)
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            isRunning = true;

            LoopClients();
        }

        public void LoopClients()
        {
            while (isRunning)
            {
                
                TcpClient newClient = server.AcceptTcpClient();
                Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                try
                {
                    t.Start(newClient);
                }
                catch (Exception ex) { Console.WriteLine("klient nieprawidlowo zakonczylpolaczenie"); }
            }
        }

        public void HandleClient(object obj)
        {
            Random rnd = new Random();
            TcpClient client = (TcpClient)obj;
            DiffieHelman diffieHelman = new DiffieHelman();
            StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);
            StreamReader sReader = new StreamReader(client.GetStream(), Encoding.ASCII);
            Boolean bClientConnected = true;
            String sData = null;

            //interesujaca nas obecnie klasa tutaj cala logika wspolpracy serwera z klientem bedzie umiesczona
            while (bClientConnected)
            {
                //nasluchiwanie komunikatu
                sData = diffieHelman.messageRecive(sReader, diffieHelman);
                Console.WriteLine("otrzymano " + sData);
                string[] fragmentCommunication=null;
                try
                {
                    fragmentCommunication = sData.Split(' ');
                }catch(Exception e)
                {
                    Console.WriteLine("błędne dane");
                    fragmentCommunication = new string[1];
                    fragmentCommunication[0] = "ERROR";

                }
                
                switch (fragmentCommunication[0])
                {
                    case "CREATE":
                        diffieHelman.createDH( sReader, sWriter);
                        break;
                    case "SEND":
                        sData = diffieHelman.messageRecive(sReader, diffieHelman);
                        Console.WriteLine("Otrzymana wiadomosc: " + sData);
                        Console.WriteLine("Wiadomosc zwrotna: ");
                        sData = Console.ReadLine();

                        //funnkcja odpowiedzialna za wysylanie do klieenta chwilowo nie przewidzialem by klient wiadomosci odroznial podlug komunikatow
                        //tylko po prostu je odbiera
                        bClientConnected = diffieHelman.sendMessage(sData, diffieHelman, sWriter);
                        break;
                    case "EXIT":
                        bClientConnected = false;
                        Console.WriteLine("Klient "+ fragmentCommunication[1]+" został wylogowany");
                        UserManager.LogOff(fragmentCommunication[1]);
                        diffieHelman.sendMessage("Logut", diffieHelman, sWriter);
                        break;
                    case "LOGIN":       // logowanie użytkownika
                        sData = UserManager.Logging(fragmentCommunication[1], fragmentCommunication[2]).ToString();
                        bClientConnected = diffieHelman.sendMessage(sData, diffieHelman, sWriter);
                        break;
                    case "REGISTER":  // dodanie uzytkownika do bazy danych
                        sData = UserManager.AddUser(fragmentCommunication[1], fragmentCommunication[2]).ToString();
                        bClientConnected = diffieHelman.sendMessage(sData, diffieHelman, sWriter);
                        break;
                    case "CHPASS":    // zmiana hasla
                        sData = UserManager.ChangePassword(fragmentCommunication[1], fragmentCommunication[2]).ToString();
                        bClientConnected = diffieHelman.sendMessage(sData, diffieHelman, sWriter);
                        break;
                    case "DELACCOUNT": // usuniecie konta
                        sData = UserManager.DeleteUser(fragmentCommunication[1]).ToString();
                        bClientConnected = diffieHelman.sendMessage(sData, diffieHelman, sWriter);
                        break;
                    case "ADDFRIEND":  // dodanie znajomego
                        sData = UserManager.AddFriend(fragmentCommunication[1], fragmentCommunication[2]).ToString();
                        bClientConnected = diffieHelman.sendMessage(sData, diffieHelman, sWriter);
                        break;
                    case "DELFRIEND":  // usuniecie znajomego
                        sData = UserManager.DelFriend(fragmentCommunication[1], fragmentCommunication[2]).ToString();
                        bClientConnected = diffieHelman.sendMessage(sData, diffieHelman, sWriter);
                        break;
                    case "SRCH":       // szukanie uzytkowika
                        sData = UserManager.SearchUser(fragmentCommunication[1], fragmentCommunication[2]).ToString();
                        bClientConnected = diffieHelman.sendMessage(sData, diffieHelman, sWriter);
                        break;
                    case "ISONLINE":   // aktualizacja statniej aktywnosci 
                        UserManager.UpdateActivityUser(fragmentCommunication[1]).ToString();
                        sData = UserManager.GetFriends(fragmentCommunication[1]);
                        bClientConnected = diffieHelman.sendMessage(sData, diffieHelman, sWriter);
                        break;
                    case "ALLHISTORY":  // uzyskanie calej hisorii rozmow danego uzytkownika
                        sData = UserManager.GetAllHistory(fragmentCommunication[1]);
                        bClientConnected = diffieHelman.sendMessage(sData, diffieHelman, sWriter);
                        break;
                    case "USERHISTORY":  // uzyskanie hisorii rozmow z konkretnym uzytkownikiem
                        sData = UserManager.GetCallsConcreteUser(fragmentCommunication[1], fragmentCommunication[2]);
                        bClientConnected = diffieHelman.sendMessage(sData, diffieHelman, sWriter);
                        break;
                    case "GETFRIENDS":   // uzysaknie listy znajomych
                        sData = UserManager.GetFriends(fragmentCommunication[1]);
                        bClientConnected = diffieHelman.sendMessage(sData, diffieHelman, sWriter);
                        break;
                    case "SAVECALL":     // zapisanie rozmowy
                        sData = UserManager.SavaCall(fragmentCommunication[1], fragmentCommunication[2], fragmentCommunication[3], fragmentCommunication[4],fragmentCommunication[5], fragmentCommunication[6]).ToString();
                        bClientConnected = diffieHelman.sendMessage(sData, diffieHelman, sWriter);
                        break;
                    case "ERROR":
                        bClientConnected = false;
                        break;
                    case "LASTACTIVITY":  // uzyskanie ostatniej aktywnosci
                        sData= UserManager.UserLastActivity(fragmentCommunication[1]);
                        bClientConnected = diffieHelman.sendMessage(sData, diffieHelman, sWriter);
                        break;
                    default:
                        sData = "Nieprawidłowe żądanie";
                        bClientConnected = diffieHelman.sendMessage(sData, diffieHelman, sWriter);
                        break;

                }
                //ify dotyczace komunikatow
               /* if (sData == "CREATE")
                {
                    //utworzenie DH
                    diffieHelman.createDH(diffieHelman, sReader, sWriter);
                }*/
                //if(sData =="LOGIN")
                //if(sData== "REGISTER")
                //itd. w zaleznosci od potrzeb
              /*  if (sData == "send")
                {
                    sData = diffieHelman.messageRecive(sReader, diffieHelman);
                    Console.WriteLine("Otrzymana wiadomosc: " + sData);
                    Console.WriteLine("Wiadomosc zwrotna: ");
                    sData = Console.ReadLine();

                    //funnkcja odpowiedzialna za wysylanie do klieenta chwilowo nie przewidzialem by klient wiadomosci odroznial podlug komunikatow
                    //tylko po prostu je odbiera
                    diffieHelman.sendMessage(sData, diffieHelman, sWriter);
                }*/

               /* if (sData == "exit")
                {
                    bClientConnected = false;
                }*/
            }

        }
    }



    class Program
    {
        static void Main(string[] args)
        {
            //numer portu na ktorym bedzie nasluchiwac i uruchomienie serwera wielowatkowego
            //  TcpServer server = new TcpServer(5555);

            /*   var myPBX = new PBX(NetworkAddressHelper.GetLocalIP().ToString(), 20000, 20500);
               myPBX.Start();

               Console.ReadLine();
               myPBX.Stop();
            */

            Thread threadServer = new Thread(new ThreadStart(runServer));
            threadServer.Start();
            Thread threadOzeki = new Thread(new ThreadStart(runOzeki));
            threadOzeki.Start();
            //testDawid.drugiMejn(); // by nie zasmiecac tutaj swoimi testami :) 
        }
        public static void runServer()
        {
            TcpServer server = new TcpServer(5555);
        }

        public static void runOzeki()
        {
            var myPBX = new PBX(NetworkAddressHelper.GetLocalIP().ToString(), 20000, 20500);
            myPBX.Start();

            Console.ReadLine();
            myPBX.Stop();
        }
    }

    
}
