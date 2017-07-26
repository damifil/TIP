﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientAplication
{


    public partial class MainWindow : Window
    {
        internal ObservableCollection<User> Friends;
        internal ObservableCollection<User> Users;
        internal User user;
        internal Client client;
        internal PhoneVOIP phoneVOIP;
        internal List<ListUser> listUsers; // lista uzytkownikow
        internal List<History> listHistory; // lista rozmow

        public MainWindow()
        {
            InitializeComponent();
        }

        bool _shown;
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_shown)
                return;
            userNameTB.Text = user.Name;
            _shown = true;
            if (Users != null)
            { lbUsers.DataContext = Users; }
            else
            { addUSerToList(); }
            phoneVOIP = new PhoneVOIP();
            lastActivity.Text = "twoja ostatnia aktywność: !2 kwietnia o godzinie 14:30";
            welcomeString.Text = "Witaj nazwa_Użytkownika";
            try
            {
                phoneVOIP.InitializeSoftPhone(user.Name, user.password, client.ipAddres, 5060);
            }
            catch (Exception ex) { MessageBox.Show("Wystapil problem podczas podpiecia do serwera odpowiedzialnego za transmisje glosowa "); }
            phoneVOIP.client = client;
            phoneVOIP.userLogged = user;
        }

        //inicjalizacja odpowiedzialna za wyswietlanie znajomych (wstepne testowanie wyswietlania)

        internal void addUSerToList()
        {
            Users = new ObservableCollection<User>();
            Friends = new ObservableCollection<User>();
            if (listUsers != null)
            {
                foreach (ListUser item in listUsers)
                {
                    if (item.active == "True")
                    {
                        Users.Add(new User() { Name = item.name, IcoCall = "\uf098", IcoUser = "\uf007" }); // aktywny
                        Friends.Add(new User() { Name = item.name, IcoCall = "\uf098", IcoUser = "\uf007" });
                    }
                    else
                    {
                        Users.Add(new User() { Name = item.name, IcoCall = "\uf098", IcoUser = "\uf2c0" }); // nieaktywny
                        Friends.Add(new User() { Name = item.name, IcoCall = "\uf098", IcoUser = "\uf2c0" });
                    }
                }
            }
            lbUsers.DataContext = Friends;
        }
           
        private List<ListUser> SearchUsers(string login, string login1)              // szukanie uzytkownikow
        {
            string searchList = client.sendMessage("SRCH " + login + " " + login1);
            string[] splitSearch = searchList.Split('&');
            List<ListUser> listUsers = new List<ListUser>();
            for (int i = 0; i < (splitSearch.Length - 1); i++)
            {
                ListUser user = new ListUser();
                string[] sp = splitSearch[i].Split(' ');
                user.name = sp[0];
                user.active = sp[1];
                listUsers.Add(user);
            }
            return listUsers;
        }

        private void searchClick(object sender, RoutedEventArgs e)
        {
            string value = searchInput.Text;

            if (string.IsNullOrWhiteSpace(value))    // gdy nic nie wpisano w polu wyszukiwania
            {
                lbUsers.DataContext = Friends;
                return;
            }
            List<ListUser> listUser = SearchUsers(user.Name, value);

            Users = new ObservableCollection<User>(Friends);
            foreach (ListUser item in listUser)
            {
                if(!Users.Where(x => x.Name == item.name).Any())
                if (item.active == "True")
                {

                    Users.Add(new User() { Name = item.name, IcoCall = "\uf098", IcoUser = "\uf234" }); // aktywny
                }
                else
                {
                    Users.Add(new User() { Name = item.name, IcoCall = "\uf098", IcoUser = "\uf234" }); // nieaktywny
                }
            }
            lbUsers.DataContext = Users;
        }

        private void callToUser(object sender, MouseButtonEventArgs e)
        {
            User userLogged = user;
            TextBlock cmd = (TextBlock)sender;
            if (cmd.DataContext is User)
            {
               
                User user = (User)cmd.DataContext;
                bool call = phoneVOIP.btn_PickUp_Click(user.Name);
                CallToWindow main = new CallToWindow();
                if (call == true)
                {
                    main.user = user;
                    main.userLogged = userLogged;
                    main.client = client;
                    main.phoneVOIP = phoneVOIP;
                    main.Show();
                }

            }
        }


        private List<ListHistory> GetConcreteHistory(string login, string login1)               // uzyskanie hisorii rozmow od konkretnego uzytkownika
        {
            string historyListString = client.sendMessage("USERHISTORY " + login + " " + login1);
            string[] historySplit = historyListString.Split('&');
            List<ListHistory> historyList = new List<ListHistory>();
            for (int i = 0; i < (historySplit.Length - 1); i++)
            {
                ListHistory history = new ListHistory();
                string[] sp = historySplit[i].Split(' ');
                history.userName = sp[0];
                history.dayBegin = sp[1];
                history.hourBegin = sp[2];
                history.dayEnd = sp[3];
                history.hourEnd = sp[4];
                historyList.Add(history);
            }
            return historyList;
        }

        private void goToUser(object sender, MouseButtonEventArgs e)
        {
            TextBlock cmd = (TextBlock)sender;
            if (cmd.DataContext is User)
            {
                User userFriend = (User)cmd.DataContext; 
                List<ListHistory> listHistory = GetConcreteHistory(user.Name, userFriend.Name);
                UserWindow main = new UserWindow(listHistory);
                main.Users = Users;
                App.Current.MainWindow = main;
                main.client = client;
                main.user = user;
                main.Friends = Friends; 
                main.phoneVOIP = phoneVOIP;
                main.listUsers = listUsers;
                main.Left = this.Left;
                main.Top = this.Top;
                this.Close();
                main.Show();
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private List<ListHistory> GetAllHistory(string login)                       // uzyskanie calej hisorii rozmow
        {
            string historyListString = client.sendMessage("ALLHISTORY " + login);
            string[] historySplit = historyListString.Split('&');
            List<ListHistory> historyList = new List<ListHistory>();
            for (int i = 0; i < (historySplit.Length - 1); i++)
            {
                ListHistory history = new ListHistory();
                string[] sp = historySplit[i].Split(' ');
                history.userName = sp[0];
                history.dayBegin = sp[1];
                history.hourBegin = sp[2];
                history.dayEnd = sp[3];
                history.hourEnd = sp[4];
                historyList.Add(history);
            }
            return historyList;
        }

        private void historyTextboxaction(object sender, MouseButtonEventArgs e)
        {
            List<ListHistory> listHistory = GetAllHistory(user.Name);
            History main = new History(user.Name, listHistory);
            App.Current.MainWindow = main;
            main.client = client;
            main.Users = Friends;
            main.Friends = Friends;
            main.listUsers = listUsers;
            main.phoneVOIP = phoneVOIP;
            main.user = user;
            main.Left = this.Left;
            main.Top = this.Top;
            this.Close();
            main.Show();
        }

        private void homeTextboxaction(object sender, MouseButtonEventArgs e)
        {

        }

        private void settingsTextboxaction(object sender, MouseButtonEventArgs e)
        {

            Settings main = new Settings();
            App.Current.MainWindow = main;
            main.Users = Friends;
            main.listUsers = listUsers;
            main.Friends = Friends;
            main.client = client;
            main.user = user;
            main.phoneVOIP = phoneVOIP;
            main.Left = this.Left;
            main.Top = this.Top;
            this.Close();
            main.Show();
        }

        private void logOutTextboxaction(object sender, MouseButtonEventArgs e)
        {

            client.sendMessage("EXIT " + user.Name);
            client.destroyfunction();
            LoginRegisterWindow main = new LoginRegisterWindow();
            main.client = null;
            this.Close();
            main.Show();
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }





    }
}

