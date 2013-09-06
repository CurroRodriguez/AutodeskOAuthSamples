/////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved 
// Written by Philippe Leefsma 2013 - ADN/Developer Technical Services
// Original Version of the code extracted from http://vikingco.de/webbroker.html
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
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace AdnOAuthCSharp
{
    public sealed partial class AdnWebAuthView : Page
    {
        public EventHandler<AdnWebAuthViewEventArgs> CancelledEvent
        {
            get;
            set;
        }

        public EventHandler<AdnWebAuthViewEventArgs> UriChangedEvent
        {
            get;
            set;
        }

        public EventHandler<AdnWebAuthViewEventArgs> NavFailedEvent
        {
            get;
            set;
        }

        public AdnWebAuthView()
        {
            this.InitializeComponent();

            Loaded += FlexWebAuth_Loaded;

            wv.LoadCompleted += wv_LoadCompleted;
            wv.NavigationFailed += wv_NavigationFailed;
        }

        void wv_NavigationFailed(
            object sender,
            WebViewNavigationFailedEventArgs e)
        {
            if (NavFailedEvent != null)
            {
                NavFailedEvent.Invoke(
                    this, new AdnWebAuthViewEventArgs()
                    {
                        Uri = e.Uri
                    });
            }
        }

        private void wv_LoadCompleted(
            object sender,
            NavigationEventArgs e)
        {
            if (UriChangedEvent != null)
            {
                UriChangedEvent.Invoke(
                    this, new AdnWebAuthViewEventArgs()
                    {
                        Uri = e.Uri
                    });
            }
        }

        private void FlexWebAuth_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            wv.Width = 800;
        }

        public void Navigate(Uri uri)
        {
            wv.Navigate(uri);
        }

        private void CancelButtonClicked(
            object sender,
            RoutedEventArgs e)
        {
            if (CancelledEvent != null)
            {
                CancelledEvent.Invoke(
                    this, new AdnWebAuthViewEventArgs()
                    {
                        Uri = null
                    });
            }
        }
    }

    public sealed class AdnWebAuthViewEventArgs
    {
        public Uri Uri
        {
            get;
            set;
        }
    }
}
