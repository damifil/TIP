using HashLib;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TIPySerwer.Models;

namespace TIPySerwer
{
    class Program
    {
        static void Main(string[] args)
        {
            
            
            UserManager.AddUser("Damian", "haslo").Wait();
            UserManager.Logging("Damian", "haslo", "192.168.122.123").Wait();


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
