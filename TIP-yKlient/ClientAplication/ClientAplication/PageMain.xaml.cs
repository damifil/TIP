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
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class Page2 : Page
    {
        SingletoneObject singletoneOBJ = SingletoneObject.GetInstance;
        public Page2()
        {
            InitializeComponent();
            try
            {
                lastActivity.Text = singletoneOBJ.user.lastActivity;
                welcomeString.Text = "Witaj " + singletoneOBJ.user.Name;
            }
            catch (Exception e) { }
           
        }

     

    }
}