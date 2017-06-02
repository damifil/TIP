using System;
using System.Collections;
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

            int p = rnd.Next(200, 500);
            string line = File.ReadLines("10000.prm").Skip(p).Take(1).First();
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

        public double generateG(double x)
        {
            double px = x;
            double k;
            double o = 1;
            ArrayList al = new ArrayList();
            for (double r = 2; r < px; r++)
            {
                k = Math.Pow(r, o);
                k = k % px;
                while (k > 1)
                {
                    o++;
                    k *= r;
                    k %= px;
                }
                if (o == (px - 1)) { al.Add(r); }
                o = 1;
            }


            int index = rnd.Next(1, al.Count);
            return Convert.ToDouble(al[index]);

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



        public void sendMessage(string sData,  StreamWriter _sWriter, string comunique)
        {
            _sWriter.WriteLine(comunique);
            _sWriter.Flush();

            double[] messagearray = generateDoubleArrayFromStrin(sData);

            //ilosc przesylanych liczb (znakow)
            sData = messagearray.Length.ToString();
            _sWriter.WriteLine(sData);
            _sWriter.Flush();

            for (int i = 0; i < messagearray.Length; i++)
            {
                messagearray[i] = messagearray[i] *s;
                _sWriter.WriteLine(messagearray[i].ToString());
                _sWriter.Flush();
            }
        }


        public void sendMessage1(string sData, StreamWriter _sWriter)
        {
            double[] messagearray = generateDoubleArrayFromStrin(sData);

            //ilosc przesylanych liczb (znakow)
            sData = messagearray.Length.ToString();
            
            _sWriter.WriteLine(sData);
            _sWriter.Flush();

            for (int i = 0; i < messagearray.Length; i++)
            {
                messagearray[i] = messagearray[i] * s;
                _sWriter.WriteLine(messagearray[i].ToString());
                _sWriter.Flush();
            }
        }

        public string reciveMessage(StreamReader sReader)
        {
            string sData = sReader.ReadLine();
            string messageDecrypt = "";
            double encryptValue;

            for (int i = Int32.Parse(sData); i > 0; i--)
            {
                sData = sReader.ReadLine();
                encryptValue = Convert.ToDouble(sData);
                messageDecrypt = messageDecrypt + (char)(encryptValue * Math.Pow(s, -1));
            }
            return messageDecrypt;
        }

        public void CreateDH(StreamReader _sReader, StreamWriter _sWriter)
        {
            String sData;
            //tutaj ustalamy difiego 
            //komunikat o tworzeniu szyfrowania
            _sWriter.WriteLine("CREATE");
            _sWriter.Flush();

            //utworzenie P i wysłanie
            P = generateP();
            sData = P.ToString();
            _sWriter.WriteLine(sData);

            _sWriter.Flush();

            //utworzenie G i wyslanie
            G = generateG(P);
 
            sData = G.ToString();
            _sWriter.WriteLine(sData);
            _sWriter.Flush();

            //wygenerowanie tajnego a
            a = rnd.Next(10, 1000);
            //wygenerowanie i wyslanie jawnego A
            A =powMod((int)G, (int)a, (int)P);
            sData = A.ToString();
            _sWriter.WriteLine(sData);
            _sWriter.Flush();

            //odebranie B
            String sDataIncomming = _sReader.ReadLine();
            double B = Convert.ToDouble(sDataIncomming);
            //utworzenie sekretu

            s = powMod((int)B, (int)a, (int)P);
        }
    }
}

