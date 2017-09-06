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
    /// Interaction logic for login.xaml
    /// </summary>
    public partial class PageLogin : Page 
    {
        LoginRegisterManagment LRM;
        ReadSettings readSettings=ReadSettings.GetInstance;
        SingletoneObject singletone = SingletoneObject.GetInstance;
        public PageLogin()
        {
            InitializeComponent();
            LRM = new LoginRegisterManagment();
        }

        public PageLogin(Boolean setingenable)
        {

            InitializeComponent();
            LRM = new LoginRegisterManagment();
            var parentWindow = this.Parent as Window;

            if (setingenable==true)
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
        private void Button_login(object sender, RoutedEventArgs e)
        {

            string login = loginInput.Text;
            string password = passwordInput.Password;
            string receiveCommunicate = null;
            //utworzenie klienta http odpowiedzialnego za transmisje z serwerem
            if (!ValidationClass.isValidLogin(login))
            {
                MessageBox.Show("Login nie może zawierać spacji oraz znaku '&'");
                return;
            }
            if (!ValidationClass.isValidPassword(password))
            {
                MessageBox.Show("Nowe hasło ma zły format. Hasło musi mieć od 8 do 16 znaków oraz posiadać conajmniej:\n - jedną małą literę\n - jedną dużą literę\n - jedną cyfrę");
                return;
            }
                try
            {
                    singletone.client = new Client(readSettings.IP , Convert.ToInt32(readSettings.PORT));
                    receiveCommunicate = singletone.client.sendMessage("LOGIN " + login + " " + password);
                }
                catch (Exception ex) { MessageBox.Show("Wystapil problem podczas polaczenia z serwererm2");
              
                return; }
         


            if (receiveCommunicate != "OK")
            {
                MessageBox.Show(receiveCommunicate);
                return;
            }
            else if (receiveCommunicate == null)
            {
                MessageBox.Show("Wystapil problem podczas polaczenia z serwererm");
            }
            else
            {
                LRM.loginRegisterfunction(login, password, true);
                MainWindow main = new MainWindow();
                var parentWindow = this.Parent as Window;
                parentWindow.Close();
                main.Show();
            }
        }

        private void Button_goToRegister(object sender, RoutedEventArgs e)
        {
            var parentWindow = this.Parent as Window;
            Page page;
            if (parentWindow.Width == 600) { page = new PageRegister(true); }
            else { page = new PageRegister(false); }
            parentWindow.Content = page;
        }

        private void settingsTextboxaction(object sender, MouseButtonEventArgs e)
        {
            var parentWindow = this.Parent as Window;

            if (parentWindow.Width == 600)
            {
               titlebar.minimalizeButon.Margin= new Thickness(220, 0, 0, 0);
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
        private void closeButon_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
