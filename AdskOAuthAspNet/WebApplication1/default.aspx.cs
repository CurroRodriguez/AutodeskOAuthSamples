/////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved 
// Created by Daniel Du, 2013 - ADN/Developer Technical Services
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
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RestSharp;
using RestSharp.Authenticators;

namespace WebApplication1
{
    public partial class _default : System.Web.UI.Page
    {

        // Hard coded consumer and secret keys and base URL.
        // In real world Apps, these values need to secured and
        // preferably not hardcoded.
        private const string m_ConsumerKey = "your.consumer.key.here";
        private const string m_ConsumerSecret = "your.comsumer.secret.key.here";
        private const string m_baseURL = "https://your.oauth.server.here";

        //you need to change the domain name and port number if you are using different ones.
        private const string m_CallbackURL = "http://localhost:25574/callback.aspx";


        private static RestClient m_Client;

        private static string m_oAuthReqToken;
        private static string m_oAuthReqTokenSecret;

        private static string m_oAuthAccessToken;
        private static string m_oAuthAccessTokenSecret;
        private static string m_sessionHandle;


        protected void Page_Load(object sender, EventArgs e)
        {



        }


        //  Obtain the request token then get the authentication url to start a another broswer window for authentication
        // This is the first in the series of steps
        // need to use the Autodesk OAuth API to authorize access to a resource
        protected void btnAuthenticate_Click(object sender, EventArgs e)
        {
            // Instantiate the RestSharp library object RestClient. RestSharp is free and makes it
            // very easy to build apps that use the OAuth and OpenID protocols with a provider supporting
            // these protocols
            m_Client = new RestClient(m_baseURL);

            
            if (oobCheckBox.Checked)
            {
                //for out-of-band, use "oob" as callback url
                m_Client.Authenticator = OAuth1Authenticator.ForRequestToken(
                m_ConsumerKey, m_ConsumerSecret, "oob");

                txtVerifier.Enabled = true;
            }
            else
            {
                // for in-band, you can specify a callback url,  which will be redirect to 
                // after the user is authenticated 
                // the callback url parameter can be ommited if you do not want to use callback 
                m_Client.Authenticator = OAuth1Authenticator.ForRequestToken(
                m_ConsumerKey, m_ConsumerSecret, m_CallbackURL);
            }

            // Build the HTTP request for a Request token and execute it against the OAuth provider
            var request = new RestRequest("OAuth/RequestToken", Method.POST);
            var response = m_Client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                m_Client = null;
                logOutput.Text += ("</br>Oops! Something went wrong - couldn't request token Autodesk oxygen provider" + "</br>" + "-----------------------" + "</br>");
                return;
            }
            else
            {
                //get the request token successfully
                //Get the request token and associated parameters.
                var qs = HttpUtility.ParseQueryString(response.Content);
                m_oAuthReqToken = qs["oauth_token"];
                m_oAuthReqTokenSecret = qs["oauth_token_secret"];


                //build URL for Authorization HTTP request
                RestRequest authorizeRequest = new RestRequest
                {
                    Resource = "OAuth/Authorize",
                    Method = Method.GET  //must be GET, POST will cause "500 - Internal server error."
                };

                authorizeRequest.AddParameter("viewmode", "full");
                authorizeRequest.AddParameter("oauth_token", m_oAuthReqToken);

                Uri authorizeUri = m_Client.BuildUri(authorizeRequest);
                var url = authorizeUri.ToString();

                // Launch another window with browser and navigate to the Authorization URL
                Response.Write("<script>window.showModalDialog('" + url + "',this, 'dialogWidth:800px;dialogHeight:500px;resizable:yes;');</script>");
                //Response.Write("<script>window.open('" + authorizeUri.ToString() + "');</script>");

            }
            



        }


        //  This button allows the user to swap out the
        // request token for an access token. This access token is what is used to access
        // resources protected by the authentication provider.
        protected void btnGetAccessToken_Click(object sender, EventArgs e)
        {
            // Build the HTTP request for an access token
            var request = new RestRequest("OAuth/AccessToken", Method.POST);

            String verifier = getVerifier();
            if (verifier.Length == 0)
            {
                logOutput.Text += ("</br>Invalid PIN." + "</br>" + "-----------------------" + "</br>");
                return;
            }
                       
            
            m_Client.Authenticator = OAuth1Authenticator.ForAccessToken(
                m_ConsumerKey, m_ConsumerSecret, m_oAuthReqToken, m_oAuthReqTokenSecret,
                verifier);


            // Execute the access token request
            var response = m_Client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                m_Client = null;
                logOutput.Text += ("</br>Oops! Something went wrong - couldn't get access token from your Autodesk account" + "</br>" + "-----------------------" + "</br>");
                

                btnGetAccessToken.Enabled = false;
                BtnRefreshToken.Enabled = false;
                btnInvalideToken.Enabled = false;
                txtVerifier.Text = "";
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

                BtnRefreshToken.Enabled = true;
                btnInvalideToken.Enabled = true;

            }


