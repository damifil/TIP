using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;




namespace TIPySerwer
{
    public static class Cryptography
    {
        private static int iterations = 2;
        private static int keySize = 256;
        private static string hash = "SHA1";
        private static byte[] vectorBytes= Encoding.ASCII.GetBytes("syk3CiUaBs4KdmuZ");
        private static byte[] saltBytes= Encoding.ASCII.GetBytes("Itg4EdEeUy8v1c2J");

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
                    //cipher.Clear();
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
        private double G, P, B, b, A, s;
        byte[] secretByteArray=null;
        Random rnd = new Random();

        public Boolean isPrimeNumber(long n)
        {
            long p = 1;

            if (n == 1)
            { return false; }

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
        public void createDH( StreamReader sReader, StreamWriter sWriter)
        {
            try
            {
                P = Convert.ToDouble(sReader.ReadLine());
                G = Convert.ToDouble(sReader.ReadLine());
                A = Convert.ToDouble(sReader.ReadLine());
                b = rnd.Next(10, 10000);
                B = powMod((int)G, (int)b, (int)P);
                string sData = B.ToString();
                sWriter.WriteLine(sData);
                sWriter.Flush();
                //utworzenie sekretu
                s = powMod((int)A, (int)b, (int)P);
                secretByteArray = BitConverter.GetBytes(s);
                string reciveMessage = messageRecive(sReader, this);
                if (reciveMessage == "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")
                {
                    sWriter.WriteLine("OK");
                    sWriter.Flush();
                }
                else
                {
                    sWriter.WriteLine("NOK");
                    sWriter.Flush();
                }
            }
            catch (Exception e) { }
            
        }
        public string messageRecive(StreamReader sReader, DiffieHelman diffieHelman)
        {
            try
            {
                string sData = sReader.ReadLine();
                if (sData == "CREATE") { return sData; }
                if (sData == null) { return null; }
                string messageDecrypt = Cryptography.Decrypt(sData, secretByteArray);
                return messageDecrypt.TrimEnd('\0');
            }
            catch (Exception e) { return "ERROR"; }
        }
        public bool sendMessage(string sData, DiffieHelman diffieHelman, StreamWriter sWriter)
        {
            try
            {
                String messageSend = Cryptography.Encrypt(sData, secretByteArray);
                sWriter.WriteLine(messageSend);
                sWriter.Flush();
                return true;
            }

            catch (Exception e) { return false; }
        }
    }

}
