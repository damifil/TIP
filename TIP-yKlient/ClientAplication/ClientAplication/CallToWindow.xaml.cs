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
    /// Interaction logic for CallToWindow.xaml
    /// </summary>
    /// 
    public partial class CallToWindow : Window
    {
        SingletoneObject singletoneOBJ = SingletoneObject.GetInstance;
        internal User user { get; set; }
        internal DateTime dateBegin { get; set; }
        public CallToWindow()
        {
            InitializeComponent();
        }


        bool _shown;
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_shown)
                return;
            singletoneOBJ.phoneVOIP.callto = this;
            stringWithName.Text = "Nawiązywanie połączenia z " + user.Name;
            _shown = true;
            
        }


        private void callDisconectTextboxaction(object sender, MouseButtonEventArgs e)
        {
            string searchList = singletoneOBJ.client.sendMessage("SAVECALL " + singletoneOBJ.user.Name + " " +  user.Name + " " + dateBegin.ToString() + " " + DateTime.Now.ToString());
            singletoneOBJ.phoneVOIP.btn_HangUp_Click(user.Name);
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
