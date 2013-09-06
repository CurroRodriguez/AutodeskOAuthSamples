﻿/////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved 
// Created by Gopinath Taget, 2013 - ADN/Developer Technical Services
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted, 
// provided that the above copyright notice appears in all copies and 
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting 
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC. 
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////////////////

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
using System.Windows.Shapes;

namespace AutoCAD_Authentication
{
    /// <summary>
    /// Interaction logic for BrowserAuthenticate.xaml
    /// </summary>
    public partial class BrowserAuthenticate : Window
    {
        public Uri Uri { get; set; }
        public Uri ResultUri { get; set; }

        public BrowserAuthenticate()
        {
            InitializeComponent();
        }

        private void brwsrCtrl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void brwsrCtrl_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (brwsrCtrl.Source.AbsoluteUri.IndexOf("Allow") > -1)
            {
                ResultUri = brwsrCtrl.Source;
                DialogResult = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Uri != null)
            {
                brwsrCtrl.Source = Uri;
            }

        }
    }
}
