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

        public MainWindow()
        {
            InitializeComponent();
            history.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.historyTextboxaction), true);

            //inicjalizacja odpowiedzialna za wyswietlanie znajomych
            Users = new ObservableCollection<User>() {
                //użytkownicy
            new User() { Name = "ofline" ,IcoCall="\uf098", IcoUser="\uf2c0"},
            new User() { Name = "do dodania", IcoCall="\uf098" ,IcoUser="\uf234"},
            new User() { Name = "online", IcoCall="\uf098" ,IcoUser="\uf007"}
            };

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
                        MessageBox.Show(user.Name);
                    }
                }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void historyTextboxaction(object sender, MouseButtonEventArgs e)  
        {
            MessageBox.Show("dziala");
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