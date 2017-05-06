using HashLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TIPySerwer.Models;

namespace TIPySerwer
{
    class UserManager
    {
        public static bool IsLoginExists(string login)                          // sprawdzenie czy login istneje
        {
            using (tipBDEntities db = new tipBDEntities())
            {
                return db.Users.Where(u => u.Login.Equals(login)).Any();
            }
        }
        public async static Task<byte[]> HashPassword(string password)          // haszowanie hasla
        {
            IHash hash = HashFactory.Crypto.SHA3.CreateKeccak512();
            HashAlgorithm hashAlgo = HashFactory.Wrappers.HashToHashAlgorithm(hash);
            byte[] input = Encoding.UTF8.GetBytes(password);
            byte[] output = hashAlgo.ComputeHash(input);
            return output;
        }
        
        public async static Task<bool> AddUser(string login, string password)  // dodanie uzytkownika do bazy danych
        {
            if (IsLoginExists(login))
            {
                Console.WriteLine("Login jest zajety");  // tutaj wyslemy do aplikacji klienckiej wiadomosc
                return false;
            }
            using (tipBDEntities db = new tipBDEntities())
            {
                Users newUser = new Users();
                newUser.Login = login;
                newUser.Password = await HashPassword(password);
                db.Users.Add(newUser);
                db.SaveChanges();
                return true;
            }
        }


        public async static Task<bool> Logging(string login, string password, string IP)  // proces logowania
        {            
            if (!IsLoginExists(login))
            {
                Console.WriteLine("Błędny login lub hasło");  // tutaj wyslemy do aplikacji klienckiej wiadomosc
                return false;
            }
            using (tipBDEntities db = new tipBDEntities())
            {

                byte[] userPass = db.Users.Where(x => x.Login == "Damian").Select(x => x.Password).SingleOrDefault();
                byte[] pass = await HashPassword(password);
                if (!userPass.SequenceEqual(pass))
                {
                    Console.WriteLine("Błędny login lub hasło");  // tutaj wyslemy do aplikacji klienckiej wiadomosc
                    return false;
                }

                Console.WriteLine("Zostałeś zalogowany");
                return true;
            }

        }
        
    }
}
