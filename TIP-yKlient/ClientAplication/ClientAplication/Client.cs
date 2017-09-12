using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientAplication
{
    /// <summary>
    /// Klasa przechowywująca informacje o kliencie oraz obiekty potrzebne do wykonywania połączeń 
    /// </summary>
    class Client
    {
        /// <summary>
        /// Obiekt klasy <c>TCPClient</c> wykorzystywana podczas połączenia z serwerem
        /// </summary>
        private TcpClient client;

        /// <summary>
        /// Obiekt klasy <c> StreamReader</c> wykorzystywany podczas odczytu streamu danych z serwera
        /// </summary>
        private static StreamReader streamReader;
        /// <summary>
        /// Obiekt klasy <c> StreamWriter</c> wykorzystywany podczas wysyłania stream danych do serwera
        /// </summary>
        private static StreamWriter streamWriter;

        /// <summary>
        /// ????????????
        /// </summary>
        private Boolean isConnected;


        /// <summary>
        /// Obiekt klasy <c>DiffieHelman</c> odpowiedzialny za szyfrowanie połączenia
        /// z serwerem
        /// </summary>
        private DiffieHelman diffieHelman;

        /// <summary>
        /// zmienna przechowująca adres ip serwera do któego klient się połączył
        /// </summary>
        internal string ipAddres { get; set; }
        /// <summary>
        /// zmienna prechowująca port serwera do którego klient się połączył
        /// </summary>
        internal int portnumber { get; set; }
        /// <summary>
        /// Konstruktor klasy podczas któego nawiazuje się połączenie 
        /// przy pomocy obiektu <c>TCPCLient</c>.
        /// Parametr <paramref name="ipAddress"/> musi być poprawnym adresem IPv4
        /// </summary>
        /// <value>
        /// jeżeli połaczenie nie udaje się rzucany jest wyjątek oraz obiekt zostaje zwolniony z pamieci
        /// </value>
        /// <param name="ipAddress"> adres ip serwera </param>
        /// <param name="portNum">port serwera</param>
        public Client(string ipAddress, int portNum)
        {
            client = new TcpClient();
            try
            {
                client.Connect(ipAddress, portNum);
            }
            catch (Exception e) { client.Dispose(); throw; }
            HandleCommunication();
            diffieHelman = new DiffieHelman();
            diffieHelman.CreateDH(streamReader, streamWriter);
                        
            ipAddres = ipAddress;
            portnumber = portNum;
        }

        /// <summary>
        /// funkcja zwalniająca wszystkie zasoby używane przez clienta
        /// </summary>
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

        /// <summary>
        /// zmienna używana do wymuszenia jednowątkowego działania
        /// </summary>
        private object m_SyncObject = new object();

        /// <summary>
        /// funkcja wysyłająca do serwera wiadomość
        /// </summary>
        /// <param name="content">wiadomość która ma zostać wysłana do serwera</param>
        /// <returns>zwraca komunikat otrzymany z serwera</returns>
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
