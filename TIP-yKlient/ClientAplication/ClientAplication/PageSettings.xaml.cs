using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Ozeki.Media;
using Ozeki.VoIP;

namespace ClientAplication
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        List<AudioDeviceInfo> MIcrophoneDevices;
        List<AudioDeviceInfo> SpeakerDevices;
        SingletoneObject singletoneOBj = SingletoneObject.GetInstance;
        public SettingsPage()
        {
            InitializeComponent();
            //lista mikrofonów
            MIcrophoneDevices = Microphone.GetDevices();
            CBMicrophone.ItemsSource = MIcrophoneDevices;
            CBMicrophone.SelectedIndex = 0;


            //lista głośników
            SpeakerDevices = Speaker.GetDevices();
            CBSound.ItemsSource = SpeakerDevices;
            CBSound.SelectedIndex = 0;
            UC.settings.Visibility = System.Windows.Visibility.Hidden;
        }


        private void backAction(object sender, MouseButtonEventArgs e)
        {
            singletoneOBj.mainwindow.Width = 300;
            var page = new Page2();
            singletoneOBj.mainwindow.Content = page;
        }

        void CBMicrophoneHandler(object sender, MouseButtonEventArgs e)
        {
            singletoneOBj.phoneVOIP._speaker.Dispose();
            singletoneOBj.phoneVOIP._microphone = Microphone.GetDevice(MIcrophoneDevices[CBMicrophone.SelectedIndex]);
        }


        void CBSoundHandler(object sender, MouseButtonEventArgs e)
        {
            singletoneOBj.phoneVOIP._speaker.Dispose();
            singletoneOBj.phoneVOIP._speaker = Speaker.GetDevice(SpeakerDevices[CBSound.SelectedIndex]);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationClass.isValidPassword(OldPassword.Password.ToString())
                && ValidationClass.isValidPassword(newPassword.Password.ToString()))
            {
                if (OldPassword.Password.ToString() != newPassword.Password.ToString())
                {
                    if (newPassword.Password.ToString() == newPassword2.Password.ToString())
                    {
                        //akcja wysyłania nowego adresu email.
                        string message = singletoneOBj.client.sendMessage("CHPASS " + singletoneOBj.user.Name+" "+  newPassword.Password.ToString());
                    }
                    else
                    {
                        MessageBox.Show("nowe hasło musi być takie same w polu nowe hasło i powtórz hasło");
                    }
                }
                else
                {
                    MessageBox.Show("Wpisz inne hasło niż stare");
                }
            }
            else
            {
                MessageBox.Show("wpisz poprawne hasło według zasady:\nhaslo og 8 do 16 znaków\njedna mala litera\njedna duża litera\njedna cyfra");
            }
        }

        private void deleteAcount(object sender, MouseButtonEventArgs e)
        {

            MessageBoxResult result = MessageBox.Show("Czy na pewno chceszusunąć konto", "Usunięcie konta", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                string message = singletoneOBj.client.sendMessage("DELACCOUNT " + singletoneOBj.user.Name);
                if (message == "True") 
                {
                    MessageBox.Show("pomyślnie usunięto konto");
                    singletoneOBj.isOnlineThread.Abort();
                    UC.refreshListThread.Abort();
                    singletoneOBj.setdefaultvalue();
                    LoginRegisterWindow main = new LoginRegisterWindow();
                    Window.GetWindow(this).Close();
                    main.Show();
                }
                else
                {
                    MessageBox.Show("Wystąpił problem podczas usuwania konta");
                }

            }

        }
    }
}
