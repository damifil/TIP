using HashLib;
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


        private void Button_login(object sender, RoutedEventArgs e)
        {
            string login = loginInput.Text;
            string password = passwordInput.Password;
            //utworzenie klienta http odpowiedzialnego za transmisje z serwerem
            if (client == null) {
                try
                {
                    client = new Client(adresIPinput.Text, Convert.ToInt32(numberPortInput.Text));
                }
                catch (Exception ex) { MessageBox.Show("Wystapil problem podczas polaczenia z serwererm"); }

            }

            if(client == null)
            {
                return;
            }
            string flag = client.sendMessage("LOGIN " + login + " " + password);
            if(flag == "False")
            {
                MessageBox.Show("Login lub hasło jest błędne");
                return;
            }

           

           
            MainWindow main = new MainWindow();
            User userToSend = new User();
            userToSend.Name = loginInput.Text;
            userToSend.password = password;
            main.user = userToSend;
            main.listUsers = GetFriends(loginInput.Text);
            main.client = client;
            this.Close();
            main.Show();
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

        private void Button_register(object sender, RoutedEventArgs e)
        {
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
                return;
            }

            //nalezy sie zastanowic tutaj czy po rejestracji jestesmy juz zalogowani
            User userToSend = new User();
            userToSend.Name = login;
            userToSend.password = password1;
            MainWindow main = new MainWindow();
            main.user = userToSend;
            main.client = client;
            this.Close();
            main.Show();
           

        }
    }
}