            var responseNamedCollection = HttpUtility.ParseQueryString(response.Content);

            string outputString = "";
            foreach (string key in responseNamedCollection.AllKeys)
            {
                outputString += key + "=" + responseNamedCollection[key] + "</br>";
            }

            //Output response log
            logOutput.Text += ("Access Token Response:</br>" + outputString + "-----------------------" + "</br>");



        }

        // get the verifier
        private string getVerifier()
        {
            //for out of band, the user should have copied the verifier to the text bos manually after authentication
            if (oobCheckBox.Checked)
            {
                if (txtVerifier.Text.Trim().Length == 0)
                {
                    //verifier invalid
                    return string.Empty;
                }

                return txtVerifier.Text.Trim();
            }
            else
            {
                //for out of band, the verifier should have saved into session after authentication, please refer to callback.asp.cs
                if (Session["verifier"] == null)
                {
                    //verifier invalid
                    return string.Empty;
                }
                else
                {
                    return Session["verifier"].ToString();
                }

            }
        }


        // Get a fresh access token
        protected void BtnRefreshToken_Click(object sender, EventArgs e)
        {
            // Build the HTTP request to obtain a fresh access token
            var request = new RestRequest("OAuth/AccessToken", Method.POST);
            if (oobCheckBox.Checked == false)
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
                string verifier = getVerifier();

                //Use PIN to request access token for users account 

                m_Client.Authenticator = OAuth1Authenticator.ForAccessTokenRefresh(
                    m_ConsumerKey,
                    m_ConsumerSecret,
                    m_oAuthAccessToken,
                    m_oAuthAccessTokenSecret,
                    verifier,
                    m_sessionHandle);
            }


            var response = m_Client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                m_Client = null;
                logOutput.Text += ("</br>Oops! Something went wrong - couldn't get access token from your Autodesk account" + "</br>" + "-----------------------" + "</br>");

                btnGetAccessToken.Enabled = false;
                BtnRefreshToken.Enabled = false;
                btnInvalideToken.Enabled = false;
                txtVerifier.Text = "";
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
                outputString += key + "=" + responseNamedCollection[key] + "</br>";
            }

            //Output response log
            logOutput.Text += ("Refresh Access Token Response:</br>" + outputString + "-----------------------" + "</br>");

        }


        // This button allows you to relinquish the access token. This is the equivalent of
        // logging out.
        protected void btnInvalideToken_Click(object sender, EventArgs e)
        {
            // Build the request to relinquish the access
            // token
            var request = new RestRequest("OAuth/InvalidateToken", Method.POST);
            if (oobCheckBox.Checked == false)
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

                string verifier = getVerifier();

                //Use PIN to request access token for users account 
                m_Client.Authenticator = OAuth1Authenticator.ForAccessTokenRefresh(
                    m_ConsumerKey,
                    m_ConsumerSecret,
                    m_oAuthAccessToken,
                    m_oAuthAccessTokenSecret,
                    verifier,
                    m_sessionHandle);
            }

            // execute the request to relinquish token
            var response = m_Client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                m_Client = null;
                logOutput.Text += ("</br>Oops! Something went wrong - couldn't log out of your Autodesk account" + "</br>" + "-----------------------" + "</br>");

                btnGetAccessToken.Enabled = false;
                BtnRefreshToken.Enabled = false;
                btnInvalideToken.Enabled = false;
                txtVerifier.Text = "";
                return;
            }
            else
            {
                logOutput.Text += ("Logout successful!" + "</br>" + "-----------------------" + "</br>");
            }



            // Disable all controls except the out of band radio button and the Request Token button if the request
            // to relinquish the access token is successful.

            btnGetAccessToken.Enabled = false;
            BtnRefreshToken.Enabled = false;
            btnInvalideToken.Enabled = false;
            txtVerifier.Text = "";


        }

        protected void txtVerifier_TextChanged(object sender, EventArgs e)
        {
            if (txtVerifier.Text.Trim().Length > 0)
            {
                btnGetAccessToken.Enabled = true;
                BtnRefreshToken.Enabled = true;
                btnInvalideToken.Enabled = true;
            }
            else
            {
                btnGetAccessToken.Enabled = false;
                BtnRefreshToken.Enabled = false;
                btnInvalideToken.Enabled = false;
            }
        }

        protected void oobCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (oobCheckBox.Checked)
            {
                verifierPinLabel.Visible = true;
                txtVerifier.Visible = true;
                txtVerifier.Enabled = true;
                lblOobHelp.Visible = true;
                
            }
            else
            {
                verifierPinLabel.Visible = false;
                txtVerifier.Visible = false;
                txtVerifier.Enabled = false;
                lblOobHelp.Visible = false;
            }
        }
    }
}