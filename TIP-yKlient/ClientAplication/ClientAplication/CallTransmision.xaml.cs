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
    /// Interaction logic for CallTransmision.xaml
    /// </summary>
    public partial class CallTransmision : Window
    {
        internal PhoneVOIP phoneVOIP;
        internal User user;
        internal Client client;
        internal User userLogged;
        internal string nameCallToUser;
        internal DateTime dateBegin;
        public CallTransmision()
        {
            InitializeComponent();
        }

        bool _shown;
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_shown)
                return;


            stringWithName.Text = "Połączenie z " + nameCallToUser;
            _shown = true;

        }


        private void callDisconectTextboxaction(object sender, MouseButtonEventArgs e)
        {
            string saveCall = client.sendMessage("SAVECALL " + userLogged.Name + " " + nameCallToUser + " " + dateBegin.ToString() + " " + DateTime.Now.ToString());
            phoneVOIP.btn_HangUp_Click(user.Name);
            this.Close();

        }

    }
}
