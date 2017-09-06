using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClientAplication
{
    public sealed class ReadSettings
    {
        public string IP;
        public string PORT;
        private static ReadSettings instance = null;
        private static readonly object PadLock = new object();
        public static ReadSettings GetInstance
        {
            get
            {
                lock (PadLock)
                {
                    if (instance == null)
                    {
                        instance = new ReadSettings();
                    }
                    return instance;
                }
            }
        }

        public ReadSettings()
        {
            readSettings();
        }
        public void readSettings()
        {

            if (!File.Exists("settings"))
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter("settings"))
                {
                    file.WriteLine("192.168.8.102");
                    file.WriteLine("5555");
                    file.Close();
                    IP = "192.168.8.102";
                    PORT = "5555";
                }
            }
            else
            {
                System.IO.StreamReader file = new System.IO.StreamReader("settings");
                IP = file.ReadLine();
                PORT = file.ReadLine();
            }

        }

        public void saveSettings(string ipk, string portk)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("settings"))
            {
                file.WriteLine(ipk);
                file.WriteLine(portk);
                file.Close();
                IP = ipk;
                PORT = portk;  
                MessageBox.Show("Pomyślnie zmieniono adres IP oraz port serwera");
            }
        }
    }

}
