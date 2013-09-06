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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace AdnOAuthCSharp
{
    public sealed class AdnWebAuthBroker
    {
        private static async Task<AdnWebAuthenticationResult> AuthenticateAsyncTask(
            WebAuthenticationOptions options,
            Uri startUri,
            Uri endUri)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();

            WebAuthenticationStatus responseStatus = WebAuthenticationStatus.Success;

            string responseData = "";

            Popup popUp = new Popup
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,

                Width = 800,
                Height = Window.Current.Bounds.Height
            };

            var webAuthView = new AdnWebAuthView
            {
                Width = Window.Current.Bounds.Width,
                Height = Window.Current.Bounds.Height
            };

            webAuthView.CancelledEvent += (s, e) =>
            {
                responseStatus = WebAuthenticationStatus.UserCancel;

                popUp.IsOpen = false;

                tcs.TrySetResult(1);
            };

            webAuthView.UriChangedEvent += (s, e) =>
            {
                if (e != null)
                {
                    if (e.Uri.AbsoluteUri.StartsWith(
                        endUri.AbsoluteUri,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        responseStatus = WebAuthenticationStatus.Success;
                        responseData = e.Uri.AbsoluteUri;

                        popUp.IsOpen = false;

                        tcs.TrySetResult(1);
                    }
                }
            };

            webAuthView.NavFailedEvent += (s, e) =>
            {
                if (e.Uri.AbsoluteUri.StartsWith(
                    endUri.AbsoluteUri,
                    StringComparison.OrdinalIgnoreCase))
                {
                    responseStatus = WebAuthenticationStatus.Success;
                    responseData = e.Uri.AbsoluteUri;
                }
                else
                {
                    responseStatus = WebAuthenticationStatus.ErrorHttp;
                    responseData = e.Uri.AbsoluteUri;
                }

                popUp.IsOpen = false;

                tcs.TrySetResult(1);
            };

            popUp.Child = webAuthView;
            popUp.IsOpen = true;
            webAuthView.Navigate(startUri);

            await tcs.Task;

            return new AdnWebAuthenticationResult
            {
                ResponseStatus = responseStatus,
                ResponseData = responseData
            };
        }

        public static IAsyncOperation<AdnWebAuthenticationResult> AuthenticateAsync(
            WebAuthenticationOptions options,
            Uri startUri,
            Uri endUri)
        {
            return AuthenticateAsyncTask(options, startUri, endUri).AsAsyncOperation();
        }
    }

    public sealed class AdnWebAuthenticationResult
    {
        public string ResponseData
        {
            get;
            set;
        }

        public WebAuthenticationStatus ResponseStatus
        {
            get;
            set;
        }
    }
}
