using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ozeki.Network;
using Ozeki.VoIP;
using TIPySerwer.Models;

namespace TIPySerwer
{
    class PBX : PBXBase
    {        
        string localAddress;

        public PBX(string localAddress, int minPortRange, int maxPortRange) : base(minPortRange, maxPortRange)
        {
            this.localAddress = localAddress;
            Console.WriteLine("Adres serwera: " + localAddress);
        }



        protected override void OnStart()
        {
            SetListenPort(localAddress, 5060, TransportType.Udp);
            Console.WriteLine("Port nasłuchowy: 5060(UDP)");
            base.OnStart();
        }

        protected override AuthenticationResult OnAuthenticationRequest(ISIPExtension extension, RequestAuthenticationInfo authInfo)
        {
            Console.WriteLine("Żądanie uwierzytelnienia od: " + extension.ExtensionID);

            AuthenticationResult result = new AuthenticationResult();
            
            if (UserManager.IsLoginExists(extension.ExtensionID))   // sprawdzenie czy uzytkownik istnieje w bazie danych
            {
                tipBDEntities db = new tipBDEntities();
                byte[] userPass = db.Users.Where(x => x.Login == extension.ExtensionID).Select(x => x.Password).SingleOrDefault(); // uzyskanie hasla z bazy danych
                string pass=  System.Text.Encoding.UTF8.GetString(userPass, 0, userPass.Length);                  
                result = extension.CheckPassword(extension.ExtensionID, pass, authInfo);               // sprawdzenie czy wpisane dane zgadzaja sie z danymi w bazie danych
            }
            else
            {
                Console.WriteLine("Błąd z uwierzytelnieniem użytkownika: " + extension.ExtensionID);
            }

            if (result.AuthenticationAccepted)
            {
                Console.WriteLine("Uwierzytelnienie powiodło się dla użytkownika: " + extension.ExtensionID);
            }
            else
            {
                Console.WriteLine("Uwierzytelnienie nie powiodło się dla użytkownika: " + extension.ExtensionID);
            }

            return result;
        }

        protected override RegisterResult OnRegisterReceived(ISIPExtension extension, SIPAddress from, int expires)
        {
            Console.WriteLine("Otrzymano żądanie rejestracji od: " + extension.ExtensionID);
            return base.OnRegisterReceived(extension, from, expires);
        }

        protected override void OnUnregisterReceived(ISIPExtension extension)
        {
            Console.WriteLine("Otrzymano żadanie wyrejestrowania od: " + extension.ExtensionID);
            base.OnUnregisterReceived(extension);
        }

        protected override void OnCallRequestReceived(ISessionCall call)
        {
            Console.WriteLine("Otrzymano żadanie rozmowy. Dzwoniący: " + call.DialInfo.CallerID + " odbiorca: " + call.DialInfo.Dialed);
            base.OnCallRequestReceived(call);
        }

        

       
    }
}
