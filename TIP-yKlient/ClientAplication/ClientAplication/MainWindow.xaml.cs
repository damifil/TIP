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
            lastActivity.Text = "twoja ostatnia aktywność: !2 kwietnia o godzinie 14:30";
            welcomeString.Text = "Witaj " + singletoneOBJ.user.Name; 
            
        }
  /*
        private void callToUser(object sender, MouseButtonEventArgs e)
        {
            User userLogged = user;
            TextBlock cmd = (TextBlock)sender;
            if (cmd.DataContext is User)
            {
               
                User user = (User)cmd.DataContext;
                phoneVOIP.nameCallToUser = user.Name;
                bool call = phoneVOIP.btn_PickUp_Click(user.Name);
                CallToWindow main = new CallToWindow();
                if (call == true)
                {
                    main.user = user;
                    main.userLogged = userLogged;
                    main.client = client;
                    main.dateBegin = DateTime.Now;
                    main.phoneVOIP = phoneVOIP;
                    main.Show();
                }

            }
        }
        */

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

      
    }
}

