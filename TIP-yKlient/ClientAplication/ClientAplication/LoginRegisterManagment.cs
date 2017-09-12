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
    /// <summary>
    /// Klasa odpowiedzialna za logowanie lub rejestracje użytkownika do serwerów
    /// </summary>
    /// <remarks>
    /// Wykorzystywana jest w klasach PageLogin oraz PageRegister
    /// </remarks>
    public class LoginRegisterManagment
    {
        /// <summary>
        /// obiekt klasy <c>SingletoneObject</c>
        /// </summary>
        SingletoneObject singletoneOBj;

        /// <summary>
        /// czas w ms po którym wysyłana jest informacja do serwera o tym żę użyktonik jest aktywny
        /// </summary>
        int timeThreadloop = 30000;


        /// <summary>
        /// metoda odpowiedzialna za hashowanie hasła
        /// </summary>
        /// <param name="password">hasło użytkownika</param>
        /// <returns>zwraca tablice byte z shasowanym hasłem</returns>
        private static byte[] HashPassword(string password)          
        {
            IHash hash = HashFactory.Crypto.SHA3.CreateKeccak512();
            HashAlgorithm hashAlgo = HashFactory.Wrappers.HashToHashAlgorithm(hash);
            byte[] input = Encoding.UTF8.GetBytes(password);
            byte[] output = hashAlgo.ComputeHash(input);
            return output;
        }


        /// <summary>
        /// metoda wykorzystywana w osobnym wątku której zadaniem 
        /// jest wysyłanie okresowo informacji do serwera o tym żę użytkownik jest aktywny
        /// </summary>
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



        /// <summary>
        /// metoda odpwoeidzialna za otrzymanie listy znajoomych z serwera
        /// </summary>
        /// <param name="login">Login użytkownika dla którego ma zostać zwrócona lista użytkownikó</param>
        /// <returns>zwraca listę użytkowników</returns>
        private List<ListUser> GetFriends(string login)
        {
            string friendsList = singletoneOBj.client.sendMessage("GETFRIENDS " + login);
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



        /// <summary>
        /// Funkcja odpowiedzialna za logowanie lub rejestracje w zależnośći od podanego parametru
        /// podczas wykonywania tej funkcji usalane jest połączenie z serwerwem OZEKi
        /// oraz uruchamioany jest osobny wątek odpowiedzialny za informowanie serwera o aktywnośći użytkownika
        /// </summary>
        /// <param name="login">Login użytkownika</param>
        /// <param name="password">Hasło użytkownika</param>
        /// <param name="islogin">Czy jest to logowanie dla wartośći true wykonuje się akcaj dla logowaniea
        /// natomiast dla wartośći false rejestracji</param>
        public void loginRegisterfunction(string login, string password, bool islogin)
        {
            singletoneOBj = SingletoneObject.GetInstance;
            AplicationUser userToSend = new AplicationUser();
            userToSend.Name = login;
            userToSend.password = password;
            singletoneOBj.user = userToSend;
            singletoneOBj.listUsers = GetFriends(login);
            singletoneOBj.phoneVOIP = new PhoneVOIP();
            try
            {
                singletoneOBj.phoneVOIP.InitializeSoftPhone(singletoneOBj.user.Name, singletoneOBj.user.password, singletoneOBj.client.ipAddres, 5060);
                singletoneOBj.phoneVOIP.client = singletoneOBj.client;
                singletoneOBj.phoneVOIP.userLogged = singletoneOBj.user;
            }
            catch (Exception ex) { MessageBox.Show("Wystapił problem podczas podpięcia do serwera odpowiedzialnego za transmisje głosową "); }
           
            if (islogin == true)
            {
                singletoneOBj.user.lastActivity = "Twoja ostatnia aktywność była: \n" + singletoneOBj.client.sendMessage("LASTACTIVITY " + login);
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
