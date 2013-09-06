/////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved 
// Written by Philippe Leefsma 2013 - ADN/Developer Technical Services
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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace AdnOAuthCSharp
{
    public sealed partial class MainPage : Page
    {
        // constants
        private string BaseUrl = "https://your.oauth.server.here";
        private string ConsumerKey = "your.consumer.key.here";
        private string ConsumerSecret = "your.comsumer.secret.key.here";

        private bool _isLoggedIn = false;

        // Connector Utility
        AdnOAuthConnector _connector;

        public MainPage()
        {
            this.InitializeComponent();

            ClearInfos();

            KeyDown += (s, e) =>
            {
                if (e.Key == Windows.System.VirtualKey.Escape)
                    OnQuit();
            };
        }

        async void DisplayError(string problem, string message)
        {
            string content =
                "Problem: " + problem +
                System.Environment.NewLine +
                "Message: " + message;

            MessageDialog dialog = new MessageDialog(
                content,
                "OAuth Authentication failed...");

            UICommand cmdOk = new UICommand("OK");

            dialog.Commands.Add(cmdOk);

            await dialog.ShowAsync();
        }

        private void ClearInfos()
        {
            listView.DataContext =
                new ObservableCollection<object>(
                    new List<object>() 
                    { 
                        new { Name = "Status:", Value = "Logged out..." }                  
                    });
        }

        private void DisplayInfos()
        {
            listView.DataContext =
                new ObservableCollection<object>(
                    new List<object>() 
                    { 
                        new { Name = "Status:", Value = "Logged in!" },
                        new { Name = "User: ", Value = _connector.UserName},
                        new { Name = "Access Token: ", Value = _connector.AccessToken },
                        new { Name = "Access Token Secret: ", Value = _connector.AccessTokenSecret },
                        new { Name = "Session Handle: ", Value = _connector.SessionHandle },
                        new { Name = "Token Expires: ", Value = SecToTime(_connector.TokenExpire) },
                        new { Name = "Session Expires: ", Value = SecToTime(_connector.SessionExpire) }
                    });
        }

        private string SecToTime(double sec)
        {
            TimeSpan ts = TimeSpan.FromSeconds(sec);

            string result = string.Format("{0} Days {1:D2}h:{2:D2}m:{3:D2}s",
                ts.Days,    
                ts.Hours,
                ts.Minutes,
                ts.Seconds);

            return result;
        }

        // Event Handlers

        private async void OnLoginButtonClicked(
            object sender, 
            RoutedEventArgs e)
        {
            // Automatically close AppBar
            _AppBar.IsOpen = false;

            ClearInfos();

            bool isNetworkAvailable = 
                NetworkInterface.GetIsNetworkAvailable();

            if (isNetworkAvailable)
            {
                _connector = new AdnOAuthConnector(
                    BaseUrl,
                    ConsumerKey,
                    ConsumerSecret);

                _connector.OnError += OnError;

                if (_isLoggedIn = await _connector.DoLogin())
                {
                    DisplayInfos();
                }
            }
            else
            {
                MessageDialog dialog = new MessageDialog(
                    "You appear to be offline, " +
                    "try to connect to an internet network first...",
                    "Network unavailable");

                UICommand cmdOK = new UICommand("OK", null, 2);

                dialog.Commands.Add(cmdOK);

                dialog.DefaultCommandIndex = 2;

                await dialog.ShowAsync();
            }
        }

        private async void OnLogoutButtonClicked(
           object sender,
           RoutedEventArgs e)
        {
            // Automatically close AppBar
            _AppBar.IsOpen = false;

            ClearInfos();

            _isLoggedIn = false;

            bool res = await _connector.DoLogout();
        }

        private async void OnRefreshButtonClicked(
            object sender,
            RoutedEventArgs e)
        {
            // Automatically close AppBar
            _AppBar.IsOpen = false;

            if (_isLoggedIn = await _connector.RefreshToken())
            {
                DisplayInfos();
            }
        }

        private void OnAppBarOpened(object sender, object e)
        {
            // Checks if we are already logged in to enable Refresh/Invalidate buttons

            LoginBtn.IsEnabled = !_isLoggedIn;
            RefreshBtn.IsEnabled = _isLoggedIn;
            LogoutBtn.IsEnabled = _isLoggedIn;
        }

        private void OnQuitButtonClicked(object sender, RoutedEventArgs e)
        {
            OnQuit();
        }

        private void OnError(string problem, string msg)
        {
            DisplayError(problem, msg);
        }

        private async void OnQuit()
        {
            MessageDialog dialog = new MessageDialog("Comfirm Quit...?");

            UICommand cmdYes = new UICommand("Yes", (cmd) =>
            {
                App.Current.Exit();
            }, 1);

            UICommand cmdNo = new UICommand("No", null, 2);

            dialog.Commands.Add(cmdNo);
            dialog.Commands.Add(cmdYes);

            dialog.DefaultCommandIndex = 2;

            await dialog.ShowAsync();
        }
    }
}
