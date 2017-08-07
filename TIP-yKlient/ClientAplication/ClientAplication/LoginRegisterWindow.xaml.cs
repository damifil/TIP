﻿using HashLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClientAplication
{
    
    public partial class LoginRegisterWindow : Window
    {
        internal Client client;
        SingletoneObject singletoneOBj;
        int timeThreadloop = 30000;
        public LoginRegisterWindow()
        {
            InitializeComponent();
        }
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
                singletoneOBj.listUsers = new List<ListUser>();
                for (int i = 0; i < (splitFriends.Length - 1); i++)
                {
                    ListUser user = new ListUser();
                    string[] sp = splitFriends[i].Split(' ');
                    user.name = sp[0];
                    user.active = sp[1];
                    singletoneOBj.listUsers.Add(user);
                }
                Thread.Sleep(timeThreadloop);
            }
        }

        private void Button_login(object sender, RoutedEventArgs e)
        {
            disable_enableButton(false);
            string login = loginInput.Text;
            string password = passwordInput.Password;
            string receiveCommunicate= null;
            //utworzenie klienta http odpowiedzialnego za transmisje z serwerem
            if (client == null) {
                try
                {
                    client = new Client(adresIPinput.Text, Convert.ToInt32(numberPortInput.Text));
                    receiveCommunicate = client.sendMessage("LOGIN " + login + " " + password);
                }
                catch (Exception ex) { MessageBox.Show("Wystapil problem podczas polaczenia z serwererm"); disable_enableButton(true); }
            }

            if(client == null)
            {
                disable_enableButton(true);
                return;
            }
            if(receiveCommunicate != "True")
            {
                MessageBox.Show(receiveCommunicate);
                disable_enableButton(true);
                return;
            }
            else if(receiveCommunicate == null)
            {
                MessageBox.Show("Wystapil problem podczas polaczenia z serwererm");
                disable_enableButton(true);
            }
            else
            {
                loginRegisterfunction(login, password,true);
                MainWindow main = new MainWindow();
                this.Close();
                main.Show();
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

        private void disable_enableButton(bool isenabled)
        {
            loginButton.IsEnabled = isenabled;
            registerButton.IsEnabled = isenabled;
            loginInput.IsEnabled = isenabled;
            password1InputRegister.IsEnabled = isenabled;
            passwordInput.IsEnabled = isenabled;
            password2InputRegister.IsEnabled = isenabled;
            adresIPinput.IsEnabled = isenabled;
            numberPortInput.IsEnabled = isenabled;  
        }
        private void Button_register(object sender, RoutedEventArgs e)
        {
            disable_enableButton(false);
            string login = loginInputRegister.Text;
            string password1 = password1InputRegister.Password;
            string password2 = password2InputRegister.Password;
            if(password1 != password2)
            {
                MessageBox.Show("Hasła nie są takie same");
                return;
            }
            if (client == null)
            {
                client = new Client(adresIPinput.Text, Convert.ToInt32(numberPortInput.Text));
            }
            string flag = client.sendMessage("REGISTER " + login + " " + password1);
            if(flag == "False")
            {
                MessageBox.Show("Login jest zajęty");
                disable_enableButton(true);
                return;
            }
            loginRegisterfunction(login, password1,false);
            MainWindow main = new MainWindow();
            this.Close();
            main.Show();
        }
        private void loginRegisterfunction(string login, string password,bool islogin)
        {
            singletoneOBj = SingletoneObject.GetInstance;
            User userToSend = new User(true);
            userToSend.Name = login;
            userToSend.password = password;
            singletoneOBj.user = userToSend;
            singletoneOBj.listUsers = GetFriends(login);
            singletoneOBj.client = client;
            singletoneOBj.phoneVOIP = new PhoneVOIP();
            try
            {
                singletoneOBj.phoneVOIP.InitializeSoftPhone(singletoneOBj.user.Name, singletoneOBj.user.password, client.ipAddres, 5060);
            }
            catch (Exception ex) {MessageBox.Show("Wystapił problem podczas podpięcia do serwera odpowiedzialnego za transmisje głosową ");}
            singletoneOBj.phoneVOIP.client = client;
            singletoneOBj.phoneVOIP.userLogged = singletoneOBj.user;
            if (islogin == true)
            {
                singletoneOBj.user.lastActivity = "Twoja ostatnia aktywność była: \n"+client.sendMessage("LASTACTIVITY " + login);
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
