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
        internal User user;
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


            stringWithName.Text = "połączenia z " + user.Name;
            _shown = true;

        }


        private void callDisconectTextboxaction(object sender, MouseButtonEventArgs e)
        {
            this.Close();

        }

    }
}
