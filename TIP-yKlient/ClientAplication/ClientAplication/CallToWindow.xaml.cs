using System;
using System.Collections.Generic;
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
    /// Klasa <c>CallToWindow</c> przechowywująca logikę okna które wyświetla się w momencie
    /// kiedy ktoś próbuje się dodzwonić do użytkownika
    /// </summary>
    public partial class CallToWindow : Window
    {
        /// <summary>
        /// Obiekt klasy <c>SingletoneObject</c> przechowywująćy informacje któe w całym programie są potrzebne
        /// </summary>
        SingletoneObject singletoneOBJ = SingletoneObject.GetInstance;

        /// <summary>
        /// Obiekt klasy <c>User</c> którzy przechowuje informacje na temat osoby
        /// do której wykonuje połączenie użytkownik
        /// </summary>
        internal User user { get; set; }

        /// <summary>
        /// zmienna przechowywujaca czas rozpoczecia dzwonienia
        /// wykorzystywany podczas wysylania do serwera informacji o czasie wykonywania połązenia
        /// które zostało odrzucone
        /// </summary>
        internal DateTime dateBegin { get; set; }

        /// <summary>
        /// Konstruktor klasy
        /// </summary>
        public CallToWindow()
        {
            InitializeComponent();
        }


        bool _shown;
        /// <summary>
        /// nadpisana metoda <c>OnContentRendered</c> uruchaiająca się podczas wygenerowania wyglądu podaje tekst z kim nawiązywuje się rozmowę.
        /// </summary>
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_shown)
                return;
            singletoneOBJ.phoneVOIP.callto = this;
            stringWithName.Text = "Nawiązywanie połączenia z " + user.Name;
            _shown = true;
            
        }

        /// <summary>
        /// Metoda uruchamiająća się podczas naciśnięcia przycisku czerwonej słuchawki, 
        /// podczas któej wysyłane są wiadomości do serwera oraz do serwera OZEKi o odrzuceniu rozmowy
        /// </summary>
        private void callDisconectTextboxaction(object sender, MouseButtonEventArgs e)
        {
            string searchList = singletoneOBJ.client.sendMessage("SAVECALL " + singletoneOBJ.user.Name + " " +  user.Name + " " + dateBegin.ToString() + " " + DateTime.Now.ToString());
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
    }
}
