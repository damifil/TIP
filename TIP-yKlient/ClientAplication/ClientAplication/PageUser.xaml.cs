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
        internal ObservableCollection<ItemTB> items { get; set; }  //  historia rozmow z uzytkownikiem
        internal ObservableCollection<ItemTB> copyitems { get; set; }
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

            items = new ObservableCollection<ItemTB>();
            copyitems = new ObservableCollection<ItemTB>();
           if(singletoneOBJ.listUsers.Find(X=> X.name == userFriend).active == "True")
            {
               
                callToFriendIco.Foreground = Brushes.Green;
            }
           
            foreach (ListHistory item in listHistory)
            {
                items.Add(new ItemTB()
                {
                    Name = item.userName,
                    Describe = "Rozmowa z użytkownikiem " + item.userName + " o godzinie " + item.hourBegin
                    +" rozpoczęła się dnia " + item.dayBegin
                    +" i zakończyła się dnia " + item.dayEnd + " o godzinie " + item.hourEnd,
                });

                copyitems.Add(new ItemTB()
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
            if (callToFriendIco.Foreground ==  Brushes.Green)
            {
                singletoneOBJ.phoneVOIP.nameCallToUser = userFriend;
                bool call = singletoneOBJ.phoneVOIP.btn_PickUp_Click(userFriend);
                CallToWindow main = new CallToWindow();
                if (call == true)
                {
                    User a = new User(true, true);
                    a.Name = userFriend;
                    main.user = a;
                    main.dateBegin = DateTime.Now;
                    main.Show();
                }
                else
                {
                    MessageBox.Show("problem z ozeki");
                }
            }
        }
        private void deleteFriend(object sender, RoutedEventArgs e)
        {
            string historyListString = singletoneOBJ.client.sendMessage("DELFRIEND " + singletoneOBJ.user.Name + " " + userFriend);

            if (historyListString == "OK")
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
                items = new ObservableCollection<ItemTB>(copyitems);
                lbHistoryAll.DataContext = items;
            }



        }
        private void backAction(object sender, MouseButtonEventArgs e)
        {
            singletoneOBJ.mainwindow.Width = 300;
            var page = new PageMain();
            singletoneOBJ.mainwindow.Content = page;
        }
    }
}
