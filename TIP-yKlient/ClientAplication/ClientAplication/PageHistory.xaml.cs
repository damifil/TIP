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
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class PageHistory : Page
    {
        internal ObservableCollection<ItemTB> items { get; set; }
        internal ObservableCollection<ItemTB> copyitems { get; set; }
        SingletoneObject singletoneOBj=SingletoneObject.GetInstance;

        public PageHistory()
        {
            InitializeComponent();
        }

        public PageHistory(string login, List<ListHistory> listHistory)
        {

            InitializeComponent();
           
            items = new ObservableCollection<ItemTB>();
            copyitems = new ObservableCollection<ItemTB>();
            singletoneOBj = SingletoneObject.GetInstance;
            singletoneOBj.mainwindow.Width = 750;
            foreach (ListHistory item in listHistory)
            {
           

                items.Add(new ItemTB()
                {
                    Name = item.userName,
                    Describe = "Rozmowa z użytkownikiem " + item.userName + " o godzinie " + item.hourBegin 
                    + " rozpoczęła się dnia " + item.dayBegin 
                    + " i zakończyła się o godzinie " + item.hourEnd + " w dniu " + item.dayEnd,
                });


                copyitems.Add(new ItemTB()
                {
                    Name = item.userName,
                    Describe = "Rozmowa z użytkownikiem " + item.userName + " o godzinie " + item.hourBegin+
                    " rozpoczęła się dnia " + item.dayBegin+
                    " i zakończyła się o godzinie " + item.hourEnd + " w dniu " + item.dayEnd,
                });
            }

            InitializeComponent();
            lbHistoryAll.DataContext = items;
            UC.history.Visibility = System.Windows.Visibility.Hidden;

        }

        private void backAction(object sender, MouseButtonEventArgs e)
        {
            singletoneOBj.mainwindow.Width = 300;
            var page = new PageMain();
            singletoneOBj.mainwindow.Content = page;
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

        private void searchClickHistory(object sender, RoutedEventArgs e)
        {
        }


        private void callToUser(object sender, MouseButtonEventArgs e)
        {
            TextBlock cmd = (TextBlock)sender;
            if (cmd.DataContext is User)
            {
                User user = (User)cmd.DataContext;
                bool call = singletoneOBj.phoneVOIP.btn_PickUp_Click(user.Name);

                CallToWindow main = new CallToWindow();
                if (call == true)
                {
                    main.user = user;
                    main.Show();
                }

            }
        }

        private void historysearchinput_TextChanged(object sender, TextChangedEventArgs e)
        {
            string value = historysearchinput.Text;
            if (value != "" )
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

    }
}
