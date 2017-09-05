﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ClientAplication
{
    /// <summary>
    /// Interaction logic for CallTransmision.xaml
    /// </summary>
    public partial class CallTransmision : Window
    {
        SingletoneObject singletoneOBJ = SingletoneObject.GetInstance;
        internal User user;
        internal Client client;
        internal string nameCallToUser;
        internal DateTime dateBegin;
        
       public int timesek = 0;
        public CallTransmision()
        {
            InitializeComponent();
           
            Application.Current.MainWindow.Closing += new CancelEventHandler(Window_Closing);
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            
        }

        bool _shown;
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_shown)
                return;


            stringWithName.Text = "Połączenie z " + nameCallToUser;
            _shown = true;

        }

        void Window_Closing(object sender, CancelEventArgs e)
        {
            MessageBox.Show("zamknelo");
            

        }

        
        private void callDisconectTextboxaction(object sender, MouseButtonEventArgs e)
        {
            string saveCall = client.sendMessage("SAVECALL " + singletoneOBJ.user.Name + " " + nameCallToUser + " " + dateBegin.ToString() + " " + DateTime.Now.ToString());
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan t = DateTime.Now - dateBegin;
            int hour = (int)t.TotalHours;
            int minute = (int)t.TotalMinutes % 60;
            int second = (int)t.Seconds % 60;
            timeTextBlock.Text = "Czas rozmowy: " +hour.ToString("00") +":"+minute.ToString("00") + ":" + second.ToString("00");
        }
    }
}
