/////////////////////////////////////////////////////////////////////////////////
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
using System.Diagnostics;
using System.Linq;
using System.Net;
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
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Contrib;

namespace Desktop_Authentication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Hard coded consumer and secret keys and base URL.
        // In real world Apps, these values need to secured.
        // One approach is to encrypt and/or obfuscate these values
        private const string m_ConsumerKey = "YOUR CONSUMER KEY HERE";
        private const string m_ConsumerSecret = "YOUR CONSUMER SECRET KEY HERE";
        private const string m_baseURL = "USE THE URL OF THE PROVIDER";/*Autodesk accounts URL is https://accounts.autodesk.com*/

        private static RestClient m_Client;

        private string m_oAuthReqToken;
        private string m_oAuthReqTokenSecret;
        private string m_strPIN;

        private string m_oAuthAccessToken;
        private string m_oAuthAccessTokenSecret;
        private string m_sessionHandle;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void oxygenWnd_Initialized(object sender, EventArgs e)
        {
            // Make sure that initially, except for Out of Band button and "Request Token" button,
            // all other controls are disabled or invisible
            authorizeTokenBtn.IsEnabled = false;
            pinTxt.IsEnabled = false;
            acssTokenBtn.IsEnabled = false;
            refreshTokenBtn.IsEnabled = false;
            invalidateButton.IsEnabled = false;
            verifierPinLabel.Visibility = Visibility.Collapsed;
            pinTxt.Visibility = Visibility.Collapsed;

        }

        // Obtain the request token. This is the first in the series of steps
        // need to use the Oxygen API to authorize access to a resource
        private void rqstTokenButton_Click(object sender, RoutedEventArgs e)
        {
            // Instantiate the RestSharp library object RestClient. RestSharp is free and makes it
            // very easy to build apps that use the OAuth and OpenID protocols with a provider supporting
            // these protocols
            m_Client = new RestClient(m_baseURL);

            // Check to see if Out of Band Authorization is preferred by the user and instantiate the
            // Authenticator object
            if (oobCheckBox.IsChecked == false)
            {
                m_Client.Authenticator =
          OAuth1Authenticator.ForRequestToken(m_ConsumerKey, m_ConsumerSecret);
            }
            else
                m_Client.Authenticator =
          OAuth1Authenticator.ForRequestToken(m_ConsumerKey, m_ConsumerSecret, "oob");

            // Build the HTTP request for a Request token and execute it against the OAuth provider
            var request = new RestRequest("OAuth/RequestToken", Method.POST);
            var response = m_Client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                m_Client = null;
                logOutput.AppendText("\nOops! Something went wrong - couldn't request token from Autodesk oxygen provider" + "\n" + "-----------------------" + "\n");
                authorizeTokenBtn.IsEnabled = false;

                acssTokenBtn.IsEnabled = false;
                refreshTokenBtn.IsEnabled = false;
                invalidateButton.IsEnabled = false;
                pinTxt.Text = "";
                return;
            }
            else
            {
                // The HTTP request succeeded. Get the request token and associated parameters.
                var qs = HttpUtility.ParseQueryString(response.Content);
                m_oAuthReqToken = qs["oauth_token"];
                m_oAuthReqTokenSecret = qs["oauth_token_secret"];
                var oauth_callback_confirmed = qs["oauth_callback_confirmed"];
                var x_oauth_client_identifier = qs["x_oauth_client_identifier"];
                var xoauth_problem = qs["xoauth_problem"];
                var oauth_error_message = qs["oauth_error_message"];
                authorizeTokenBtn.IsEnabled = true;
                acssTokenBtn.IsEnabled = false;
                refreshTokenBtn.IsEnabled = false;
                invalidateButton.IsEnabled = false;
                pinTxt.Text = "";
            }

            var responseNamedCollection = HttpUtility.ParseQueryString(response.Content);

            string outputString = "";
            foreach (string key in responseNamedCollection.AllKeys)
            {
                outputString += key + "=" + responseNamedCollection[key] + "\n";
            }

            //Output response log
            logOutput.AppendText("Request Token Response:\n" + outputString + "-----------------------" + "\n");
        }

        // This button gets enabled if the HTTP request for the request token succeeded
        private void authorizeTokenBtn_Click(object sender, RoutedEventArgs e)
        {
            
            if (oobCheckBox.IsChecked == false)
            {
                // For in band authorization build URL for Authorization HTTP request
                RestRequest authorizeRequest = new RestRequest
                {
                    Resource = "OAuth/Authorize",
                };

                authorizeRequest.AddParameter("viewmode", "desktop");
                authorizeRequest.AddParameter("oauth_token", m_oAuthReqToken);
                Uri authorizeUri = m_Client.BuildUri(authorizeRequest);

                // Launch another window with browser control and navigate to the Authorization URL
                BrowserAuthenticate frm = new BrowserAuthenticate();
                frm.Uri = authorizeUri;
                if (frm.ShowDialog() != true)
                {
                    logOutput.AppendText("In band Authorization failed" + "\n" + "-----------------------" + "\n");
                    m_Client = null;
                    authorizeTokenBtn.IsEnabled = false;

                    acssTokenBtn.IsEnabled = false;
                    refreshTokenBtn.IsEnabled = false;
                    invalidateButton.IsEnabled = false;
                    pinTxt.Text = "";
                    return;

                }
                logOutput.AppendText("In band Authorization succeeded" + "\n" + "-----------------------" + "\n");
               

            }
            else
            {
                //This is for Out of band Authorization, Build Authorization HTTP URL
                var request = new RestRequest("OAuth/Authorize");
                request.AddParameter("oauth_token", m_oAuthReqToken);
                Uri authorizeUri = m_Client.BuildUri(request);
                var url = authorizeUri.ToString();
                logOutput.AppendText("Requesting Out of band Authorization PIN" + "\n" + "-----------------------" + "\n");
                //Launch default browser with authorization URL
                Process.Start(url);
                
                //User logs into Oxygen  and clicks 'authorize' button
                //Browser displays PIN 

                pinTxt.IsEnabled = true;
            }

            acssTokenBtn.IsEnabled = true;
        }

        // The button associated with this handler is activated if Authorization of the
        // request token is successful. This button allows the user to swap out the
        // request token for an access token. This access token is what is used to access
        // resources protected by the authentication provider.
        private void acssTokenBtn_Click(object sender, RoutedEventArgs e)
        {
            // Build the HTTP request for an access token
            var request = new RestRequest("OAuth/AccessToken", Method.POST);
            if (oobCheckBox.IsChecked == false)
            {
                m_Client.Authenticator = OAuth1Authenticator.ForAccessToken(
            m_ConsumerKey, m_ConsumerSecret, m_oAuthReqToken, m_oAuthReqTokenSecret
            );
            }
            else
            {
                m_strPIN = pinTxt.Text;
                string strPIN = m_strPIN.Trim();
                if (strPIN == "")
                {
                    logOutput.AppendText("\nInvalid PIN." + "\n" + "-----------------------" + "\n");
                    m_Client = null;
                    authorizeTokenBtn.IsEnabled = false;

                    acssTokenBtn.IsEnabled = false;
                    refreshTokenBtn.IsEnabled = false;
                    invalidateButton.IsEnabled = false;
                    pinTxt.Text = "";
                    return;
                }
                //Use PIN to request access token for users account for an out of band request.
                m_Client.Authenticator = OAuth1Authenticator.ForAccessToken(
            m_ConsumerKey, m_ConsumerSecret, m_oAuthReqToken, m_oAuthReqTokenSecret, strPIN
            );
            }


            // Execute the access token request
            var response = m_Client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                m_Client = null;
                logOutput.AppendText("\nOops! Something went wrong - couldn't get access token from your Autodesk account" + "\n" + "-----------------------" + "\n");
                authorizeTokenBtn.IsEnabled = false;

                acssTokenBtn.IsEnabled = false;
                refreshTokenBtn.IsEnabled = false;
                invalidateButton.IsEnabled = false;
                pinTxt.Text = "";
                return;
            }
            else
            {
                // The request for access token is successful. Parse the response and store token,token secret and session handle
                var qs = HttpUtility.ParseQueryString(response.Content);
                m_oAuthAccessToken = qs["oauth_token"];
                m_oAuthAccessTokenSecret = qs["oauth_token_secret"];
                var x_oauth_user_name = qs["x_oauth_user_name"];
                var x_oauth_user_guid = qs["x_oauth_user_guid"];
                var x_scope = qs["x_scope"];
                var xoauth_problem = qs["xoauth_problem"];
                var oauth_error_message = qs["oauth_error_message"];
                m_sessionHandle = qs["oauth_session_handle"];

                refreshTokenBtn.IsEnabled = true;
                invalidateButton.IsEnabled = true;

                authorizeTokenBtn.IsEnabled = false;
            }

            var responseNamedCollection = HttpUtility.ParseQueryString(response.Content);

            string outputString = "";
            foreach (string key in responseNamedCollection.AllKeys)
            {
                outputString += key + "=" + responseNamedCollection[key] + "\n";
            }

            //Output response log
            logOutput.AppendText("Access Token Response:\n" + outputString + "-----------------------" + "\n");
        }

        // Get a fresh access token
        private void refreshTokenBtn_Click(object sender, RoutedEventArgs e)
        {
            // Build the HTTP request to obtain a fresh access token
            var request = new RestRequest("OAuth/AccessToken", Method.POST);
            if (oobCheckBox.IsChecked == false)
            {
                
                m_Client.Authenticator = OAuth1Authenticator.ForAccessTokenRefresh(
                    m_ConsumerKey,
                    m_ConsumerSecret,
                    m_oAuthAccessToken,
                    m_oAuthAccessTokenSecret,
                    m_sessionHandle);
            }
            else
            {
                m_strPIN = pinTxt.Text;
                string strPIN = m_strPIN.Trim();
                if (strPIN == "")
                {
                    logOutput.AppendText("\nInvalid PIN." + "\n" + "-----------------------" + "\n");
                    m_Client = null;
                    authorizeTokenBtn.IsEnabled = false;

                    acssTokenBtn.IsEnabled = false;
                    refreshTokenBtn.IsEnabled = false;
                    invalidateButton.IsEnabled = false;
                    pinTxt.Text = "";
                    return;
                }
                
                //Use PIN to request access token for users account 

                m_Client.Authenticator = OAuth1Authenticator.ForAccessTokenRefresh(
                    m_ConsumerKey,
                    m_ConsumerSecret,
                    m_oAuthAccessToken,
                    m_oAuthAccessTokenSecret,
                    strPIN,
                    m_sessionHandle);
            }

            
            var response = m_Client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                m_Client = null;
                logOutput.AppendText("\nOops! Something went wrong - couldn't get access token from your Autodesk account" + "\n" + "-----------------------" + "\n");
                authorizeTokenBtn.IsEnabled = false;

                acssTokenBtn.IsEnabled = false;
                refreshTokenBtn.IsEnabled = false;
                invalidateButton.IsEnabled = false;
                pinTxt.Text = "";
                return;
            }
            else
            {
                // The request for a fresh access token was successful. Store the new
                // access token, secret and session handle.
                var qs = HttpUtility.ParseQueryString(response.Content);
                m_oAuthAccessToken = qs["oauth_token"];
                m_oAuthAccessTokenSecret = qs["oauth_token_secret"];
                var x_oauth_user_name = qs["x_oauth_user_name"];
                var x_oauth_user_guid = qs["x_oauth_user_guid"];
                var x_scope = qs["x_scope"];
                var xoauth_problem = qs["xoauth_problem"];
                var oauth_error_message = qs["oauth_error_message"];
                m_sessionHandle = qs["oauth_session_handle"];
            }

            var responseNamedCollection = HttpUtility.ParseQueryString(response.Content);

            string outputString = "";
            foreach (string key in responseNamedCollection.AllKeys)
            {
                outputString += key + "=" + responseNamedCollection[key] + "\n";
            }

            //Output response log
            logOutput.AppendText("Refresh Access Token Response:\n" + outputString + "-----------------------" + "\n");
        }

        // This button allows you to relinquish the access token. This is the equivalent of
        // logging out.
        private void invalidateButton_Click(object sender, RoutedEventArgs e)
        {
            // Build the request to relinquish the access
            // token
            var request = new RestRequest("OAuth/InvalidateToken", Method.POST);

            if (oobCheckBox.IsChecked == false)
            {
                m_Client.Authenticator = OAuth1Authenticator.ForAccessTokenRefresh(
                    m_ConsumerKey,
                    m_ConsumerSecret,
                    m_oAuthAccessToken,
                    m_oAuthAccessTokenSecret,
                    m_sessionHandle);
            }
            else
            {
                m_strPIN = pinTxt.Text;
                string strPIN = m_strPIN.Trim();
                if (strPIN == "")
                {
                    logOutput.AppendText("\nInvalid PIN." + "\n" + "-----------------------" + "\n");
                    m_Client = null;
                    authorizeTokenBtn.IsEnabled = false;

                    acssTokenBtn.IsEnabled = false;
                    refreshTokenBtn.IsEnabled = false;
                    invalidateButton.IsEnabled = false;
                    pinTxt.Text = "";
                    return;
                }
                //Use PIN to request access token for users account 
                m_Client.Authenticator = OAuth1Authenticator.ForAccessTokenRefresh(
                    m_ConsumerKey,
                    m_ConsumerSecret,
                    m_oAuthAccessToken,
                    m_oAuthAccessTokenSecret,
                    strPIN,
                    m_sessionHandle);

            }
            
            // execute the request to relinquish token
            var response = m_Client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                m_Client = null;
                logOutput.AppendText("\nOops! Something went wrong - couldn't log out of your Autodesk account" + "\n" + "-----------------------" + "\n");
                authorizeTokenBtn.IsEnabled = false;

                acssTokenBtn.IsEnabled = false;
                refreshTokenBtn.IsEnabled = false;
                invalidateButton.IsEnabled = false;
                pinTxt.Text = "";
                return;
            }
            else
            {
                logOutput.AppendText("Logout successful!" + "\n" + "-----------------------" + "\n");
            }



            // Disable all controls except the out of band radio button and the Request Token button if the request
            // to relinquish the access token is successful.
            authorizeTokenBtn.IsEnabled = false;

            acssTokenBtn.IsEnabled = false;
            refreshTokenBtn.IsEnabled = false;
            invalidateButton.IsEnabled = false;
            pinTxt.Text = "";
        }

        private void oobCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            verifierPinLabel.Visibility = Visibility.Visible;
            pinTxt.Visibility = Visibility.Visible;
            pinTxt.IsEnabled = true;
        }

        private void oobCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            verifierPinLabel.Visibility = Visibility.Collapsed;
            pinTxt.Visibility = Visibility.Collapsed;
            pinTxt.IsEnabled = false;
        }
    }
}
