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
    /// Interaction logic for History.xaml
    /// </summary>
    public partial class History : Window
    {
        internal ObservableCollection<itemTB> items;
        internal User user;
        internal ObservableCollection<User> Users;
        internal List<ListUser> listUsers;
        internal Client client;

      
        public History(string login, List<ListHistory> listHistory)
        {
            items = new ObservableCollection<itemTB>();
            foreach (ListHistory item in listHistory)
            {
                items.Add(new itemTB() { Name = item.userName,
                    Describe = "Rozmowa z użytkownikiem " + item.userName + " o godzinie " + item.hourBegin,
                    Describe2 = "rozpoczęła się dnia " + item.dayBegin,
                    Describe3 = "i zakonczyła się dnia " + item.dayEnd + " o godzinie " + item.hourEnd,
                    Date = "dzinen"
                });
            }
    
            InitializeComponent();
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
                main.listUsers = listUsers;
                main.Left = this.Left;
                main.Top = this.Top;
                this.Close();
                main.Show();
            }
        }


        private void historyTextboxaction(object sender, MouseButtonEventArgs e)
        {


        }

        private void homeTextboxaction(object sender, MouseButtonEventArgs e)
        {

            MainWindow main = new MainWindow();
            App.Current.MainWindow = main;
            main.Users = Users;
            main.listUsers = listUsers;
            main.client = client;
            main.user = user;
            main.Left = this.Left;
            main.Top = this.Top;
            this.Close();
            main.Show();
        }

        private void settingsTextboxaction(object sender, MouseButtonEventArgs e)
        {

            Settings main = new Settings();
            App.Current.MainWindow = main;
            main.Users = Users;
            main.listUsers = listUsers;
            main.client = client;
            main.Left = this.Left;
            main.Top = this.Top;
            this.Close();
            main.Show();
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
        private void searchClickHistory(object sender, RoutedEventArgs e)
        {
            string value = historysearchinput.Text;
            for (int i = 0; i < items.Count; i++)
            {
                if (value != items[i].Date)
                {
                    items.RemoveAt(i);
                    i--;
                }
            }

        }



        private void callToUser(object sender, MouseButtonEventArgs e)
        {
            TextBlock cmd = (TextBlock)sender;
            if (cmd.DataContext is User)
            {
                User user = (User)cmd.DataContext;
                MessageBox.Show(user.Name);
            }
            if (cmd.DataContext is itemTB)
            {
                itemTB item = (itemTB)cmd.DataContext;
                MessageBox.Show(item.Name);
            }

        }



    }


    class itemTB
    {
        public string Describe { get; set; }
        public string Describe2 { get; set; }
        public string Describe3 { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
    }
}
