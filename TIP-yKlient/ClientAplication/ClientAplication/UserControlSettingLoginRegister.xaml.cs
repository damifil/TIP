using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        ReadSettings rs=ReadSettings.GetInstance;
        public UserControl2()
        {
            
            InitializeComponent();
            adresIPinput.Text = rs.IP;
            numberPortInput.Text = rs.PORT;
        }

        private void change_click(object sender, RoutedEventArgs e)
        {
            IPAddress address;
            if (adresIPinput.Text.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Length == 4) {
                if (IPAddress.TryParse(adresIPinput.Text, out address))
                {
                    rs.saveSettings(adresIPinput.Text, numberPortInput.Text);
                    return;
                }
            }
                MessageBox.Show("niepoprawny adres IP");
        }
    }
}
