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

        public async static Task<bool> UpdateActivityUser(string login)   // aktualizacja ostatniej aktywnosci uzytkownika
        {
            if (!IsLoginExists(login))
            {
                return false;  // uzytkownik nie istnieje, np. usunal juz konto
            }
            using (tipBDEntities db = new tipBDEntities())
            {
                Users user = db.Users.Where(x => x.Login == login).Single();
                user.DateLastActiv = DateTime.Now;
                
                await db.SaveChangesAsync();
            }
            return true;
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
            List<CallsHistoryModel> callsHistory = new List<CallsHistoryModel>();
            using (tipBDEntities db = new tipBDEntities())
            {
                Users user = db.Users.Where(x => x.Login == login).Single();
                var callsDBFrom = db.Calls.Where(x => x.From_ID == user.ID);
                var callsDBTo = db.Calls.Where(x => x.To_ID == user.ID);
                
                foreach(Calls item in callsDBFrom)
                {
                    CallsHistoryModel call = new CallsHistoryModel();
                    call.login = db.Users.Where(x => x.ID == item.To_ID).Select(x => x.Login).Single();
                    call.dateBegin = item.Date_Begin;
                    call.dateEnd = item.Date_End;
                    callsHistory.Add(call);                   
                }
                
                foreach (Calls item in callsDBTo)
                {
                    CallsHistoryModel call = new CallsHistoryModel();
                    call.login = db.Users.Where(x => x.ID == item.From_ID).Select(x => x.Login).Single();
                    call.dateBegin = item.Date_Begin;
                    call.dateEnd = item.Date_End;
                    callsHistory.Add(call);
                }                
            }
            
            return callsHistory.OrderBy(x => x.dateBegin).ToList();
        }

        public static bool AddFriend(string login, string newFriend)  // dodanie znajomego
        {
            using (tipBDEntities db = new tipBDEntities())
            {
                Users user = db.Users.Where(x => x.Login == login).Single();
                Users nFriend = db.Users.Where(x => x.Login == newFriend).Single();               
                bool checkHasFriend = db.Friends.Where(x => x.UserID == user.ID && x.UserID_From == nFriend.ID).Any();
                bool checkHasFriend1 = db.Friends.Where(x => x.UserID == nFriend.ID && x.UserID_From == user.ID).Any();
                
                if (checkHasFriend == true || checkHasFriend1 == true)
                {
                    return false;    // uzytkownik ma juz takiego znajomego
                }

                Friends friend = new Friends();
                friend.UserID = nFriend.ID;
                friend.UserID_From = user.ID;
                db.Friends.Add(friend);
                db.SaveChanges();
            }
            return true;
        }

        public static string GetFriends(string login)        // pobranie listy znajomych
        {

            string listFriends = "";
            using (tipBDEntities db = new tipBDEntities())
            {
                Users user = db.Users.Where(x => x.Login == login).Single();
                bool checkHasFriends = db.Friends.Where(x => x.UserID == user.ID).Any();
                int friendsCount = 0;
                if(!checkHasFriends)
                {
                                       // brak znajomych
                }
                else
                {
                    var friends = db.Friends.Where(x => x.UserID == user.ID);
                    foreach (Friends item in friends)
                    {
                        Users friend = db.Users.Where(x => x.ID == item.UserID_From).Single();
                        listFriends = listFriends + friend.Login + "&"; // znak & oddziela jeden login od drugiego 
                        friendsCount++;
                    }
                }
                

                checkHasFriends = db.Friends.Where(x => x.UserID_From == user.ID).Any();
                if(!checkHasFriends)
                {
                                        // brak znajomych
                }
                else
                {
                    var friends1 = db.Friends.Where(x => x.UserID_From == user.ID);
                    foreach (Friends item in friends1)
                    {
                        Users friend = db.Users.Where(x => x.ID == item.UserID).Single();
                        listFriends = listFriends + friend.Login + "&";
                        friendsCount++;
                    }
                }
                listFriends = listFriends + friendsCount;   // dodanie na koncu liczby znajomych
            }
            return listFriends;
        }
        
    }
}
