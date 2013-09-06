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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using Windows.Security.Authentication.Web;
using Windows.Security.Credentials;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace AdnOAuthCSharp
{
    class AdnOAuthConnector
    {
        RestClient _client;

        PasswordVault _vault;

        public AdnOAuthConnector(
            string baseUrl, 
            string consumerKey, 
            string consumerSecret)
        {
             _client = new RestClient(baseUrl);

            ConsumerKey = consumerKey;

            ConsumerSecret = consumerSecret;

            _vault = new PasswordVault();

            _vault.Add(new PasswordCredential(
                "AdnOAuthConnector",
                "AccessToken",
                "-"));

            _vault.Add(new PasswordCredential(
                "AdnOAuthConnector",
                "AccessTokenSecret",
                "-"));
        }

        public string ConsumerKey
        {
            get;
            private set;
        }

        public string ConsumerSecret
        {
            get;
            private set;
        }

        public string AccessToken
        {
            get
            {
                var pwdCreds = _vault.Retrieve(
                    "AdnOAuthConnector",
                    "AccessToken");

                return pwdCreds.Password;
            }

            private set
            {
                var pwdCreds = _vault.Retrieve(
                   "AdnOAuthConnector",
                   "AccessToken");

                _vault.Remove(pwdCreds);

                _vault.Add(new PasswordCredential(
                 "AdnOAuthConnector",
                 "AccessToken",
                 (string.IsNullOrEmpty(value) ? "-" : value)));
            }
        }

        public string AccessTokenSecret
        {
            get
            {
                var pwdCreds = _vault.Retrieve(
                    "AdnOAuthConnector",
                    "AccessTokenSecret");

                return pwdCreds.Password;
            }

            private set
            {
                var pwdCreds = _vault.Retrieve(
                   "AdnOAuthConnector",
                   "AccessTokenSecret");

                _vault.Remove(pwdCreds);

                _vault.Add(new PasswordCredential(
                 "AdnOAuthConnector", 
                 "AccessTokenSecret",
                 (string.IsNullOrEmpty(value) ? "-" : value)));
            }
        }

        // Number of seconds until the access token expires
        public double TokenExpire
        {
            get;
            private set;
        }

        // A token, separate from the access token, 
        // that can be used to acquire a new access token w/o user interaction
        public string SessionHandle
        {
            get;
            private set;
        }

        // Duration that the Consumer is authorized to access
        // When this expires the access token cannot be refreshed 
        // and requires full user authorization to get a new access token.
        public double SessionExpire
        {
            get;
            private set;
        }

        public string UserName
        {
            get;
            private set;
        }

        public string UserGUID
        {
            get;
            private set;
        }

        // Helper method to parse URL response string 
        private Dictionary<string, string> ParseResponse(string response)
        {
            Dictionary<string, string> results = new Dictionary<string, string>();

            string[] elements = response.Split(new char[] { '&' });

            foreach (string element in elements)
            {
                string[] keyValue = element.Split(new char[] { '=' });

                if (keyValue.Length > 1)
                {
                    string decoded = System.Net.WebUtility.UrlDecode(keyValue[1]);

                    results.Add(keyValue[0], decoded);
                }
            }

            return results;
        }

        // Checks if URL response contains "xoauth_problem" tag
        private bool CheckError(Dictionary<string, string> responseMap)
        {
            return responseMap.ContainsKey("xoauth_problem");
        }

        // Connects to OAuth service, returns true if connection successful
        public async Task<bool> DoLogin()
        {
            try
            {
                // The first step of authentication is to request a token
                _client.Authenticator = OAuth1Authenticator.ForRequestToken(
                    ConsumerKey,
                    ConsumerSecret);

                RestRequest requestTokenRequest = new RestRequest
                {
                    Resource = "OAuth/RequestToken",
                    Method = Method.POST,
                };

                Uri requestUri = _client.BuildUri(requestTokenRequest);

                IRestResponse requestTokenResponse =
                    await _client.ExecuteAsync(requestTokenRequest);

                var requestTokenResponseValues =
                    ParseResponse(requestTokenResponse.Content);

                if (CheckError(requestTokenResponseValues))
                {
                    string problem = requestTokenResponseValues["xoauth_problem"];
                    string msg = requestTokenResponseValues["oauth_error_message"];

                    OnError(problem, msg);

                    return false;
                }

                string requestToken = requestTokenResponseValues["oauth_token"];
                string requestTokenSecret = requestTokenResponseValues["oauth_token_secret"];

                // The second step is to authorize the user using the Autodesk login system
                RestRequest authorizeRequest = new RestRequest
                {
                    Resource = "OAuth/Authorize",
                };

                authorizeRequest.AddParameter("oauth_token", requestToken);
                authorizeRequest.AddParameter("viewmode", "mobile");

                Uri authorizeUri = _client.BuildUri(authorizeRequest);

                var callbackUri = new Uri(_client.BaseUrl + "/OAuth/Allow");

                // Custom AuthenticationBroker 
                //var asyncOperation = await AdnWebAuthBroker.AuthenticateAsync(

                // WinRT built-in AuthenticationBroker
                var asyncOperation = await WebAuthenticationBroker.AuthenticateAsync(

                    WebAuthenticationOptions.UseTitle,
                    authorizeUri,
                    callbackUri);

                switch (asyncOperation.ResponseStatus)
                {
                    case WebAuthenticationStatus.ErrorHttp:
                        //An HTTP error like a 404 was returned
                        return false;

                    case WebAuthenticationStatus.Success:
                        break;

                    case WebAuthenticationStatus.UserCancel:
                        return false;

                    default:
                        return false;
                }

                // The third step is to authenticate using the request tokens
                // Once you get the access token and access token secret
                // you need to use those to make your further REST calls
                // Same in case of refreshing the access tokens or invalidating
                // the current session. To do that we need to pass in
                // the acccess token and access token secret as the token and
                // tokenSecret parameter of the OAuth1Authenticator.ForAccessToken
                // constructor

                RestRequest accessTokenRequest = new RestRequest
                {
                    Resource = "OAuth/AccessToken",
                    Method = Method.POST,
                };

                _client.Authenticator = OAuth1Authenticator.ForAccessToken(
                    ConsumerKey,
                    ConsumerSecret,
                    requestToken,
                    requestTokenSecret);

                IRestResponse accessTokenResponse = await _client.ExecuteAsync(
                    accessTokenRequest);

                var accessTokenResponseValues =
                    ParseResponse(accessTokenResponse.Content);

                if (CheckError(accessTokenResponseValues))
                {
                    string problem = accessTokenResponseValues["xoauth_problem"];
                    string msg = accessTokenResponseValues["oauth_error_message"];

                    OnErrorEvent(problem, msg);

                    return false;
                }

                // Parsing reply components

                AccessToken =
                    accessTokenResponseValues["oauth_token"];

                AccessTokenSecret =
                    accessTokenResponseValues["oauth_token_secret"];

                TokenExpire = double.Parse(
                    accessTokenResponseValues["oauth_expires_in"]);

                SessionHandle = 
                    accessTokenResponseValues["oauth_session_handle"];

                SessionExpire = double.Parse(
                    accessTokenResponseValues["oauth_authorization_expires_in"]);

                UserName =
                    accessTokenResponseValues["x_oauth_user_name"];

                UserGUID =
                    accessTokenResponseValues["x_oauth_user_guid"];

                return true;
            }
            catch (Exception ex)
            {
                OnErrorEvent(ex.ToString(), ex.Message);

                return false;
            }
        }

        // To refresh an existing, not expired token, it's similar to 
        // requesting an access token but with the additional
        // session handle retrieved from the initial access token reply
        public async Task<bool> RefreshToken()
        {
            try
            {
                RestRequest accessTokenRequest = new RestRequest
                {
                    Resource = "OAuth/AccessToken",
                    Method = Method.POST,
                };

                _client.Authenticator = OAuth1Authenticator.ForAccessTokenRefresh(
                    ConsumerKey,
                    ConsumerSecret,
                    AccessToken,
                    AccessTokenSecret,
                    SessionHandle);

                IRestResponse accessTokenResponse = await _client.ExecuteAsync(
                    accessTokenRequest);

                var accessTokenResponseValues =
                    ParseResponse(accessTokenResponse.Content);

                if (CheckError(accessTokenResponseValues))
                {
                    string problem = accessTokenResponseValues["xoauth_problem"];
                    string msg = accessTokenResponseValues["oauth_error_message"];

                    OnErrorEvent(problem, msg);

                    return false;
                }

                // Parsing reply components

                AccessToken =
                    accessTokenResponseValues["oauth_token"];

                AccessTokenSecret =
                    accessTokenResponseValues["oauth_token_secret"];

                TokenExpire = double.Parse(
                    accessTokenResponseValues["oauth_expires_in"]);

                SessionHandle = 
                    accessTokenResponseValues["oauth_session_handle"];

                SessionExpire = double.Parse(
                    accessTokenResponseValues["oauth_authorization_expires_in"]);

                UserName =
                    accessTokenResponseValues["x_oauth_user_name"];

                UserGUID =
                    accessTokenResponseValues["x_oauth_user_guid"];

                return true;
            }
            catch (Exception ex)
            {
                OnErrorEvent(ex.ToString(), ex.Message);

                return false;
            }
        }

        // Logout OAuth service, i.e. Invalidate Token
        public async Task<bool> DoLogout()
        {
            try
            {
                RestRequest invalidateRequest = new RestRequest
                {
                    Resource = "OAuth/InvalidateToken",
                };

                _client.Authenticator = OAuth1Authenticator.ForAccessTokenRefresh(
                   ConsumerKey,
                   ConsumerSecret,
                   AccessToken,
                   AccessTokenSecret,
                   SessionHandle);

                IRestResponse accessTokenResponse = await _client.ExecuteAsync(
                    invalidateRequest);

                var invalidateTokenResponseValues =
                    ParseResponse(accessTokenResponse.Content);

                if (CheckError(invalidateTokenResponseValues))
                {
                    string problem = invalidateTokenResponseValues["xoauth_problem"];
                    string msg = invalidateTokenResponseValues["oauth_error_message"];

                    OnErrorEvent(problem, msg);

                    return false;
                }

                // Clear resources

                AccessToken = string.Empty;

                AccessTokenSecret = string.Empty;

                TokenExpire = 0;

                SessionHandle = string.Empty;

                SessionExpire = 0;

                UserName = string.Empty;

                UserGUID = string.Empty;

                return true;
            }
            catch (Exception ex)
            {
                OnErrorEvent(ex.ToString(), ex.Message);

                return false;
            }
        }

        public event OnErrorHandler 
            OnError = null;

        private void OnErrorEvent(string problem, string msg)
        {
            if (OnError != null)
                OnError(problem, msg);
        }
    }

    public delegate void OnErrorHandler(string problem, string msg);
}
