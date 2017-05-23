using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
 
    
    public partial class MainWindow : Window
    {
       



        // internal ObservableCollection<User> Users { get; private set; }
        internal ObservableCollection<User> Users;
        internal string userName;
        internal Client client;
        public MainWindow()
        {
            InitializeComponent();

            //inicjalizacja odpowiedzialna za wyswietlanie znajomych
            lastActivity.Text = "twoja ostatnia aktywność: !2 kwietnia o godzinie 14:30";
            welcomeString.Text = "Witaj nazwa_Użytkownika";
        }


        bool _shown;
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_shown)
                return;

            userNameTB.Text = userName;
            _shown = true;
            if (Users != null)
            { lbUsers.DataContext = Users; }
            else
            { addUSerToList(); }
        }

        internal void addUSerToList()
        {
            Users = new ObservableCollection<User>() {
                //użytkownicy
            new User() { Name = "ofline" ,IcoCall="\uf098", IcoUser="\uf2c0"},
            new User() { Name = "do dodania", IcoCall="\uf098" ,IcoUser="\uf234"},
            new User() { Name = "online", IcoCall="\uf098" ,IcoUser="\uf007"}
            };
            Users.Add(new User() { Name = "testowanie dodawania", IcoCall = "\uf098", IcoUser = "\uf007" });
            lbUsers.DataContext = Users;
        }
        //funkcja odpowiedzialna za wyswietlanie wyszukanycyh osob
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
                       CallToWindow main = new CallToWindow();
                        main.user = user;
                        main.Show();
            }
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
       


        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

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

        private void logOutTextboxaction(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("przedfunkcja2");

            client.sendMessage("EXIT " + userName);
            Console.WriteLine("przedfunkcja");
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

class User
{
    public string Name { get; set; }
    public string IcoUser { get; set; }
    public string IcoCall { get; set; }
}