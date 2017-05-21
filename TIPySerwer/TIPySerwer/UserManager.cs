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
        public static byte[] HashPassword(string password)          // haszowanie hasla
        {
            IHash hash = HashFactory.Crypto.SHA3.CreateKeccak512();
            HashAlgorithm hashAlgo = HashFactory.Wrappers.HashToHashAlgorithm(hash);
            byte[] input = Encoding.UTF8.GetBytes(password);
            byte[] output = hashAlgo.ComputeHash(input);
            return output;
        }
        
        public static bool AddUser(string login, string password)  // dodanie uzytkownika do bazy danych
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
                newUser.Password =  HashPassword(password);
                db.Users.Add(newUser);
                db.SaveChanges();
                return true;
            }
        }

        public static bool UpdateActivityUser(string login)   // aktualizacja ostatniej aktywnosci uzytkownika
        {
            if (!IsLoginExists(login))
            {
                return false;  // uzytkownik nie istnieje, np. usunal juz konto
            }
            using (tipBDEntities db = new tipBDEntities())
            {
                Users user = db.Users.Where(x => x.Login == login).Single();
                user.DateLastActiv = DateTime.Now;                
                db.SaveChanges();
            }
            return true;
        }

        public  static bool Logging(string login, string password)  // proces logowania
        {            
            if (!IsLoginExists(login))
            {
                Console.WriteLine("Błędny login lub hasło");  // tutaj wyslemy do aplikacji klienckiej wiadomosc
                return false;
            }
            using (tipBDEntities db = new tipBDEntities())
            {

                byte[] userPass = db.Users.Where(x => x.Login == login).Select(x => x.Password).SingleOrDefault();
                byte[] pass =  HashPassword(password);
                if (!userPass.SequenceEqual(pass))
                {
                    Console.WriteLine("Błędny login lub hasło");  // tutaj wyslemy do aplikacji klienckiej wiadomosc
                    return false;
                }

                Console.WriteLine("Zostałeś zalogowany");
                return true;
            }

        }

        public static bool ChangePassword(string login, string password)  // zmiana hasla
        {
            if (!IsLoginExists(login))
            {
                return false;  // uzytkownik nie istnieje, np. usunal juz konto
            }
            using (tipBDEntities db = new tipBDEntities())
            {
                Users user = db.Users.Where(x => x.Login == login).Single();
                byte[] pass = Encoding.UTF8.GetBytes(password);
                user.Password = pass;
                db.SaveChanges();
                return true;
            }
        }

        public static bool SavaCall(string login, string login1, DateTime dateBegin)   // zapisanie rozmowy 
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
                db.SaveChanges();
            }
            return true;
        }

        public static string GetCalls(string login)   // pobranie wszystkich rozmow danego uzytkownika
        {
            string history = "";
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

            callsHistory = callsHistory.OrderBy(x => x.dateBegin).ToList();
            foreach (CallsHistoryModel item in callsHistory)
            {
                history = history + item.login + " " + item.dateBegin + " " + item.dateEnd + "&";
            }
            return history;

        }
        public static string GetCallsConcreteUser(string login, string login1)  // pobranie rozmow z konkretym uzytkownikiem
        {
            string listCallsHistory = "";
            List<CallsHistoryModel> callsHistory = new List<CallsHistoryModel>();
            using (tipBDEntities db = new tipBDEntities())
            {
                Users user = db.Users.Where(x => x.Login == login).Single();
                Users user1 = db.Users.Where(x => x.Login == login1).Single();
                var callsDBFrom = db.Calls.Where(x => x.From_ID == user.ID && x.To_ID == user1.ID);
                var callsDBTo = db.Calls.Where(x => x.To_ID == user.ID && x.From_ID == user1.ID);

                foreach (Calls item in callsDBFrom) 
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
                callsHistory = callsHistory.OrderBy(x => x.dateBegin).ToList();
                foreach(CallsHistoryModel item in callsHistory)
                {
                    listCallsHistory = listCallsHistory + item.login + " " + item.dateBegin + " " + item.dateEnd + "&";
                }
                return listCallsHistory;
            }
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
        
        public static string SearchUser(string loginUser, string loginSearch)
        {
            using (tipBDEntities db = new tipBDEntities())
            {               
               /* bool isExists = db.Users.Contains(x => x.Login == loginSearch);
                if(!isExists)
                {
                    return String.Empty; // brak uzytkownikow o podanej nazwie
                }*/

                var users = from us in db.Users
                            where us.Login.Contains(loginSearch)
                            select us;

                string listUsers = "";
                int usersCount = 0;
                foreach(Users item in users)
                {
                    listUsers = listUsers + "" + item.Login + "&";
                    usersCount++;
                }
                listUsers = listUsers + "" + usersCount;
                return listUsers;
            }
        }

    }
}
