using HashLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
using System.Windows.Shapes;

namespace ClientAplication
{
    /// <summary>
    /// klasa  przechowująca strony logowania oraz rejestracji i ustawień
    /// </summary>
    public partial class LoginRegisterWindow : Window
    {
       
        /// <summary>
        /// Konstruktor klasy
        /// przypisuje strone <c>PageLogin</c> do zawartośći okn
        /// </summary>
        public LoginRegisterWindow()
        {
            InitializeComponent();
            var page = new PageLogin();
            this.Content = page;
        }

        /// <summary>
        /// metoda odpowiedzialna za przcycisk zamykania 
        /// zamyka całą aplikację
        /// </summary>
        private void closeButon_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// nadpisana metoda <c>OnMOuseLeftButtonDown</c> umożliwająca przesuwanie okna po całym 
        /// ekranie za pomocą naciśnięcia myszy na pole okna w którym nie znajduje się żaden inny element
        /// taki jak textblock itp.
        /// </summary>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
    }
}
