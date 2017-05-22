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
        internal User user;
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


            stringWithName.Text = "połączenia z " + user.Name;
            _shown = true;

        }


        private void callDisconectTextboxaction(object sender, MouseButtonEventArgs e)
        {
            this.Close();

        }

        private void callAceptTextboxaction(object sender, MouseButtonEventArgs e)
        {
            CallToWindow main = new CallToWindow();
            main.user = user;
            main.Show();
            this.Close();

        }


    }
}
