using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ozeki.Media;
using Ozeki.VoIP;
using HashLib;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Threading;

namespace ClientAplication
{
    class PhoneVOIP
    {

        private ISoftPhone _softPhone;
        private IPhoneLine _phoneLine;
        private RegState _phoneLineInformation;
        private IPhoneCall _call;

        public PhoneVOIP ()
        {
            _microphone = Microphone.GetDefaultDevice();
            Speaker _speaker = Speaker.GetDefaultDevice();
            _connector = new MediaConnector();
            _mediaSender = new PhoneCallAudioSender();
            _mediaReceiver = new PhoneCallAudioReceiver();
            callto = null;
            transimiso = null;
        }

        internal Microphone _microphone   { get; set; }
        internal Speaker _speaker   { get; set; }
        MediaConnector _connector;
        PhoneCallAudioSender _mediaSender;
        PhoneCallAudioReceiver _mediaReceiver;
        internal CallToWindow callto   { get; set; }
        internal Client client { get; set; }
        internal AplicationUser userLogged { get; set; }
        public string nameCallToUser { get; set; }
        private bool _inComingCall;
        private string _reDialNumber;
        private bool _localHeld;
        CallFromWindow main=null;
        internal CallTransmisionWindow transimiso { get; set; }
        SIPAccount sa;
        public static byte[] HashPassword(string password)          // haszowanie hasla
        {
            IHash hash = HashFactory.Crypto.SHA3.CreateKeccak512();
            HashAlgorithm hashAlgo = HashFactory.Wrappers.HashToHashAlgorithm(hash);
            byte[] input = Encoding.UTF8.GetBytes(password);
            byte[] output = hashAlgo.ComputeHash(input);
            return output;
        }

        public void InitializeSoftPhone(string login, string password, string adresIP, int port)
        {
            //  try
            {
                _softPhone = SoftPhoneFactory.CreateSoftPhone(SoftPhoneFactory.GetLocalIP(), 5700, 5750);
                InvokeGUIThread(() => { });

                _softPhone.IncomingCall += softPhone_inComingCall;
                byte[] userPass = HashPassword(password);
                string pass = System.Text.Encoding.UTF8.GetString(userPass, 0, userPass.Length);
                sa = new SIPAccount(true, login, login, login, pass, adresIP, port);
                InvokeGUIThread(() => { });

                _phoneLine = _softPhone.CreatePhoneLine(sa);
                _phoneLine.RegistrationStateChanged += phoneLine_PhoneLineInformation;
                _softPhone.RegisterPhoneLine(_phoneLine);

                



               
                

                _inComingCall = false;
                _reDialNumber = string.Empty;
                _localHeld = false;

                ConnectMedia();
            }
            /* catch (Exception ex)
             {
                 InvokeGUIThread(() => { MessageBox.Show("Problem z polaczeniem do OZEKI"); });
             }*/
        }

        public void LogOff()
        {
            if (_softPhone != null && _phoneLine != null)
            {
                
                _phoneLine.RegistrationStateChanged -= phoneLine_PhoneLineInformation;
                _softPhone.IncomingCall -= softPhone_inComingCall;
                _softPhone.UnregisterPhoneLine(_phoneLine);
                _phoneLine.Dispose();
                _softPhone.Close();
               
                sa = null;
            }
        }
        private void StartDevices()
        {
            if (_microphone != null)
            {
                _microphone.Start();
                InvokeGUIThread(() => { });
            }

            if (_speaker != null)
            {
                _speaker.Start();
                InvokeGUIThread(() => { });
            }
        }


        private void StopDevices()
        {
            if (_microphone != null)
            {
                _microphone.Stop();
                InvokeGUIThread(() => { });
            }

            if (_speaker != null)
            {
                _speaker.Stop();
                InvokeGUIThread(() => { });
            }
        }


