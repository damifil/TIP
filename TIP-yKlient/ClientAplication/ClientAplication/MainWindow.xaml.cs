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
        SingletoneObject singletoneOBJ = SingletoneObject.GetInstance;

        public MainWindow()
        {
            InitializeComponent();
        }
        
        bool _shown;
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_shown)
                return;
           
            _shown = true;
            lastActivity.Text = singletoneOBJ.user.lastActivity;
            welcomeString.Text = "Witaj " + singletoneOBJ.user.Name; 
            
        }
        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}

