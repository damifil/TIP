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
        SingletoneObject singletone = SingletoneObject.GetInstance;
        public register()
        {
            InitializeComponent();
            LRM = new LoginRegisterManagment();
        }

        public register(Boolean setingenable)
        {
            InitializeComponent();
            LRM = new LoginRegisterManagment();

            var parentWindow = this.Parent as Window;

            if (setingenable == true)
            {
                titlebar.minimalizeButon.Margin = new Thickness(520, 0, 0, 0);
                settings.Text = "\uE5C4";
            }
            else
            {
                titlebar.minimalizeButon.Margin = new Thickness(220, 0, 0, 0);
                settings.Text = "\uE5D2";
            }
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
            if (singletone.client == null)
            {
                readSettings = ReadSettings.GetInstance;
                singletone.client = new Client(readSettings.IP, Convert.ToInt32(readSettings.PORT));
            }
            string flag = singletone.client.sendMessage("REGISTER " + login + " " + password1);
            if(flag == "ERROR")
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
            var parentWindow = this.Parent as Window;
            Page page;
            if (parentWindow.Width == 600) { page = new login(true); }
            else { page = new login(false); }
            parentWindow.Content = page;
        }
        private void settingsTextboxaction(object sender, MouseButtonEventArgs e)
        {
            var parentWindow = this.Parent as Window;

            if (parentWindow.Width == 600)
            {
                titlebar.minimalizeButon.Margin = new Thickness(220, 0, 0, 0);
                parentWindow.Width = 300;
                settings.Text = "\uE5D2";
            }
            else
            {
                titlebar.minimalizeButon.Margin = new Thickness(520, 0, 0, 0);
                parentWindow.Width = 600;
                settings.Text = "\uE5C4";
            }
        }

    }
}
