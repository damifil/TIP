using HashLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ClientAplication
{
    public class LoginRegisterManagment
    {
        internal Client client;
        SingletoneObject singletoneOBj;
        int timeThreadloop = 30000;

        private static byte[] HashPassword(string password)          // haszowanie hasla
        {
            IHash hash = HashFactory.Crypto.SHA3.CreateKeccak512();
            HashAlgorithm hashAlgo = HashFactory.Wrappers.HashToHashAlgorithm(hash);
            byte[] input = Encoding.UTF8.GetBytes(password);
            byte[] output = hashAlgo.ComputeHash(input);
            return output;
        }


        private void isOnlineloop()
        {
            while (true)
            {
                string friendsList = singletoneOBj.client.sendMessage("ISONLINE " + singletoneOBj.user.Name);
                string[] splitFriends = friendsList.Split('&');
                List<ListUser> listUsers = new List<ListUser>();
               
                for (int i = 0; i < (splitFriends.Length - 1); i++)
                {
                    ListUser user = new ListUser();
                    string[] sp = splitFriends[i].Split(' ');
                    user.name = sp[0];
                    user.active = sp[1];
                    listUsers.Add(user);
                }
              
                    singletoneOBj.listUsers = new List<ListUser>();
                    singletoneOBj.listUsers = listUsers;
                    singletoneOBj.listusercompare = true;
               
                Thread.Sleep(timeThreadloop);
            }
        }



        private List<ListUser> GetFriends(string login)
        {
            string friendsList = client.sendMessage("GETFRIENDS " + login);
            string[] splitFriends = friendsList.Split('&');
            List<ListUser> listUsers = new List<ListUser>();
            for (int i = 0; i < (splitFriends.Length - 1); i++)
            {
                ListUser user = new ListUser();
                string[] sp = splitFriends[i].Split(' ');
                user.name = sp[0];
                user.active = sp[1];
                listUsers.Add(user);
            }
            return listUsers;
        }



        public void loginRegisterfunction(string login, string password, bool islogin)
        {
            singletoneOBj = SingletoneObject.GetInstance;
            AplicationUser userToSend = new AplicationUser();
            userToSend.Name = login;
            userToSend.password = password;
            singletoneOBj.user = userToSend;
            singletoneOBj.listUsers = GetFriends(login);
            singletoneOBj.client = client;
            singletoneOBj.phoneVOIP = new PhoneVOIP();
            try
            {
                singletoneOBj.phoneVOIP.InitializeSoftPhone(singletoneOBj.user.Name, singletoneOBj.user.password, client.ipAddres, 5060);
                singletoneOBj.phoneVOIP.client = client;
                singletoneOBj.phoneVOIP.userLogged = singletoneOBj.user;
            }
            catch (Exception ex) { MessageBox.Show("Wystapił problem podczas podpięcia do serwera odpowiedzialnego za transmisje głosową "); }
           
            if (islogin == true)
            {
                singletoneOBj.user.lastActivity = "Twoja ostatnia aktywność była: \n" + client.sendMessage("LASTACTIVITY " + login);
            }
            else
            {
                singletoneOBj.user.lastActivity = "Cieszymy się, że dołączyłeś do społeczności aplikacji :D";
            }
            singletoneOBj.isOnlineThread = new Thread(isOnlineloop);
            singletoneOBj.isOnlineThread.IsBackground = true;
            singletoneOBj.isOnlineThread.Start();
        }
    }
}
