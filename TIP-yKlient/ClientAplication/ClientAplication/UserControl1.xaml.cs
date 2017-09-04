using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
        int timeThreadloop = 5000;
        SingletoneObject singletoneOBj;
        int numberSelectedItem = -1;
        public  string SelectedItem = "";
        public Thread refreshListThread;
        public UserControl1()
        {
            try
            {
                singletoneOBj = SingletoneObject.GetInstance;
                InitializeComponent();
                userNameTB.Text = singletoneOBj.user.Name;
                searchInput.Text = singletoneOBj.searchvalue;
                if (singletoneOBj.searchvalue == null)
                {
                        updateFriendList(); 
                }
               
                refreshListThread = new Thread(ListRefreshloop);
                refreshListThread.IsBackground = true;
                refreshListThread.Start();
            }
            catch (Exception e) { }
            InitializeComponent();

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }
        }


        private void UserControl1_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                refreshListThread.Abort();
            }
            catch (Exception exc) { }
        }

        internal void updateFriendList()
        {
            singletoneOBj.Users = new ObservableCollection<User>();
            singletoneOBj.Friends = new ObservableCollection<User>();
            numberSelectedItem = -1;
            int i = 0;
            if (singletoneOBj.listUsers != null)
            {
                foreach (ListUser item in singletoneOBj.listUsers)
                {
                 
                    if (item.active == "True")
                    {
                        singletoneOBj.Users.Add(new User(true,true) { Name = item.name, IcoCall = "\uE0CD",  IcoColor = "green" }); // aktywny
                        singletoneOBj.Friends.Add(new User(true,true) { Name = item.name, IcoCall = "\uE0CD", IcoColor = "green" });
                    }
                    else
                    {
                        singletoneOBj.Users.Add(new User(true,false) { Name = item.name, IcoCall = "\uE0CD",  IcoColor = "gray" }); // nieaktywny
                        singletoneOBj.Friends.Add(new User(true,false) { Name = item.name, IcoCall = "\uE0CD", IcoColor = "gray" });
                    }
                 
                if (item.name == SelectedItem) { numberSelectedItem = i;  }
                    i++;
                }
            }


            lbUsers.DataContext = singletoneOBj.Friends;
            lbUsers.SelectedIndex = numberSelectedItem;


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

        private void updateList()
        {
            if (singletoneOBj.listusercompare == true)
            {
                if (string.IsNullOrWhiteSpace(singletoneOBj.searchvalue))
                {
                    updateFriendList();
                    lbUsers.DataContext = singletoneOBj.Friends;
                    singletoneOBj.listusercompare = false;
                }
                else
                {
                    searchupdate();
                }
            }
            return;
        }

        private void searchupdate()
        {
            singletoneOBj.Users = new ObservableCollection<User>();
            foreach (ListUser item in singletoneOBj.listUserSearch)
            {
                if (singletoneOBj.Friends.Any(x => x.Name == item.name && singletoneOBj.listUsers.Where(y=> y.name==item.name && y.active=="True").Any()  ) )
                    singletoneOBj.Users.Add(new User(true,true) { Name = item.name, IcoCall = "\uE0CD", IcoColor = "green"}); // aktywny przyjaciel
                else  if (singletoneOBj.Friends.Any(x => x.Name == item.name))
                    singletoneOBj.Users.Add(new User(true,false) { Name = item.name, IcoCall = "\uE0CD", IcoColor = "green" }); // nieaktywny przyjaciel
                else
                    singletoneOBj.Users.Add(new User(false,false) { Name = item.name, IcoCall = "\uE7FE;", IcoColor = "DarkOrange" }); // nieznajomy
             
            }
            lbUsers.DataContext = singletoneOBj.Users;
            lbUsers.SelectedIndex = -1;

        }
        private void userAction(object sender, MouseButtonEventArgs e)
        {
         AplicationUser userLogged = singletoneOBj.user;
            TextBlock cmd = (TextBlock)sender;
            if (cmd.DataContext is User)
            {
                User user = (User)cmd.DataContext;
                if (user.isFriend == true && user.isActive==true )
                {
                    MessageBox.Show("tutaj");
                    singletoneOBj.phoneVOIP.nameCallToUser = user.Name;
                    bool call = singletoneOBj.phoneVOIP.btn_PickUp_Click(user.Name);
                    CallToWindow main = new CallToWindow();
                    if (call == true)
                    {
                        main.user = user;
                        main.dateBegin = DateTime.Now;
                        main.Show();
                    }
                    else
                    {
                        MessageBox.Show("problem z ozeki");
                    }
                }
                else if(user.isFriend==false)
                {
                    //wyslanie komunikatu do serwera
                    singletoneOBj.client.sendMessage("ADDFRIEND " + singletoneOBj.user.Name+ " "+ user.Name);
                    //dodanie przyjaciela do listy 
                    MessageBox.Show("Dodano " + user.Name + " do znajomych");
                    //zmiana ikonki przy użytkowniku
                    singletoneOBj.listUserSearch = SearchUsers(singletoneOBj.user.Name, singletoneOBj.searchvalue);
                    ListUser us = new ListUser();
                    us.name = user.Name;
                    if (singletoneOBj.listUserSearch.Any(x=> x.name==user.Name && x.active=="True"))
                    {
                        singletoneOBj.Friends.Add(new User(true,true) { Name = user.Name, IcoCall = "\uf098", IcoUser = "\uf007", IcoColor = "green" }); // nieaktywny przyjaciel
                        us.active = "True";
                        singletoneOBj.listUsers.Add(us);
                    }
                    else
                    {
                        singletoneOBj.Friends.Add(new User(true,false) { Name = user.Name, IcoCall = "\uf098", IcoUser = "\uf2c0", IcoColor = "green" }); // nieaktywny przyjaciel
                        us.active = "False";
                        singletoneOBj.listUsers.Add(us);
                    }

                }
                MessageBox.Show("tutaj2");
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
            if (cmd.DataContext is User )
            {
                User userFriend = (User)cmd.DataContext;
                if (userFriend.isFriend == true)
                {
                    List<ListHistory> listHistory = GetConcreteHistory(singletoneOBj.user.Name, userFriend.Name);
                    singletoneOBj.mainwindow.Width = 750;
                    var page = new UserPage(listHistory, userFriend.Name);
                    singletoneOBj.mainwindow.Content = page;
                }
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string value = searchInput.Text;
            if (value != null && value !="")
            { singletoneOBj.listUserSearch = SearchUsers(singletoneOBj.user.Name, value); searchupdate(); }
            else
            {
                updateFriendList();
            }
            singletoneOBj.searchvalue = value;
            
        }

        private List<ListHistory> GetAllHistory(string login)                       // uzyskanie calej hisorii rozmow
        {
            string historyListString = singletoneOBj.client.sendMessage("ALLHISTORY " + login);
            string[] historySplit = historyListString.Split('&');
            List<ListHistory> historyList = new List<ListHistory>();
            for (int i = 0; i < (historySplit.Length - 1); i++)
            {
                try
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
                catch (Exception e) { MessageBox.Show(historySplit[i]); }
            }
            return historyList;
        }

        private void historyTextboxaction(object sender, MouseButtonEventArgs e)
        {

            
            var page = new Page1(singletoneOBj.user.Name, GetAllHistory(singletoneOBj.user.Name));
            singletoneOBj.mainwindow.Content = page;
           
        }

        private void homeTextboxaction(object sender, MouseButtonEventArgs e)
        {
            singletoneOBj.mainwindow.Width = 300;
            var page = new Page2();
            singletoneOBj.mainwindow.Content = page;

        }

        private void settingsTextboxaction(object sender, MouseButtonEventArgs e)
        {
            singletoneOBj.mainwindow.Width = 750;
            var page = new SettingsPage();
            singletoneOBj.mainwindow.Content = page;
        }

        private void logOutTextboxaction(object sender, MouseButtonEventArgs e)
        {
            singletoneOBj.isOnlineThread.Abort();
            refreshListThread.Abort();
            singletoneOBj.client.sendMessage("EXIT " + singletoneOBj.user.Name);
            singletoneOBj.setdefaultvalue();
            LoginRegisterWindow main = new LoginRegisterWindow();
            Window.GetWindow(this).Close();
            main.Show();
        }
      
        private void ListRefreshloop()
        {
            while (true) { 
            this.Dispatcher.Invoke(() => updateList());
            Thread.Sleep(timeThreadloop);
            }
        }

        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lbUsers.SelectedIndex == -1)
            {
                return;
            }
            MessageBoxResult result = MessageBox.Show("Czy na pewno chcesz usunąć znajomego", "Usunięcie znajomego", MessageBoxButton.YesNo, MessageBoxImage.Question);

            int index = lbUsers.SelectedIndex;
            
            if (result == MessageBoxResult.Yes)
            {
                string historyListString = singletoneOBj.client.sendMessage("DELFRIEND " + singletoneOBj.user.Name + " " 
                + singletoneOBj.listUsers.ElementAt(index).name);

                if (historyListString == "True")
                {
                    singletoneOBj.listusercompare = true;
                    singletoneOBj.listUsers.RemoveAt(index);
                    updateList();
                }
                else
                {
                    MessageBox.Show("Wystąpił problem podczas usuwania znajomgeo");
                }
            }
        }
    }
}
