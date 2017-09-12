using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ClientAplication
{


    /// <summary>
    /// Klasa <c>CallTransmisionWindow</c> przechowywująća logikę okna któe wyświetla
    /// się w momkencie kiedy trwa rozmowa z użytkownikiem
    /// </summary>
    public partial class CallTransmisionWindow : Window
    {
        /// <summary>
        /// Obiekt klasy <c>User</c> którzy przechowuje informacje na temat osoby
        /// z którą prowadzona jest rozmowa
        /// </summary>
        internal User user { get; set; }
        /// <summary>
        /// zmienna przechowywujaca czas rozpoczecia rozmowy
        /// wykorzystywany podczas wysylania do serwera informacji o czasie wykonywania rozmowy
        /// </summary>
        internal DateTime dateBegin { get; set; }


        /// <summary>
        /// Obiekt klasy <c>DispatcherTimer</c> wykorzystywany do wyświetlania czasu rozmowy w oknie
        /// </summary>
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
       
        /// <summary>
        /// Konstruktor klasy
        /// inicjuje Event dla obiektu dispatcherTime 
        /// </summary>
        public CallTransmisionWindow()
        {
            InitializeComponent();
           
            Application.Current.MainWindow.Closing += new CancelEventHandler(Window_Closing);
           
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            
        }

        bool _shown;
        /// <summary>
        /// nadpisana metoda <c>OnContentRendered</c> uruchaiająca się podczas 
        /// wygenerowania wyglądu podaje tekst z kim trwa rozmowa.
        /// </summary>
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_shown)
                return;


            stringWithName.Text = "Połączenie z " + user.Name;
            _shown = true;

        }

        /// <summary>
        /// metoda wywołująca się podczas zamykania okna
        /// zatrzymuje odliczanie czasu dla obiektu dispatchetTimer
        /// </summary>
        void Window_Closing(object sender, CancelEventArgs e)
        {
            dispatcherTimer.Stop();
        }

        
        /// <summary>
        /// metoda wywołująca się podczas naciśnięcia czerwonej słuchawki
        /// wysyła informacje do serwerów o zakończeniu rozmowy
        /// </summary>
        private void callDisconectTextboxaction(object sender, MouseButtonEventArgs e)
        {
            SingletoneObject singletoneOBJ = SingletoneObject.GetInstance;
            string saveCall = singletoneOBJ.client.sendMessage("SAVECALL " + singletoneOBJ.user.Name + " " + user.Name + " " + dateBegin.ToString() + " " + DateTime.Now.ToString());
            singletoneOBJ.phoneVOIP.btn_HangUp_Click(user.Name);
            this.Close();
        }

        /// <summary>
        /// nadpisana metoda <c>OnMOuseLeftButtonDown</c> umożliwająca przesuwanie okna po całym 
        /// ekranie za pomocą naciśnięcia myszy na pole okna w którym nie znajduje się żaden inny element
        /// taki jak textblock itp.
        /// </summary>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            try
            {
                base.OnMouseLeftButtonDown(e);
                this.DragMove();
            }
            catch (Exception exc) { }
        }
        /// <summary>
        /// ?????
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }


        /// <summary>
        /// metoda wywołująca się podczas zdarzenia ustalonego w konstruktorze
        /// odpowiedzialna jest za wyświetlanie czasu rozmowy w oknie 
        /// </summary>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan t = DateTime.Now - dateBegin;
            int hour = (int)t.TotalHours;
            int minute = (int)t.TotalMinutes % 60;
            int second = (int)t.Seconds % 60;
            timeTextBlock.Text = "Czas rozmowy: " +hour.ToString("00") +":"+minute.ToString("00") + ":" + second.ToString("00");
        }
    }
}
