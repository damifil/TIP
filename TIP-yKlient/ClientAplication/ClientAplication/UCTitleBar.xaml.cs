﻿using System;
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
    /// Interaction logic for UCTitleBar.xaml
    /// </summary>
    public partial class UCTitleBar : UserControl
    {
        public UCTitleBar()
        {
            InitializeComponent();
        }

        private void closeButon_Click(object sender, RoutedEventArgs e)
        {
            var targetWindow = Window.GetWindow(this);
            targetWindow.Close();
        }

        private void minimalizeButon_Click(object sender, RoutedEventArgs e)
        {
            var targetWindow = Window.GetWindow(this);
           targetWindow.WindowState = WindowState.Minimized;
        }
    }
}