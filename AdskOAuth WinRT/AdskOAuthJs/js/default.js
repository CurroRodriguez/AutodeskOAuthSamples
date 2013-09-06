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

(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;

    //var baseUrl = 'https://your.oauth.server.here';
    //var consumerKey = 'your.consumer.key.here';
    //var consumerSecret = 'your.comsumer.secret.key.here';

    var baseUrl = 'https://accounts-staging.autodesk.com';
    var consumerKey = 'mycloud-staging.autodesk.com';
    var consumerSecret = 'Secret123';

    var connector;

    app.onactivated = function (args) {

        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.         

            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }

            args.setPromise(WinJS.UI.processAll());

            document.addEventListener("keypress", app.onKeypress, false);

            //Initialize AppBar
            document.getElementById("cmdLogin").addEventListener("click", login, false);
            document.getElementById("cmdRefresh").addEventListener("click", refresh, false);
            document.getElementById("cmdLogout").addEventListener("click", logout, false);

            var appBar = document.getElementById("AppBar").winControl;

            appBar.hideCommands(['cmdRefresh', 'cmdSeparator', 'cmdLogout'], true);
            
            clearInfos();
        }
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };

    app.onKeypress = function (e) {
        if (e.keyCode === 27) { //Escape
            window.MSApp.terminateApp(e);
        }
    };

    app.start();

    function showErrorDialog(title, problem, message) {

        var msg = 'Problem: ' + problem + '\n' + 'Message:' + message;
        var dlg = new Windows.UI.Popups.MessageDialog(msg, title);

        dlg.commands.append(new Windows.UI.Popups.UICommand('OK'));
        dlg.showAsync();
    }

    function onError(responseParams) {

        showErrorDialog(
            'OAuth Login Failed...',
            responseParams['xoauth_problem'],
            responseParams['oauth_error_message']);
    }

    function onSuccess() {

        var token = connector.getAccessToken();
        var tokenSecret = connector.getAccessTokenSecret();

        var appBar = document.getElementById("AppBar").winControl;

        appBar.showCommands(['cmdRefresh', 'cmdSeparator', 'cmdLogout'], true);
        appBar.hideCommands(['cmdLogin'], true);

        // Fills up info listview
        var listView = document.getElementById('infoList').winControl;

        var data = new WinJS.Binding.List([
            { name: "Status: ", value: "Logged in!" },
            { name: "User: ", value: connector.getUserName()},
            { name: "Access Token: ", value: connector.getAccessToken() },
            { name: "Access Token Secret: ", value: connector.getAccessTokenSecret() },
            { name: "Session Handle: ", value: connector.getSessionHandle() },
            { name: "Token Expires: ", value: secToTime(connector.getTokenExpire()) },
            { name: "Session Expires: ", value: secToTime(connector.getSessionExpire()) }
        ]);

        listView.itemDataSource = data.dataSource;
    }

    function clearInfos() {
      
        var listView = document.getElementById('infoList').winControl;

        var data = new WinJS.Binding.List([
            { name: "Status: ", value: "Logged out..." }
        ]);

        listView.itemDataSource = data.dataSource;
    }

    function secToTime(strTimeInSec) {

        var val = parseInt(strTimeInSec);

        var days = val / 86400 | 0;
        var hrs = (val % 86400) / 3600 | 0;
        var mins = (val % 3600) / 60 | 0;
        var secs = Math.floor (val % 60);

        return days + ' day' + (days > 1 ? 's ' : ' ') + z(hrs) + 'h:' + z(mins) + 'm:' + z(secs) +'s';

        function z(n) { return (n < 10 ? '0' : '') + n; }
    }

    ////////////////////////////////////////////////////////////////////////////////////
    // AppBar Handlers
    //
    ////////////////////////////////////////////////////////////////////////////////////
    function login() {
        connector = new AdnOAuthConnector(baseUrl, consumerKey, consumerSecret);
        connector.doLogin(onSuccess, onError);
    }

    function refresh() {
        if (connector !== undefined) {
            connector.doRefresh(onSuccess, onError);
        }
    }

    function logout() {
        if (connector !== undefined) {
            connector.doLogout(onError);

            var appBar = document.getElementById("AppBar").winControl;

            appBar.hideCommands(['cmdRefresh', 'cmdSeparator', 'cmdLogout'], false);
            appBar.showCommands(['cmdLogin'], true);

            clearInfos();
        }
    }

})();
