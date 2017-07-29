using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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


   
    public partial class UserControl1 : UserControl
    {
        SingletoneObject singletoneOBj;

        public UserControl1()
        {
            singletoneOBj =  SingletoneObject.GetInstance;
            InitializeComponent();
            userNameTB.Text = singletoneOBj.user.Name;
            if (singletoneOBj.Users != null)
            { lbUsers.DataContext = singletoneOBj.Users; }
            else
            { addUSerToList(); }
        }

        internal void addUSerToList()
        {
            singletoneOBj.Users = new ObservableCollection<User>();
            singletoneOBj.Friends = new ObservableCollection<User>();
            if (singletoneOBj.listUsers != null)
            {
                foreach (ListUser item in singletoneOBj.listUsers)
                {
                    if (item.active == "True")
                    {
                        singletoneOBj.Users.Add(new User() { Name = item.name, IcoCall = "\uf098", IcoUser = "\uf007" }); // aktywny
                        singletoneOBj.Friends.Add(new User() { Name = item.name, IcoCall = "\uf098", IcoUser = "\uf007" });
                    }
                    else
                    {
                        singletoneOBj.Users.Add(new User() { Name = item.name, IcoCall = "\uf098", IcoUser = "\uf2c0" }); // nieaktywny
                        singletoneOBj.Friends.Add(new User() { Name = item.name, IcoCall = "\uf098", IcoUser = "\uf2c0" });
                    }
                }
            }
            lbUsers.DataContext = singletoneOBj.Friends;
        }

        private List<ListUser> SearchUsers(string login, string login1)              // szukanie uzytkownikow
        {
            string searchList = singletoneOBj.client.sendMessage("SRCH " + login + " " + login1);
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
                lbUsers.DataContext = singletoneOBj.Friends;
                return;
            }
            List<ListUser> listUser = SearchUsers(singletoneOBj.user.Name, value);

            singletoneOBj.Users = new ObservableCollection<User>(singletoneOBj.Friends);
            foreach (ListUser item in listUser)
            {
                if (!singletoneOBj.Users.Where(x => x.Name == item.name).Any())
                    if (item.active == "True")
                    {

                        singletoneOBj.Users.Add(new User() { Name = item.name, IcoCall = "\uf098", IcoUser = "\uf234" }); // aktywny
                    }
                    else
                    {
                        singletoneOBj.Users.Add(new User() { Name = item.name, IcoCall = "\uf098", IcoUser = "\uf234" }); // nieaktywny
                    }
            }
            lbUsers.DataContext = singletoneOBj.Users;
        }

        private void callToUser(object sender, MouseButtonEventArgs e)
        {
            User userLogged = singletoneOBj.user;
            TextBlock cmd = (TextBlock)sender;
            if (cmd.DataContext is User)
            {

                User user = (User)cmd.DataContext;
                singletoneOBj.phoneVOIP.nameCallToUser = user.Name;
                bool call = singletoneOBj.phoneVOIP.btn_PickUp_Click(user.Name);
                CallToWindow main = new CallToWindow();
                if (call == true)
                {
                    main.user = user;
                    main.userLogged = userLogged;
                    main.client = singletoneOBj.client;
                    main.dateBegin = DateTime.Now;
                    main.phoneVOIP = singletoneOBj.phoneVOIP;
                    main.Show();
                }

            }
        }

        private List<ListHistory> GetConcreteHistory(string login, string login1)               // uzyskanie hisorii rozmow od konkretnego uzytkownika
        {
            string historyListString = singletoneOBj.client.sendMessage("USERHISTORY " + login + " " + login1);
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
                List<ListHistory> listHistory = GetConcreteHistory(singletoneOBj.user.Name, userFriend.Name);
                UserWindow main = new UserWindow(listHistory);
                App.Current.MainWindow = main;
                main.Left = Window.GetWindow(this).Left;
                main.Top = Window.GetWindow(this).Top;
                Window.GetWindow(this).Close();
                main.Show();
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private List<ListHistory> GetAllHistory(string login)                       // uzyskanie calej hisorii rozmow
        {
            string historyListString = singletoneOBj.client.sendMessage("ALLHISTORY " + login);
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
            List<ListHistory> listHistory = GetAllHistory(singletoneOBj.user.Name);
            History main = new History(singletoneOBj.user.Name, listHistory);
            App.Current.MainWindow = main;
            main.Left = Window.GetWindow(this).Left;
            main.Top = Window.GetWindow(this).Top;
            Window.GetWindow(this).Close();
            main.Show();
        }

        private void homeTextboxaction(object sender, MouseButtonEventArgs e)
        {
            MainWindow main = new MainWindow();
            App.Current.MainWindow = main;
            main.Left = Window.GetWindow(this).Left;
            main.Top = Window.GetWindow(this).Top;
            Window.GetWindow(this).Close();
            main.Show();
        }

        private void settingsTextboxaction(object sender, MouseButtonEventArgs e)
        {

            Settings main = new Settings();
            App.Current.MainWindow = main;
            main.Left = Window.GetWindow(this).Left;
            main.Top = Window.GetWindow(this).Top;
            Window.GetWindow(this).Close();
            main.Show();
        }

        private void logOutTextboxaction(object sender, MouseButtonEventArgs e)
        {
            singletoneOBj.client.sendMessage("EXIT " + singletoneOBj.user.Name);
            singletoneOBj.client.destroyfunction();
            LoginRegisterWindow main = new LoginRegisterWindow();
            main.client = null;
            Window.GetWindow(this).Close();
            main.Show();
        }
      
    }




}
