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
        internal ObservableCollection<User> Users;
        public History()
        {
            InitializeComponent();
            //testowe wypelnienie
           items = new ObservableCollection<itemTB>() {
                //użytkownicy
            new itemTB() {
            Name="Kowalski",
            Describe ="Rozmowa z użytkownikem Kowalski ",
            Describe2="rozpoczęła się dnia 12.03.2017 o gdzoinie 13:00 ",
            Describe3="i zakończyła się dnia 14.03.2017 o godzinie 00:34",
            Date="12.03.2017"
            },
            new itemTB() {
            Name="Makumba",
            Describe ="Rozmowa z użytkownikem Makumba",
            Describe2="rozpoczęła się dnia 12.03.2017 o gdzoinie 13:00 ",
            Describe3="i zakończyła się dnia 14.03.2017 o godzinie 00:34",
            Date="13.03.2017"
            },
            new itemTB() {
            Name="Iksinski",
            Describe ="Rozmowa z użytkownikem Iksinski",
            Describe2="rozpoczęła się dnia 12.03.2017 o gdzoinie 13:00 ",
            Describe3="i zakończyła się dnia 14.03.2017 o godzinie 00:34",
             Date="14.03.2017"
            },
            new itemTB() {
            Name="Walenrod",
            Describe ="Rozmowa z użytkownikem Walenrod",
            Describe2="rozpoczęła się dnia 12.03.2017 o gdzoinie 13:00 ",
            Describe3="i zakończyła się dnia 14.03.2017 o godzinie 00:34",
            Date="15.03.2017"
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



        private void goToUser(object sender, MouseButtonEventArgs e)
        {
            TextBlock cmd = (TextBlock)sender;
            if (cmd.DataContext is User)
            {
                User user = (User)cmd.DataContext;
                UserWindow main = new UserWindow();
                main.Users = Users;
                App.Current.MainWindow = main;
                main.user = user;
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
