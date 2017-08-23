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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientAplication
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    public partial class UserControl2 : UserControl
    {
        ReadSettings rs;
        public UserControl2()
        {
            rs = new ReadSettings();
            InitializeComponent();
            adresIPinput.Text = rs.IP;
            numberPortInput.Text = rs.PORT;
        }

        private void change_click(object sender, RoutedEventArgs e)
        {
            rs.saveSettings(adresIPinput.Text, numberPortInput.Text);
        }
    }
}
