﻿using System;
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
    public partial class login : Page 
    {
        LoginRegisterManagment LRM;
        ReadSettings readSettings;
        public login()
        {
            InitializeComponent();
            LRM = new LoginRegisterManagment();
        }

        private void Button_login(object sender, RoutedEventArgs e)
        {

            string login = loginInput.Text;
            string password = passwordInput.Password;
            string receiveCommunicate = null;
            //utworzenie klienta http odpowiedzialnego za transmisje z serwerem
            readSettings = ReadSettings.GetInstance;

            if (LRM.client == null)
            {
                try
                {
                    LRM.client = new Client(readSettings.IP , Convert.ToInt32(readSettings.PORT));
                    receiveCommunicate = LRM.client.sendMessage("LOGIN " + login + " " + password);
                }
                catch (Exception ex) { MessageBox.Show("Wystapil problem podczas polaczenia z serwererm");  }
            }

            if (LRM.client == null)
            {
               
                return;
            }
            if (receiveCommunicate != "True")
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
            var page = new register();
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