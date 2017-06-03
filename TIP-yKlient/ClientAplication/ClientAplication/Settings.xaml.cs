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
using System.Windows.Shapes;

namespace ClientAplication
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        internal ObservableCollection<itemTB> items;
        internal User user;
        internal ObservableCollection<User> Users;
        internal List<ListUser> listUsers;
        internal PhoneVOIP phoneVOIP;
        internal Client client; public Settings()
        {
            InitializeComponent();
        }

        bool _shown;
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_shown)
                return;

            _shown = true;
            lbUsers.DataContext = Users;
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
                main.phoneVOIP = phoneVOIP;
                main.user = user;
                main.listUsers = listUsers;
                main.Left = this.Left;
                main.Top = this.Top;
                this.Close();
                main.Show();
            }
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
            main.Users = Users;
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

            MainWindow main = new MainWindow();
            App.Current.MainWindow = main;
            main.Users = Users;
            main.listUsers = listUsers;
            main.client = client;
            main.user = user;
            main.phoneVOIP = phoneVOIP;
            main.Left = this.Left;
            main.Top = this.Top;
            this.Close();
            main.Show();
        }

        private void settingsTextboxaction(object sender, MouseButtonEventArgs e)
        {

            
        }


        private void searchClick(object sender, RoutedEventArgs e)
        {
            string value = searchInput.Text;
            Users = new ObservableCollection<User>() {
                //uzytkownicy
            new User() { Name = value ,IcoCall="\uf098", IcoUser="\uf2c0"},

            };
            lbUsers.DataContext = Users;
        }


        private void callToUser(object sender, MouseButtonEventArgs e)
        {
            TextBlock cmd = (TextBlock)sender;
            if (cmd.DataContext is User)
            {
                User user = (User)cmd.DataContext;
                bool call = phoneVOIP.btn_PickUp_Click(user.Name);

                CallToWindow main = new CallToWindow();
                if (call == true)
                {
                    main.user = user;
                    main.phoneVOIP = phoneVOIP;
                    main.Show();
                }

            }
        }

    }
}
