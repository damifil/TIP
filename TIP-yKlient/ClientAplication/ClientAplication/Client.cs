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
            diffieHelman = new DiffieHelman();
            diffieHelman.CreateDH(streamReader, streamWriter);
                        
            Console.WriteLine("create");
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
        }
        private object m_SyncObject = new object();
        public string sendMessage(string content)
        {
            lock (m_SyncObject)
            {
                isConnected = true;

                diffieHelman.sendMessage1(content, streamWriter);
                String sDataIncomming = diffieHelman.reciveMessage(streamReader);
                return sDataIncomming;
            }
        }
    }
}
