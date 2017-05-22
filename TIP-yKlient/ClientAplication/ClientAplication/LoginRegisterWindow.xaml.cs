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
    /// <summary>
    /// Interaction logic for LoginRegisterWindow.xaml
    /// </summary>
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


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.userName = loginInput.Text;
            string login = loginInput.Text;
            string password = passwordInput.Password;
            if (client == null) {
             client= new Client(adresIPinput.Text, Convert.ToInt32(numberPortInput.Text));
            }
            string flag = client.sendMessage("LOGIN " + login + " " + password);
            if(flag == "False")
            {
                MessageBox.Show("Login lub hasło jest błędne");
                return;
            }

            this.Close();
            main.Show();
        }


        public static void doDziela()
        {
           
           
        }
       

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.userName = loginInputRegister.Text;
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
            Console.WriteLine("flag" + flag);
            if(flag == "False")
            {
                MessageBox.Show("Login jest zajęty");
                return;
            }
            this.Close();
            main.Show();
            main.client = client;

        }
    }
}
