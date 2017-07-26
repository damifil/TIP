using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIPySerwer
{
    class testDawid
    {
        public static void drugiMejn()
        {
            
            //UserManager.AddUser("Dawid", "haslo");
           // UserManager.Logging("Damian", "haslo");
           // UserManager.SavaCall("Dawid", "Damian", DateTime.Now);
            /*List<CallsHistoryModel> listCalls = UserManager.GetCalls("Andrzej");
            foreach(CallsHistoryModel item in listCalls)
            {
                Console.WriteLine("Rozmowa z użytkownikiem " + item.login + " rozpoczęła się " + item.dateBegin
                    + " i zakończyła się " + item.dateEnd);
            }
            */

           /* List<CallsHistoryModel> listCalls = UserManager.GetCallsConcreteUser("Damian", "Dawid");
            foreach (CallsHistoryModel item in listCalls)
            {
                Console.WriteLine("Rozmowa z użytkownikiem " + item.login + " rozpoczęła się " + item.dateBegin
                    + " i zakończyła się " + item.dateEnd);
            }*/
            


            //UserManager.UpdateActivityUser("Dawid").Wait();
          //  Console.WriteLine("b " + UserManager.AddFriend("Dawid", "Robert"));
            Console.WriteLine("Znajomi Damiana: " + UserManager.GetFriends("Damian"));

            //Console.WriteLine("Szukajka: " + UserManager.SearchUser("Da"));

            /*
                        tipBDEntities db = new tipBDEntities();
                        Users user = new Users();
                        user.Login = "A1dam1";
                        user.Password = "adam";
                        user.IP_Address = "192.168.123.101";
                        user.DateLastActiv = DateTime.Now;
                        db.Users.Add(user);

                       Calls call = new Calls();
                        call.From_ID = 5;
                        call.To_ID = 6;
                        call.Date_Begin = DateTime.Now;
                        call.Date_End = DateTime.Now;
                        db.Calls.Add(call);

                        db.SaveChanges();
                        */
            /*var users = db.Users;
            foreach(Users u in users)
            {
                Console.WriteLine(u.Login + " p: " + u.Password + " IP: " + u.IP_Address + " date: " + u.DateLastActiv);
            }
            var calls = db.Calls;
            foreach (Calls c in calls)
            {
                Console.WriteLine(c.From_ID + " "  + c.To_ID);
            }*/
        }
    }
}
