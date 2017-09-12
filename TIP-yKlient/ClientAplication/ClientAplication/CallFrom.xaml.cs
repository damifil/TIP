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
    /// Klasa <c>CallFromWindow</c> przechowywująca logikę okna wyskakującego
    /// podczas nawiazywania połączenia 
    /// </summary>
    public partial class CallFromWindow : Window
    {


        /// <summary>
        /// Obiekt klasy <c>SingletoneObject</c> przechowywująćy informacje które w całym programie są potrzebne
        /// </summary>
        SingletoneObject singletoneOBJ = SingletoneObject.GetInstance;


        /// <summary>
        /// zmienna <c>user</c> przechowywujaca dane użytkownika który dzwoni do użytkownika
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
        public CallFromWindow()
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

            stringWithName.Text = "Połączenie " + user.Name;
            _shown = true;

        }


        /// <summary>
        /// Metoda uruchamiająća się podczas naciśnięcia przycisku czerwonej słuchawki, 
        /// podczas któej wysyłane są wiadomości do serwera oraz do serwera OZEKi o odrzuceniu rozmowy
        /// </summary>
        private void callDisconectTextboxaction(object sender, MouseButtonEventArgs e)
        {
            string saveCall = singletoneOBJ.client.sendMessage("SAVECALL " + singletoneOBJ.user.Name + " " + user.Name + " " + dateBegin + " " + DateTime.Now.ToString());
            singletoneOBJ.phoneVOIP.btn_HangUp_Click(user.Name);
            this.Close();
        }

        /// <summary>
        /// Metoda uruchaiająca sie podczas naciśnięcia zelonego przycisku słuchawki,
        /// podczas któej wysyłana jest wiadomość do serwera ozeki o zaakceptowaniu rozmowy 
        /// oraz zamykane jest okno z nawiazywaniem rozmowy
        /// </summary>
        private void callAceptTextboxaction(object sender, MouseButtonEventArgs e)
        {
            singletoneOBJ.phoneVOIP.btn_PickUp_Click(user.Name);
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
