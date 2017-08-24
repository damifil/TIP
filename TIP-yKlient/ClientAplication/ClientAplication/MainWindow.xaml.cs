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


    public partial class MainWindow : Window
    {

        SingletoneObject singletoneOBj;
        public MainWindow()
        {
            singletoneOBj = SingletoneObject.GetInstance;
            InitializeComponent();
            var page = new Page2();
            this.Content = page;
            singletoneOBj.mainwindow = this;
        }
        
        bool _shown;
    
        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
                singletoneOBj.isOnlineThread.Abort();
        }
    }
}

