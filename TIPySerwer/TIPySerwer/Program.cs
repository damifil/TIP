using HashLib;
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



            /*
            UserManager.AddUser("Dawid", "haslo").Wait();
            UserManager.Logging("Damian", "haslo", "192.168.122.123").Wait();
            UserManager.SavaCall("Dawid", "Damian", DateTime.Now).Wait();
            List<CallsHistoryModel> listCalls = UserManager.GetCalls("Dawid");
            foreach(CallsHistoryModel item in listCalls)
            {
                Console.WriteLine("Rozmowa z użytkownikiem " + item.login + " rozpoczęła się " + item.dateBegin
                    + " i zakończyła się " + item.dateEnd);
            }*/

            /*
                        tipBDEntities db = new tipBDEntities();
                        Users user = new Users();
                        user.Login = "A1dam1";
                        user.Password = "adam";
                        user.IP_Address = "192.168.123.101";
                        user.DateLastActiv = DateTime.Now;
                        db.Users.Add(user);

                       Calls call = new Calls();
                        call.From_ID = 5;
                        call.To_ID = 6;
                        call.Date_Begin = DateTime.Now;
                        call.Date_End = DateTime.Now;
                        db.Calls.Add(call);

                        db.SaveChanges();
                        */
            /*var users = db.Users;
            foreach(Users u in users)
            {
                Console.WriteLine(u.Login + " p: " + u.Password + " IP: " + u.IP_Address + " date: " + u.DateLastActiv);
            }
            var calls = db.Calls;
            foreach (Calls c in calls)
            {
                Console.WriteLine(c.From_ID + " "  + c.To_ID);
            }*/
        }


    
    }
}
