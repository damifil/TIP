using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClientAplication
{
    class DiffieHelman
    {
        public double G, a, A, s, P;
        Random rnd = new Random();
        public double generateP()
        {

            int p = rnd.Next(3, 20);
            string line = File.ReadLines("primary_value.txt").Skip(p).Take(1).First();
            p = Int32.Parse(line);
            return p;
        }
        public Boolean isPrimeNumber(long n)
        {
            long p = 1;

            if (n == 1)
                return false;

            for (int i = 1; i < n; i++)
            {
                if (n % i == 0)
                {
                    p++;
                    if (p > 2)
                        return false;
                }
            }
            return true;
        }

        public int powMod(int a, int w, int n)
        {
            int pot, wyn;
            int q;

            pot = a; wyn = 1;
            for (q = w; q > 0; q /= 2)
            {
                if (q % 2 != 0) wyn = (wyn * pot) % n;
                pot = (pot * pot) % n; // kolejna potęga
            }
            return wyn;
        }

        public int generateG(double x, int size)
        {

            int p = (int)x;
            int chek = 0;
            int q = 1;
            do
            {
                q = rnd.Next(3,1000);
                chek = 0;
                for (int i = 1; i <= (p - 1); i++)
                {
                    for (int j = 1; j <= (p - 1); j++)
                    {
                        if ((i % p) == powMod(q, j, p)) { chek++; j = p; }
                    }

                }
            } while (chek != (p - 1));

            return q;
        }

        public double[] generateDoubleArrayFromStrin(string message)
        {
            double[] returnmesage = new double[message.Length];
            for (int i = 0; i < message.Length; i++)
            {

                returnmesage[i] = (double)message[i];
            }

            return returnmesage;
        }



        public void sendMessage(string sData, DiffieHelman diffieHelman, StreamWriter _sWriter, string comunique)
        {
            _sWriter.WriteLine(comunique);
            _sWriter.Flush();

            double[] messagearray = diffieHelman.generateDoubleArrayFromStrin(sData);

            //ilosc przesylanych liczb (znakow)
            sData = messagearray.Length.ToString();
            _sWriter.WriteLine(sData);
            _sWriter.Flush();

            for (int i = 0; i < messagearray.Length; i++)
            {
                messagearray[i] = messagearray[i] * diffieHelman.s;
                _sWriter.WriteLine(messagearray[i].ToString());
                _sWriter.Flush();
            }
        }


        public void sendMessage1(string sData, DiffieHelman diffieHelman, StreamWriter _sWriter)
        {
            double[] messagearray = diffieHelman.generateDoubleArrayFromStrin(sData);

            //ilosc przesylanych liczb (znakow)
            sData = messagearray.Length.ToString();
            
            _sWriter.WriteLine(sData);
            _sWriter.Flush();

            for (int i = 0; i < messagearray.Length; i++)
            {
                messagearray[i] = messagearray[i] * diffieHelman.s;
                _sWriter.WriteLine(messagearray[i].ToString());
                _sWriter.Flush();
            }
        }

        public string reciveMessage(StreamReader sReader, DiffieHelman diffieHelman)
        {
            string sData = sReader.ReadLine();
            string messageDecrypt = "";
            double encryptValue;

            for (int i = Int32.Parse(sData); i > 0; i--)
            {
                sData = sReader.ReadLine();
                encryptValue = Convert.ToDouble(sData);
                messageDecrypt = messageDecrypt + (char)(encryptValue * Math.Pow(diffieHelman.s, -1));
            }
            return messageDecrypt;
        }

        public void CreateDH(StreamReader _sReader, StreamWriter _sWriter, DiffieHelman diffieHelman)
        {
            String sData;
            //tutaj ustalamy difiego 
            //komunikat o tworzeniu szyfrowania
            _sWriter.WriteLine("CREATE");
            _sWriter.Flush();

            //utworzenie P i wysłanie
            diffieHelman.P = diffieHelman.generateP();
            sData = diffieHelman.P.ToString();
            _sWriter.WriteLine(sData);

            _sWriter.Flush();

            //utworzenie G i wyslanie
            diffieHelman.G = diffieHelman.generateG(diffieHelman.P, 100);
 
            sData = diffieHelman.G.ToString();
            _sWriter.WriteLine(sData);
            _sWriter.Flush();

            //wygenerowanie tajnego a
            diffieHelman.a = rnd.Next(10, 1000);
            //wygenerowanie i wyslanie jawnego A
            diffieHelman.A = diffieHelman.powMod((int)diffieHelman.G, (int)diffieHelman.a, (int)diffieHelman.P);
            sData = diffieHelman.A.ToString();
            _sWriter.WriteLine(sData);
            _sWriter.Flush();

            //odebranie B
            String sDataIncomming = _sReader.ReadLine();
            double B = Convert.ToDouble(sDataIncomming);
            //utworzenie sekretu

            diffieHelman.s = diffieHelman.powMod((int)B, (int)diffieHelman.a, (int)diffieHelman.P);
        }
    }
}

