using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public static byte[] HashPassword(string password)          // haszowanie hasla
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
            this.Close();
            main.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
