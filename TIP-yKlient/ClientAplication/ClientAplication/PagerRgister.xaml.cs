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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientAplication
{
    /// <summary>
    /// Interaction logic for register.xaml
    /// </summary>
    public partial class register : Page
    {
        LoginRegisterManagment LRM;
        ReadSettings readSettings;
        public register()
        {
            InitializeComponent();
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
            if (LRM.client == null)
            {
                readSettings = ReadSettings.GetInstance;
                LRM.client = new Client(readSettings.IP, Convert.ToInt32(readSettings.PORT));
            }
            string flag = LRM.client.sendMessage("REGISTER " + login + " " + password1);
            if(flag == "False")
            {
                MessageBox.Show("Login jest zajęty");
               
                return;
            }
            LRM.loginRegisterfunction(login, password1,false);
            MainWindow main = new MainWindow();
            var parentWindow = this.Parent as Window;
            parentWindow.Close();
            main.Show();
            
        }

        private void Button_gotoLogin(object sender, RoutedEventArgs e)
        {
            var page = new login();
            var parentWindow = this.Parent as Window;
            parentWindow.Content = page;
        }
        private void settingsTextboxaction(object sender, MouseButtonEventArgs e)
        {
            var parentWindow = this.Parent as Window;
            if (parentWindow.Width == 600)
            {
                parentWindow.Width = 300;
            }
            else
            {
                parentWindow.Width = 600;
            }
        }

    }
}
