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
    /// Interaction logic for CallFrom.xaml
    /// </summary>
    public partial class CallFrom : Window
    {
        internal PhoneVOIP pv;
        internal User user;
        internal User UserLogged;
        internal Client client;
        internal string nameCallToUser;
        public CallFrom()
        {
            InitializeComponent();

        }


      

        bool _shown;
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_shown)
                return;


            stringWithName.Text = "Połączenie " + user.Name;
            _shown = true;






        }


        private void callDisconectTextboxaction(object sender, MouseButtonEventArgs e)
        {
            string saveCall = client.sendMessage("SAVECALL " + UserLogged.Name + " " + nameCallToUser + " " + DateTime.Now.ToString() + " " + DateTime.Now.ToString());
            pv.btn_HangUp_Click(user.Name);
            this.Close();

        }

        private void callAceptTextboxaction(object sender, MouseButtonEventArgs e)
        {
            
            pv.btn_PickUp_Click(user.Name);
            this.Close();

        }


    }
}
