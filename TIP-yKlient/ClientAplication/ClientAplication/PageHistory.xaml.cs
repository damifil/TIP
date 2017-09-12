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
    /// klasa Strony zawierająca logię do widoku historia
    /// </summary>
    public partial class PageHistory : Page
    {
        /// <summary>
        /// Obiekt <c>ObservableCollection</c> zawietajacy kolekcje opisów dotyczących historii rozmów
        /// </summary>
        internal ObservableCollection<ItemTB> items { get; set; }
        /// <summary>
        /// kopia obiektu items
        /// </summary>
        internal ObservableCollection<ItemTB> copyitems { get; set; }
        /// <summary>
        /// obiekt klasy <c>SingletoneObject</c>
        /// </summary>
        SingletoneObject singletoneOBj=SingletoneObject.GetInstance;

        /// <summary>
        /// Konstruktor klasy
        /// </summary>
        public PageHistory()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Konstruktor klasy, inicjuję listę historii oraz ukrywa przycisk historia na stronie mainPage
        /// </summary>
        /// <param name="login"> login użytkownika </param>
        /// <param name="listHistory">lista zawierajaca elementy histori rozmów użytkownika</param>
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

        /// <summary>
        /// Akcja do przycisku cofnij
        /// </summary>
        /// <remarks>
        /// zmniejsza rozmiar okna MainWindow do rozmiaru strony pageMain, oraz ustawia jako zawartość stronę pageMain
        /// </remarks>
        private void backAction(object sender, MouseButtonEventArgs e)
        {
            singletoneOBj.mainwindow.Width = 300;
            var page = new PageMain();
            singletoneOBj.mainwindow.Content = page;
        }

        /// <summary>
        /// Funkcja odpowiedzialna za uzyskanie historii konkretnego użytkownika
        /// </summary>
        /// <param name="login">Login użytkownika aplikacji</param>
        /// <param name="login1">login poszukiwango użytkownika</param>
        /// <returns>zwraca liste zwierająćą historię rozmów z danym użytkownikiem</returns>
        private List<ListHistory> GetConcreteHistory(string login, string login1)               
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



        /// <summary>
        /// Funkcja odpowiedzialna za dzwonienie do danego użytkownika z listy
        /// </summary>
        /// <remarks>
        /// funkcja ta otwiera nowe okno <c>CallToWindow</c>
        /// </remarks>
        /// <param name="sender">obiekt z któego zczytywana jest nazwa uzytkownika do któerego wykonywana jest
        /// akcja dzwonienia</param>
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


        /// <summary>
        /// Funkcja odpowiedzialna za akcje podczas wpisywania wartośći do pola wyszukiwania historii
        /// </summary>
        /// <remarks>
        /// dynamicznie zostawia tylko te wartośći pól historii któe zawierają dany ciąg znaków, dla wartośći pustej wyświetla całą historię
        /// </remarks>
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
