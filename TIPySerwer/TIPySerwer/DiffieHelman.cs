using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIPySerwer
{

    class DiffieHelman
    {
        private double G, P, B, b, A, s;
        Random rnd = new Random();
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



        public double[] generateDoubleArrayFromStrin(string message)
        {
            double[] returnmesage = new double[message.Length];
            for (int i = 0; i < message.Length; i++)
            {

                returnmesage[i] = (double)message[i];
            }
            return returnmesage;
        }




        public void createDH(DiffieHelman diffieHelman, StreamReader sReader, StreamWriter sWriter)
        {
            diffieHelman.P = Convert.ToDouble(sReader.ReadLine());
            Console.WriteLine(" p" + diffieHelman.P);
            diffieHelman.G = Convert.ToDouble(sReader.ReadLine());
            Console.WriteLine(" g" + diffieHelman.G);
            diffieHelman.A = Convert.ToDouble(sReader.ReadLine());
            Console.WriteLine(" a" + diffieHelman.A);

            diffieHelman.b = rnd.Next(10, 10000);

            diffieHelman.B = diffieHelman.powMod((int)diffieHelman.G, (int)diffieHelman.b, (int)diffieHelman.P);
            Console.WriteLine("g" + diffieHelman.G + " b" + diffieHelman.b + " p" + diffieHelman.P);
            string sData = diffieHelman.B.ToString();
            sWriter.WriteLine(sData);
            sWriter.Flush();

            //utworzenie sekretu
            diffieHelman.s = diffieHelman.powMod((int)diffieHelman.A, (int)diffieHelman.b, (int)diffieHelman.P);
        }
        public string messageRecive(StreamReader sReader, DiffieHelman diffieHelman)
        {
            double encryptValue = 0;
            string sData = sReader.ReadLine();
            string messageDecrypt = "";
            if (sData == "CREATE") { return sData; }
            for (int i = Int32.Parse(sData); i > 0; i--)
            {
                sData = sReader.ReadLine();
                encryptValue = Convert.ToDouble(sData);
                messageDecrypt = messageDecrypt + (char)(encryptValue * Math.Pow(diffieHelman.s, -1));
            }
            return messageDecrypt;
        }

        public void sendMessage(string sData, DiffieHelman diffieHelman, StreamWriter _sWriter)
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

    }


}
