////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved 
// Written by Gopinath Taget 2013 - ADN/Developer Technical Services
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

This sample illustrate how to connect to Autodesk OAuth webservice for the 
from an application running inside AutoCAD.

In the samples, you will need to replace the following place holders with 
appropriate values and credentials:

        private string BaseUrl = "https://your.oauth.server.here";
        private string ConsumerKey = "your.consumer.key.here";
        private string ConsumerSecret = "your.comsumer.secret.key.here";

CommandClass.cs:
        This file implements a simple command that displays a window modelessly.

OAuthProcess.xaml.cs: 
        This file implements the main window of the application.
        It contains all the code that works with the OAuth Provider.It uses the 
        RestSharp API in order to perform REST requests and OAuth authetication. You will 
        find information on the the RestSharp API here: http://restsharp.org/.
	It's available here for download: https://github.com/restsharp/RestSharp. 
        If you are using Visual Studio, you can use the NuGet Package Manager to 
        download and install the latest version of the RestSharp API into your 
        solution: http://www.nuget.org/

BrowserAuthenticate.xaml.cs:
        This file implements a browser window containing a browser control used to Authorize
        a request token in the in band mode.

