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
                await db.SaveChangesAsync();
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

                byte[] userPass = db.Users.Where(x => x.Login == login).Select(x => x.Password).SingleOrDefault();
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

        public async static Task<bool> SavaCall(string login, string login1, DateTime dateBegin)   // zapisanie rozmowy 
        {
            using (tipBDEntities db = new tipBDEntities())
            {
                Users user = db.Users.Where(x => x.Login == login).Single();
                Users user1 = db.Users.Where(x => x.Login == login1).Single();

                Calls call = new Calls();
                call.From_ID = user.ID;
                call.To_ID = user1.ID;
                call.Date_Begin = dateBegin;
                call.Date_End = DateTime.Now;
                db.Calls.Add(call);
                await db.SaveChangesAsync();
            }
            return true;
        }

        public static List<CallsHistoryModel> GetCalls(string login)   // pobranie rozmow danego uzytkownika
        {
            string calls = "";
            List<CallsHistoryModel> callsHistory = new List<CallsHistoryModel>();
            using (tipBDEntities db = new tipBDEntities())
            {
                Users user = db.Users.Where(x => x.Login == login).Single();
                var callsDBFrom = db.Calls.Where(x => x.From_ID == user.ID);
                var callsDBTo = db.Calls.Where(x => x.To_ID == user.ID);

                foreach(var item in callsDBTo)
                {
                    Console.WriteLine(" " + item.To_ID + " " + item.From_ID + " " + item.Date_Begin + "  " + item.Date_End);
                }

                CallsHistoryModel call = new CallsHistoryModel();
                foreach(Calls item in callsDBFrom)
                {

                    call.login = db.Users.Where(x => x.ID == item.To_ID).Select(x => x.Login).Single();
                    call.dateBegin = item.Date_Begin;
                    call.dateEnd = item.Date_End;
                    callsHistory.Add(call);
                }

                foreach (Calls item in callsDBTo)
                {

                    call.login = db.Users.Where(x => x.ID == item.From_ID).Select(x => x.Login).Single();
                    call.dateBegin = item.Date_Begin;
                    call.dateEnd = item.Date_End;
                    callsHistory.Add(call);
                }
            }
                
            
            return callsHistory; 
        }


        
    }
}
