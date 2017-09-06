using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Security.Cryptography;
namespace ClientAplication
{

    public static class Cryptography
    {

        private static int iterations = 2;
        private static int keySize = 256;
        private static string hash = "SHA1";
        private static byte[] vectorBytes = Encoding.ASCII.GetBytes("syk3CiUaBs4KdmuZ");
        private static byte[] saltBytes = Encoding.ASCII.GetBytes("Itg4EdEeUy8v1c2J");

        public static string Encrypt(string value, byte[] password)
        {
            return Encrypt<AesManaged>(value, password);
        }
        public static string Encrypt<T>(string value, byte[] password)
                where T : SymmetricAlgorithm, new()
        {
            byte[] valueBytes = Encoding.UTF8.GetBytes(value);
            byte[] encrypted;
            using (T cipher = new T())
            {
                PasswordDeriveBytes passwordBytes = new PasswordDeriveBytes(password, saltBytes, hash, iterations);
                byte[] keyBytes = passwordBytes.GetBytes(keySize / 8);
                cipher.Clear();
                cipher.Padding = PaddingMode.Zeros;
                cipher.Mode = CipherMode.ECB;
                using (ICryptoTransform encryptor = cipher.CreateEncryptor(keyBytes, vectorBytes))
                {
                    using (MemoryStream to = new MemoryStream())
                    {
                        using (CryptoStream writer = new CryptoStream(to, encryptor, CryptoStreamMode.Write))
                        {
                            writer.Write(valueBytes, 0, valueBytes.Length);
                            writer.FlushFinalBlock();
                            encrypted = to.ToArray();
                        }
                    }
                }
                cipher.Clear();
            }
            return Convert.ToBase64String(encrypted);
        }
        public static string Decrypt(string value, byte[] password)
        {
            return Decrypt<AesManaged>(value, password);
        }
        public static string Decrypt<T>(string value, byte[] password) where T : SymmetricAlgorithm, new()
        {
            byte[] valueBytes = Convert.FromBase64String(value);
            byte[] decrypted;
            int decryptedByteCount = 0;
            using (T cipher = new T())
            {
                PasswordDeriveBytes passwordBytes = new PasswordDeriveBytes(password, saltBytes, hash, iterations);
                byte[] keyBytes = passwordBytes.GetBytes(keySize / 8);
                cipher.Clear();
                cipher.Padding = PaddingMode.Zeros;
                cipher.Mode = CipherMode.ECB;
                try
                {
                    using (ICryptoTransform decryptor = cipher.CreateDecryptor(keyBytes, vectorBytes))
                    {
                        using (MemoryStream from = new MemoryStream(valueBytes))
                        {
                            using (CryptoStream reader = new CryptoStream(from, decryptor, CryptoStreamMode.Read))
                            {
                                decrypted = new byte[valueBytes.Length];
                                decryptedByteCount = reader.Read(decrypted, 0, decrypted.Length);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return String.Empty;
                }
                cipher.Clear();
            }
            return Encoding.UTF8.GetString(decrypted, 0, decryptedByteCount);
        }

    }

    class DiffieHelman
    {
        private double G, a, A, s, P;
        byte[] secretByteArray = null;
        Random rnd = new Random();
        private double generateP()
        {
            int p = rnd.Next(200, 500);
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            path=path+"\\file\\10000.prm";
            string line = File.ReadLines(path).Skip(p).Take(1).First();
            p = Int32.Parse(line);
            return p;
        }
        private Boolean isPrimeNumber(long n)
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
        private int powMod(int a, int w, int n)
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
        private double generateG(double x)
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
        private double[] generateDoubleArrayFromStrin(string message)
        {
            double[] returnmesage = new double[message.Length];
            for (int i = 0; i < message.Length; i++)
            {
                returnmesage[i] = (double)message[i];
            }
            return returnmesage;
        }
        public void sendMessage(string sData, StreamWriter sWriter, string comunique)
        {
            try
            {
                sWriter.WriteLine(comunique);
                sWriter.Flush();
                String messageSend = Cryptography.Encrypt(sData, secretByteArray);
                sWriter.WriteLine(messageSend);
                sWriter.Flush();
            }
            catch (Exception ex) { return; }
        }
        public void sendMessage1(string sData, StreamWriter sWriter)
        {
            try
            {
                String messageSend = Cryptography.Encrypt(sData, secretByteArray);
            sWriter.WriteLine(messageSend);
            sWriter.Flush();
            }
            catch (Exception ex) { return; }
        }
        public string reciveMessage(StreamReader sReader)
        {
            try { 
            string sData = sReader.ReadLine();
            string messageDecrypt = "";
            messageDecrypt = Cryptography.Decrypt(sData, secretByteArray);
            return messageDecrypt.TrimEnd('\0');
            }
            catch (Exception ex) { return "error"; }
        }
        public void CreateDH(StreamReader _sReader, StreamWriter sWriter)
        {
            String sData;
            Boolean isCorrect = false;
         
            while (isCorrect != true)
            {
                sWriter.WriteLine("CREATE");
                sWriter.Flush();

                //utworzenie P i wysłanie
                P = generateP();
                sData = P.ToString();
                sWriter.WriteLine(sData);
                sWriter.Flush();

                //utworzenie G i wyslanie
                G = generateG(P);
                sData = G.ToString();
                sWriter.WriteLine(sData);
                sWriter.Flush();

                //wygenerowanie tajnego a
                a = rnd.Next(10, 1000);
                //wygenerowanie i wyslanie jawnego A
                A = powMod((int)G, (int)a, (int)P);
                sData = A.ToString();
                sWriter.WriteLine(sData);
                sWriter.Flush();

                //odebranie B
                String sDataIncomming = _sReader.ReadLine();
                double B = Convert.ToDouble(sDataIncomming);
                //utworzenie sekretu

                s = powMod((int)B, (int)a, (int)P);
                secretByteArray = BitConverter.GetBytes(s);
                sendMessage1("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", sWriter);
                if (_sReader.ReadLine() == "OK")
                {
                    Console.WriteLine("dostal okej");
                    isCorrect = true;
                }
            }
        }
    }
}

