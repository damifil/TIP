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
        SingletoneObject singletoneOBj;


        public History(string login, List<ListHistory> listHistory)
        {
            items = new ObservableCollection<itemTB>();
            singletoneOBj = SingletoneObject.GetInstance;

            singletoneOBj.isOnlineThread.Abort();
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
            singletoneOBj.isOnlineThread.Abort();
        }

        bool _shown;

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_shown)
                return;

            _shown = true;
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
                bool call = singletoneOBj.phoneVOIP.btn_PickUp_Click(user.Name);

                CallToWindow main = new CallToWindow();
                if (call == true)
                {
                    main.user = user;
                    main.phoneVOIP = singletoneOBj.phoneVOIP;
                    main.Show();
                }

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
