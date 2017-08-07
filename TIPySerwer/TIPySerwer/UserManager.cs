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
                newUser.Is_Exists = true;
                db.Users.Add(newUser);
                db.SaveChanges();
                return true;
            }
        }

        public static bool DeleteUser(string login) // usuniecie konta
        {
            if (!IsLoginExists(login))
            {
                return false;  // uzytkownik nie istnieje, np. usunal juz konto
            }

            using (tipBDEntities db = new tipBDEntities())
            {

                Users user = db.Users.Where(x => x.Login == login).Single();
                user.Is_Exists = false;
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
                user.Is_Active = true;
                db.SaveChanges();
            }
            return true;
        }

        public  static string Logging(string login, string password)  // proces logowania
        {            
            if (!IsLoginExists(login))
            {
                Console.WriteLine("Błędny login lub hasło (akcja z loginem: "+login+" )");  
                return "Błędny login lub hasło";
            }
            using (tipBDEntities db = new tipBDEntities())
            {
                Users user = db.Users.Where(x => x.Login == login).SingleOrDefault();
                if(!(bool)user.Is_Exists)
                {
                    Console.WriteLine("Błędny login lub hasło (akcja z loginem: " + login + " )");  
                    return "Błędny login lub hasło";
                }
                byte[] userPass = user.Password;
                byte[] pass =  HashPassword(password);
                if (!userPass.SequenceEqual(pass))
                {
                    Console.WriteLine("Błędny login lub hasło (akcja z loginem: " + login + " )");  
                    return "Błędny login lub hasło";
                }
                Console.WriteLine("Klient "+ login+" zalogował się");
                return "True";
            }
        }


        public static bool LogOff(string login)  // proces wylogowania
        {
            if (!IsLoginExists(login))
            {
                Console.WriteLine("Błędny login lub hasło(akcja z loginem: " + login + ")");  // tutaj wyslemy do aplikacji klienckiej wiadomosc
                return false;
            }
            using (tipBDEntities db = new tipBDEntities())
            {

                Users user = db.Users.Where(x => x.Login == login).Single();
                user.Is_Active = false;
                db.SaveChanges();
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

        

        public static bool SavaCall(string login, string login1, string dateBegin, string hourBegin, string dateEnd, string hourEnd)   // zapisanie rozmowy 
        {
            using (tipBDEntities db = new tipBDEntities())
            {
                Users user = db.Users.Where(x => x.Login == login).Single();
                Users user1 = db.Users.Where(x => x.Login == login1).Single();

                Calls call = new Calls();
                call.From_ID = user.ID;
                call.To_ID = user1.ID;
                call.Date_Begin = DateTime.Parse(dateBegin + " " + hourBegin);
                call.Date_End = DateTime.Parse(dateEnd+ " " + hourEnd);
                db.Calls.Add(call);
                db.SaveChanges();
            }
            return true;
        }
        public static string UserLastActivity(string login)
        {
            using (tipBDEntities db = new tipBDEntities())
            {
                Users user = db.Users.Where(x => x.Login == login).Single();
                return user.DateLastActiv.ToString();
            }
        }
        public static string GetAllHistory(string login)   // pobranie wszystkich rozmow danego uzytkownika
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

        public static bool DelFriend(string login, string oldFriend) // usuniecie znajomego
        {
            using (tipBDEntities db = new tipBDEntities())
            {

                Users user = db.Users.Where(x => x.Login == login).Single();
                Users oFriend = db.Users.Where(x => x.Login == oldFriend).Single();
                bool checkHasFriend = db.Friends.Where(x => x.UserID == user.ID && x.UserID_From == oFriend.ID).Any();
                if(checkHasFriend == true)
                {
                    Friends friends = db.Friends.Where(x => x.UserID == user.ID && x.UserID_From == oFriend.ID).Single();
                    db.Friends.Remove(friends);
                    db.SaveChanges();
                    return true;
                }
                bool checkHasFriend1 = db.Friends.Where(x => x.UserID == oFriend.ID && x.UserID_From == user.ID).Any();
                if (checkHasFriend1 == true)
                {
                    Friends friends = db.Friends.Where(x => x.UserID == oFriend.ID && x.UserID_From == user.ID).Single();
                    db.Friends.Remove(friends);
                    db.SaveChanges();
                    return true;
                }

            }
            return false;

        }
        public static string GetFriends(string login)        // pobranie listy znajomych
        {
            List<FriendsListModel> friendsList = new List<FriendsListModel>();
            string listFriends = "";
            using (tipBDEntities db = new tipBDEntities())
            {
                Users user = db.Users.Where(x => x.Login == login).Single();
                bool checkHasFriends = db.Friends.Where(x => x.UserID == user.ID).Any();
                if(!checkHasFriends)
                {
                                       // brak znajomych
                }
                else
                {
                    var friends = db.Friends.Where(x => x.UserID == user.ID);
                    foreach (Friends item in friends)
                    {
                
                        Users friend = db.Users.Where(x => x.ID == item.UserID_From).SingleOrDefault();
                        friendsList.Add(new FriendsListModel()
                        {
                            login = friend.Login,
                            isActive = (bool)friend.Is_Active
                        });
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
                        Users friend = db.Users.Where(x => x.ID == item.UserID && x.Is_Exists == true).SingleOrDefault();
                        friendsList.Add(new FriendsListModel()
                        {
                            login = friend.Login,
                            isActive = (bool)friend.Is_Active
                        });
                    }
                }
            }

            friendsList = friendsList.OrderByDescending(user => user.isActive).ThenBy(user => user.login).ToList();
            foreach (FriendsListModel item in friendsList)
            {
                listFriends = listFriends + item.login + " " + item.isActive + "&";  
            }
            return listFriends;
        }

      
        public static string SearchUser(string loginUser, string loginSearch)
        {
            using (tipBDEntities db = new tipBDEntities())
            {               
                var users = from us in db.Users
                            where us.Login.Contains(loginSearch) && us.Is_Exists == true
                            select us;

                string listUsers = "";
                users = users.OrderByDescending(user => user.Is_Active).ThenBy(user => user.Login);
                foreach(Users item in users)
                {
                    if(item.Login != loginUser)
                        listUsers = listUsers + item.Login + " " + item.Is_Active + "&";
                    Console.WriteLine(item.Login);
                }
                return listUsers;
            }
        }

        }
}
