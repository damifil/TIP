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
    

    public partial class UserPage : Page
    {
        internal ObservableCollection<itemTB> items;   //  historia rozmow z uzytkownikiem
        internal ObservableCollection<itemTB> copyitems;
        SingletoneObject singletoneOBJ = SingletoneObject.GetInstance;
        private string userFriend;


        public UserPage()
        {
            InitializeComponent();
        }
        public UserPage(List<ListHistory> listHistory, string userFriend)
        {

            InitializeComponent();
            userControlLeftSide.SelectedItem = userFriend;
            userControlLeftSide.updateFriendList();
            this.userFriend = userFriend;
            stringInUserWindow.Text =  userFriend;

            items = new ObservableCollection<itemTB>();
            copyitems = new ObservableCollection<itemTB>();

            foreach (ListHistory item in listHistory)
            {
                items.Add(new itemTB()
                {
                    Name = item.userName,
                    Describe = "Rozmowa z użytkownikiem " + item.userName + " o godzinie " + item.hourBegin
                    +" rozpoczęła się dnia " + item.dayBegin
                    +" i zakończyła się dnia " + item.dayEnd + " o godzinie " + item.hourEnd,
                });

                copyitems.Add(new itemTB()
                {
                    Name = item.userName,
                    Describe = "Rozmowa z użytkownikiem " + item.userName + " o godzinie " + item.hourBegin
                    +" rozpoczęła się dnia " + item.dayBegin
                    +" i zakończyła się dnia " + item.dayEnd + " o godzinie " + item.hourEnd,
                });
            }
            lbHistoryAll.DataContext = items;
        }
        private void callToFriend(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Dzwonimy do " + singletoneOBJ.user.Name);
        }
        private void deleteFriend(object sender, RoutedEventArgs e)
        {
            string historyListString = singletoneOBJ.client.sendMessage("DELFRIEND " + singletoneOBJ.user.Name + " " + userFriend);

            if (historyListString == "True")
            {
                var itemToRemove = singletoneOBJ.listUsers.Single(r => r.name == userFriend);
                singletoneOBJ.listUsers.Remove(itemToRemove);
                try
                {
                    itemToRemove = singletoneOBJ.listUserSearch.Single(r => r.name == userFriend);

                    singletoneOBJ.listUserSearch.Remove(itemToRemove);
                }
                catch (Exception ex) { }
                MessageBox.Show("Użytkownik " + userFriend + " został usunięty");
                MainWindow main = new MainWindow();
                App.Current.MainWindow = main;
                main.Left = Window.GetWindow(this).Left;
                main.Top = Window.GetWindow(this).Top;
                Window.GetWindow(this).Close();
                main.Show();

            }
            else
            {
                MessageBox.Show("Użytkownik " + userFriend + " nie został usunięty");
            }

        }
        private void searchClickHistoryuser(object sender, RoutedEventArgs e)
        {


        }

        private void historysearchinput_TextChanged(object sender, TextChangedEventArgs e)
        {
            string value = userhistorysearchinput.Text;

            if (value != "")
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (!items[i].Describe.Contains(value) )
                    {
                        items.RemoveAt(i);
                        i--;
                    }
                }
            }
            else
            {
                items = new ObservableCollection<itemTB>(copyitems);
                lbHistoryAll.DataContext = items;
            }



        }
        private void backAction(object sender, MouseButtonEventArgs e)
        {
            singletoneOBJ.mainwindow.Width = 300;
            var page = new Page2();
            singletoneOBJ.mainwindow.Content = page;
        }
    }
}
