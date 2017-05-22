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
        public static string login = "";
        public static byte[] pass;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.userName = loginInput.Text;
            login = loginInput.Text;
            pass = HashPassword(passwordInput.Password);

            Thread threadServer = new Thread(new ThreadStart(doDziela));
            threadServer.Start();
            /// wyslanie 
            this.Close();
            main.Show();
        }


        public static void doDziela()
        {
           
            string password = Encoding.UTF8.GetString(pass, 0, pass.Length);
            Client client = new Client("192.168.8.100", 5555);
            Console.WriteLine(login + " pass " + password + "\n");
            client.sendMessage("REGISTER " + login+ " " + password);
        }
       

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