        private void ConnectMedia()
        {
            if (_microphone != null)
            {
                _connector.Connect(_microphone, _mediaSender);
            }

            if (_speaker != null)
            {
                _connector.Connect(_mediaReceiver, _speaker);
            }
        }

        private void DisconnectMedia()
        {
            if (_microphone != null)
            {
                _connector.Disconnect(_microphone, _mediaSender);
            }

            if (_speaker != null)
            {
                _connector.Disconnect(_mediaReceiver, _speaker);
            }
        }


        private void InvokeGUIThread(Action action)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(action);
        }


        private void softPhone_inComingCall(object sender, VoIPEventArgs<IPhoneCall> e)
        {

            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (main == null)
                {
                    main = new CallFromWindow();
                    main.user = new User(true, true);
                    main.dateBegin = DateTime.Now;
                    main.nameCallToUser = e.Item.DialInfo.CallerDisplay;
                    nameCallToUser = e.Item.DialInfo.CallerDisplay;
                    main.user.Name = e.Item.DialInfo.CallerDisplay;
                    main.Show();
                }
            }));




            _reDialNumber = e.Item.DialInfo.Dialed;
            _call = e.Item;
            WireUpCallEvents();
            _inComingCall = true;
        }



        private void phoneLine_PhoneLineInformation(object sender, RegistrationStateChangedArgs e)
        {
            _phoneLineInformation = e.State;

            InvokeGUIThread(() =>
            {
                if (_phoneLineInformation == RegState.RegistrationSucceeded)
                {

                }
                else
                {

                }

            });
        }


        private void call_CallStateChanged(object sender, CallStateChangedArgs e)
        {
            InvokeGUIThread(() => {  });

            if (e.State == CallState.Answered)
            {

                StartDevices();
                _mediaReceiver.AttachToCall(_call);
                _mediaSender.AttachToCall(_call);
            }

            if (e.State == CallState.InCall)
            {

                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (callto != null)
                    { callto.Close(); callto = null; }
                    if (transimiso == null)
                    {
                        transimiso = new CallTransmisionWindow();
                        transimiso.nameCallToUser = nameCallToUser;
                        transimiso.dateBegin = DateTime.Now;
                        transimiso.client = client;
                        transimiso.user = new User(true, true);
                        transimiso.Show();
                    }
                }));
            }

            if (e.State.IsCallEnded())
            {
                _localHeld = false;

                StopDevices();

                _mediaReceiver.Detach();
                _mediaSender.Detach();

                WireDownCallEvents();

                _call = null;

                _inComingCall = false;
                InvokeGUIThread(() => { });
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (transimiso != null)
                    { transimiso.Close(); transimiso = null; }
                    if (main != null)
                    {
                        main.Close(); main = null;
                    }
                    if (callto != null)
                    { callto.Close(); callto = null; }
                    
                }));
            }

            if (e.State == CallState.LocalHeld)
            {


                StopDevices();
            }

        }


        private void WireUpCallEvents()
        {
            _call.CallStateChanged += (call_CallStateChanged);
        }

        private void WireDownCallEvents()
        {
            _call.CallStateChanged -= (call_CallStateChanged);
        }








        public bool btn_PickUp_Click(string Name)
        {
            if (_inComingCall)
            {

                _inComingCall = false;
                _call.Answer();


                return false;
            }

            if (_call != null)
            {
                return false;
            }

            if (_phoneLineInformation != RegState.RegistrationSucceeded)
            {
                InvokeGUIThread(() => { });
                return false;
            }

            _call = _softPhone.CreateCallObject(_phoneLine, Name);
            WireUpCallEvents();
            _call.Start();
            return true;
        }





        public void btn_HangUp_Click(string Name)
        {

            if (_call != null)
            {
                if (_inComingCall && _call.CallState == CallState.Ringing)
                {
                    _call.Reject();
                }
                else
                {
                    _call.HangUp();
                    _inComingCall = false;
                    InvokeGUIThread(() => { });
                }
                _inComingCall = false;
                _call = null;

            }

        }







    }
}
