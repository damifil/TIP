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
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        internal User user;
        internal Client client;
        internal ObservableCollection<User> Users;
        internal ObservableCollection<User> Friends;
        internal List<ListUser> listUsers;
        internal PhoneVOIP phoneVOIP;
        internal ObservableCollection<itemTB> items;   //  historia rozmow z uzytkownikiem
        

        public UserWindow(List<ListHistory> listHistory)
        {
            InitializeComponent();
            stringInUserWindow.Text = "Historia ostatnich połączeń z użytkownikem Nazwa uzytkownika";

            items = new ObservableCollection<itemTB>();
            foreach (ListHistory item in listHistory)
            {
                items.Add(new itemTB()
                {
                    Name = item.userName,
                    Describe = "Rozmowa z użytkownikiem " + item.userName + " o godzinie " + item.hourBegin,
                    Describe2 = "rozpoczęła się dnia " + item.dayBegin,
                    Describe3 = "i zakonczyła się dnia " + item.dayEnd + " o godzinie " + item.hourEnd,
                    Date = "dzinen"
                });
            }

            lbHistoryAll.DataContext = items;

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

        private List<ListHistory> GetAllHistory(string login)                       // uzyskanie calej hisorii rozmow
        {
            Console.WriteLine("GetALL in userwindow: " + login);
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
            main.phoneVOIP = phoneVOIP;
            main.user = user;
            main.Friends = Friends;
            main.listUsers = listUsers;
            main.Left = this.Left;
            main.Top = this.Top;
            this.Close();
            main.Show();
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
                main.listUsers = listUsers;
                main.phoneVOIP = phoneVOIP;
                main.Left = this.Left;
                main.Top = this.Top;
                this.Close();
                main.Show();
            }
        }



        private void homeTextboxaction(object sender, MouseButtonEventArgs e)
        {

            MainWindow main = new MainWindow();
            App.Current.MainWindow = main;
            main.Users = Friends;
            main.listUsers = listUsers;
            main.client = client;
            main.phoneVOIP = phoneVOIP;
            main.user = user;
            main.Friends = Friends;
            main.Left = this.Left;
            main.Top = this.Top;
            this.Close();
            main.Show();
        }

        private void settingsTextboxaction(object sender, MouseButtonEventArgs e)
        {

            Settings main = new Settings();
            App.Current.MainWindow = main;
            main.Users = Friends;
            main.phoneVOIP = phoneVOIP;
            main.user = user;
            main.listUsers = listUsers;
            main.Friends = Friends;
            main.client = client;
            main.Left = this.Left;
            main.Top = this.Top;
            this.Close();
            main.Show();
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
                if (!Users.Where(x => x.Name == item.name).Any())
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

        private void callToFriend(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Dzwonimy do " + user.Name);
        }
        private void deleteFriend(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("usuwamy " + user.Name);
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


        private void searchClickHistoryuser(object sender, RoutedEventArgs e)
        {
            string value = userhistorysearchinput.Text;
            for (int i = 0; i < items.Count; i++)
            {
                if (value != items[i].Date)
                {
                    items.RemoveAt(i);
                    i--;
                }
            }

        }


    }
}
