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

function AdnOAuthConnector(
    baseUrl, 
    consumerKey, 
    consumerSecret) {

    //Private members
    _baseUrl = baseUrl;
    _consumerKey = consumerKey;
    _consumerSecret = consumerSecret;

    _requestToken = '';
    _requestTokenSecret = '';

    _accessToken = '';
    _accessTokenSecret = '';

    _tokenExpire = 0.0;
    _sessionHandle = '';
    _sessionExpire = 0.0;

    _userName = '';
    _userGUID = '';

    ////////////////////////////////////////////////////////////////////////////////////
    // Getters
    ////////////////////////////////////////////////////////////////////////////////////
    this.getAccessToken = function () {
        return _accessToken;
    }

    this.getAccessTokenSecret = function () {
        return _accessTokenSecret;
    }

    this.getTokenExpire = function () {
        return _tokenExpire;
    }

    this.getSessionHandle = function () {
        return _sessionHandle;
    }

    this.getSessionExpire = function () {
        return _sessionExpire;
    }

    this.getUserName = function () {
        return _userName;
    }

    this.getUserGUID = function () {
        return _userGUID;
    }
       
    ////////////////////////////////////////////////////////////////////////////////////
    // OAuth Log In
    ////////////////////////////////////////////////////////////////////////////////////
    this.doLogin = function (onSuccess, onError) {

        var onAccessSuccess = function (responseParams) {

            _accessToken = responseParams['oauth_token'];
            _accessTokenSecret = responseParams['oauth_token_secret'];

            _tokenExpire = responseParams['oauth_expires_in'];
            _sessionHandle = responseParams['oauth_session_handle'];
            _sessionExpire = responseParams['oauth_authorization_expires_in'];

            _userName = responseParams['x_oauth_user_name'];
            _userGUID = responseParams['x_oauth_user_guid'];

            onSuccess();
        }

        // The third step is to authenticate using the request tokens
        // Once you get the access token and access token secret
        // you need to use those to make your further REST calls
        // Same in case of refreshing the access tokens or invalidating
        // the current session. 

        var onAuthorizeSuccess = function () {

            getAccessToken(
                _baseUrl + '/OAuth/AccessToken',
                _consumerKey,
                _consumerSecret,
                _requestToken,
                _requestTokenSecret,
                onAccessSuccess,
                onError);
        }

        // The second step is to authorize the user using the Autodesk login system
        var onRequestSuccess = function (responseParams) {

            _requestToken = responseParams['oauth_token'];
            _requestTokenSecret = responseParams['oauth_token_secret'];

            getAuthorized(
                _baseUrl + '/OAuth/Authorize',
                _baseUrl + '/OAuth/Allow',
                _requestToken,
                onAuthorizeSuccess,
                onError);
        }

        // The first step of authentication is to request a token
        getRequestToken(
          _baseUrl + '/OAuth/RequestToken',
          _consumerKey,
          _consumerSecret,
          onRequestSuccess,
          onError); 
    }

    ////////////////////////////////////////////////////////////////////////////////////
    // OAuth Token Refresh
    // To refresh an existing, not expired token, it's similar to 
    // requesting an access token but with the additional
    // session handle retrieved from the initial access token reply
    //
    ////////////////////////////////////////////////////////////////////////////////////
    this.doRefresh = function (onSuccess, onError) {

        var accessor = {
            consumerKey: _consumerKey,
            consumerSecret: _consumerSecret,
            tokenSecret: _accessTokenSecret
        };

        var message = {
            action: _baseUrl + '/OAuth/AccessToken',
            method: "GET",
            parameters: {
                oauth_consumer_key: _consumerKey,
                oauth_token: _accessToken,
                oauth_signature_method: "HMAC-SHA1",
                oauth_session_handle: _sessionHandle
            }
        };

        OAuth.setTimestampAndNonce(message);
        OAuth.SignatureMethod.sign(message, accessor);

        var url = message.action + "?" + OAuth.formEncode(message.parameters);

        WinJS.xhr({
            url: url
        }).then(
            function completed(result) {

                var responseParams = getQueryStringParams(result.responseText);

                if (responseParams['xoauth_problem'] !== undefined) {
                    onError(responseParams);
                }
                else {

                    _accessToken = responseParams['oauth_token'];
                    _accessTokenSecret = responseParams['oauth_token_secret'];

                    _tokenExpire = responseParams['oauth_expires_in'];
                    _sessionHandle = responseParams['oauth_session_handle'];
                    _sessionExpire = responseParams['oauth_authorization_expires_in'];

                    _userName = responseParams['x_oauth_user_name'];
                    _userGUID = responseParams['x_oauth_user_guid'];

                    onSuccess();
                }
            },
            function error(err) {
                var responseParams = getQueryStringParams(err.responseText);
                onError(responseParams);
            });
    }

    ////////////////////////////////////////////////////////////////////////////////////
    // OAuth Token Refresh
    ////////////////////////////////////////////////////////////////////////////////////
    this.doLogout = function (onError) {

        var accessor = {
            consumerKey: _consumerKey,
            consumerSecret: _consumerSecret,
            tokenSecret: _accessTokenSecret
        };

        var message = {
            action: _baseUrl + '/OAuth/InvalidateToken',
            method: "GET",
            parameters: {
                oauth_consumer_key: _consumerKey,
                oauth_token: _accessToken,
                oauth_signature_method: "HMAC-SHA1",
                oauth_session_handle: _sessionHandle
            }
        };

        OAuth.setTimestampAndNonce(message);
        OAuth.SignatureMethod.sign(message, accessor);

        var url = message.action + "?" + OAuth.formEncode(message.parameters);

        WinJS.xhr({
            url: url
        }).then(
            function completed(result) {

                var responseParams = getQueryStringParams(result.responseText);

                if (responseParams['xoauth_problem'] !== undefined) {
                    onError(responseParams);
                }
            },
            function error(err) {
                var responseParams = getQueryStringParams(err.responseText);
                onError(responseParams);
            });
    }

    ////////////////////////////////////////////////////////////////////////////////////
    // Parse url reply
    ////////////////////////////////////////////////////////////////////////////////////
    var getQueryStringParams = function (url) {

        var params = url.split('&');

        if (params === "") return {};

        var map = {};

        for (var i = 0; i < params.length; ++i) {

            var param = params[i].split('=', 2);

            map[param[0]] = decodeURIComponent(param[1]);
        }

        return map;
    }

    ////////////////////////////////////////////////////////////////////////////////////
    // Get request token query
    ////////////////////////////////////////////////////////////////////////////////////
    var getRequestToken = function (
        requestUrl,
        consumerKey,
        consumerSecret,
        onSuccess,
        onError) {

        var accessor = {
            consumerKey: consumerKey,
            consumerSecret: consumerSecret,
            tokenSecret: ''
        };

        var message = {
            action: requestUrl,
            method: "GET",
            parameters: {
                oauth_consumer_key: consumerKey,
                oauth_signature_method: 'HMAC-SHA1'
                //oauth_callback: 'http://callbackurl'
            }
        };

        OAuth.setTimestampAndNonce(message);
        OAuth.SignatureMethod.sign(message, accessor);

        var url = message.action + "?" + OAuth.formEncode(message.parameters);

        WinJS.xhr({
            url: url
        }).then(
        function completed(result) {

            var responseParams = getQueryStringParams(result.responseText);

            if (responseParams['xoauth_problem'] !== undefined) {
                onError(responseParams);
            }
            else {
                onSuccess(responseParams);
            }
        },
        function error(err) {
            var responseParams = getQueryStringParams(err.responseText);
            onError(responseParams);
        });
    }

    ////////////////////////////////////////////////////////////////////////////////////
    // viewmode: [full, iframe, desktop, mobile]
    //
    ////////////////////////////////////////////////////////////////////////////////////
    var getAuthorized = function (
        authorizeUrl,
        allowedUrl,
        token,
        onSuccess,
        onError) {

        var loginUrl = authorizeUrl + "?oauth_token=" + encodeURIComponent(token) + "&viewmode=mobile";

        Windows.Security.Authentication.Web.WebAuthenticationBroker.authenticateAsync(
            Windows.Security.Authentication.Web.WebAuthenticationOptions.useTitle,
            new Windows.Foundation.Uri(loginUrl),
            new Windows.Foundation.Uri(allowedUrl)).done(

            function completed(result) {

                if (result) {
                    if (result.responseData !== "") {
                        onSuccess();
                    }
                }
            },
            function error(err) {
                onError(err);
            });
    }

    ////////////////////////////////////////////////////////////////////////////////////
    // Get access token query
    ////////////////////////////////////////////////////////////////////////////////////
    var getAccessToken = function (
        accessUrl,
        consumerKey,
        consumerSecret,
        token,
        tokenSecret,
        onSuccess,
        onError) {

        var accessor = {
            consumerKey: consumerKey,
            consumerSecret: consumerSecret,
            tokenSecret: tokenSecret
        };

        var message = {
            action: accessUrl,
            method: "GET",
            parameters: {
                oauth_consumer_key: consumerKey,
                oauth_token: token,
                oauth_signature_method: "HMAC-SHA1"
                //oauth_verifier: verifier,
                //oauth_callback: 'http://callbackurl'
            }
        };

        OAuth.setTimestampAndNonce(message);
        OAuth.SignatureMethod.sign(message, accessor);

        var url = message.action + "?" + OAuth.formEncode(message.parameters);

        WinJS.xhr({
            url: url
        }).then(
            function completed(result) {

                var responseParams = getQueryStringParams(result.responseText);

                if (responseParams['xoauth_problem'] !== undefined) {
                    onError(responseParams);
                }
                else {
                    onSuccess(responseParams);
                }
            },
            function error(err) {
                var responseParams = getQueryStringParams(err.responseText);
                onError(responseParams);
            });
    }   
}




