using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// klasa przechowująca strony histori, ustawień,znajomego oraz główną z listą znajomych
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Konstruktor klasy przypisujący na wstępie do zawartośći okna stronę głóówną zawierajaća listę znajomych.
        /// </summary>
        public MainWindow()
        {
            SingletoneObject singletoneOBj;
            singletoneOBj = SingletoneObject.GetInstance;
            singletoneOBj.isloged = true;
            InitializeComponent();
            var page = new PageMain();
            this.Content = page;
            singletoneOBj.mainwindow = this;
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

