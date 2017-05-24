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
        internal ObservableCollection<User> Users;
        internal ObservableCollection<itemTB> items;

        
        public UserWindow()
        {
            InitializeComponent();
            stringInUserWindow.Text = "Historia ostatnich połączeń z użytkownikem Nazwa uzytkownika";


            //wstepne testowanie wpisywania do listboxa
            items = new ObservableCollection<itemTB>() {
                //użytkownicy
            new itemTB() {
            Name="Kowalski",
            Describe ="Rozmowa",
            Describe2="rozpoczęła się dnia 12.03.2017 o gdzoinie 13:00 ",
            Describe3="i zakończyła się dnia 14.03.2017 o godzinie 00:34",
            Date="12.03.2017"
            }
            

            };

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

        private void historyTextboxaction(object sender, MouseButtonEventArgs e)
        {

            History main = new History();
            App.Current.MainWindow = main;
            main.Users = Users;
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
                MessageBox.Show(user.Name);
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
