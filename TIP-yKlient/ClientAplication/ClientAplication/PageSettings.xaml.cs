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
            var page = new PageMain();
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
            if (!ValidationClass.isValidPassword(OldPassword.Password.ToString()))
            {
                MessageBox.Show("Stare hasło ma zły format. Hasło musi mieć od 8 do 16 znaków oraz posiadać conajmniej:\n - jedną małą literę\n - jedną dużą literę\n - jedną cyfrę");
                return;
            }
            if (!ValidationClass.isValidPassword(newPassword.Password.ToString()))
            {
                MessageBox.Show("Nowe hasło ma zły format. Hasło musi mieć od 8 do 16 znaków oraz posiadać conajmniej:\n - jedną małą literę\n - jedną dużą literę\n - jedną cyfrę");
                return;
            }

            if (OldPassword.Password.ToString() != newPassword.Password.ToString())
            {
                if (newPassword.Password.ToString() == newPassword2.Password.ToString())
                {
                    //akcja wysyłania nowego adresu email.
                    string message = singletoneOBj.client.sendMessage("CHPASS " + singletoneOBj.user.Name + " " + OldPassword.Password.ToString() + " " + newPassword.Password.ToString());
                    switch(message)
                    {
                        case "OK":
                            MessageBox.Show("Hasło zostało zmienione");
                            return;
                        case "ERROR":
                            MessageBox.Show("Hasło nie zostało zmienione");
                            return;
                        default:
                            MessageBox.Show(message);
                            return;
                    }
                }
                else
                {
                    MessageBox.Show("Wpisane hasła nie są takie same w polach \"Nowe hasło\" i \"Powtórz hasło\" ");
                }
            }
            else
            {
                MessageBox.Show("Nowe hasło musi być inne niż stare");
            }
        }
        private void deleteAcount(object sender, MouseButtonEventArgs e)
        {

            MessageBoxResult result = MessageBox.Show("Czy na pewno chcesz usunąć konto", "Usunięcie konta", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                string message = singletoneOBj.client.sendMessage("DELACCOUNT " + singletoneOBj.user.Name);
                if (message == "OK")
                {
                    MessageBox.Show("Pomyślnie usunięto konto");
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
