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
        SingletoneObject singletoneOBJ = SingletoneObject.GetInstance;
        internal User user;
        internal string nameCallToUser;
        internal DateTime dateBegin;
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
            string saveCall = singletoneOBJ.client.sendMessage("SAVECALL " + singletoneOBJ.user.Name + " " + nameCallToUser + " " + dateBegin + " " + DateTime.Now.ToString());
            singletoneOBJ.phoneVOIP.btn_HangUp_Click(user.Name);
            this.Close();
        }

        private void callAceptTextboxaction(object sender, MouseButtonEventArgs e)
        {
            singletoneOBJ.phoneVOIP.btn_PickUp_Click(user.Name);
            this.Close();
        }

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
