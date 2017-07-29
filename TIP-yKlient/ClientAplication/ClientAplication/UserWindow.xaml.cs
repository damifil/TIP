﻿using System;
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
using System.Windows.Shapes;

namespace ClientAplication
{
    /// <summary>
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
  
        internal ObservableCollection<itemTB> items;   //  historia rozmow z uzytkownikiem
        SingletoneObject singletoneOBJ = SingletoneObject.GetInstance;

        public UserWindow(List<ListHistory> listHistory)
        {
            InitializeComponent();
            stringInUserWindow.Text = "Historia ostatnich połączeń z użytkownikem Nazwa uzytkownika";

            items = new ObservableCollection<itemTB>();
            foreach (ListHistory item in listHistory)
            {
                items.Add(new itemTB()
                {
                    Name = item.userName,
                    Describe = "Rozmowa z użytkownikiem " + item.userName + " o godzinie " + item.hourBegin,
                    Describe2 = "rozpoczęła się dnia " + item.dayBegin,
                    Describe3 = "i zakonczyła się dnia " + item.dayEnd + " o godzinie " + item.hourEnd,
                    Date = "dzinen"
                });
            }
            lbHistoryAll.DataContext = items;

        }

        bool _shown;
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_shown)
                return;

            _shown = true;
        }

        private void callToFriend(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Dzwonimy do " + singletoneOBJ.user.Name);
        }
        private void deleteFriend(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("usuwamy " + singletoneOBJ.user.Name);
        }
        private void searchClickHistoryuser(object sender, RoutedEventArgs e)
        {
            string value = userhistorysearchinput.Text;
            for (int i = 0; i < items.Count; i++)
            {
                if (value != items[i].Date)
                {
                    items.RemoveAt(i);
                    i--;
                }
            }

        }


    }
}
