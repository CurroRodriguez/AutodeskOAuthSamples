////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved 
// Written by Daniel Du 2013 - ADN/Developer Technical Services
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
from a ASP.NET web form application.

How to run this sample: 

1. Open AdskOAuthAspNet.sln in Visual Studio 2012
2. Replace the necessory place holders in code
2. Menu "Build" - "Clean Solution", then "Build" - "Rebuild Solution"

To run this sample, your will need to use Google Chrome or Firefox,
It does not work well on Internet Explorer due to some issues.

In the samples, you will need to replace the following place holders with 
appropriate values and credentials:

        private const string m_ConsumerKey = "your.consumer.key.here";
        private const string m_ConsumerSecret = "your.comsumer.secret.key.here";
        private const string m_baseURL = "https://your.oauth.server.here";

You will also need to change the callback URL according to your own test
enviroment:
        private const string m_CallbackURL = "http://localhost:<port>/callback.aspx";

How do I know my port number?

You can just open default.aspx, and press F5 to lunch a broswer,  you will 
get an url similar like "http://localhost:25574/default.aspx"
if there is no complier error.  For me 25574 is my port number. Yours may be different
with mine, please update the port number to following code and run the sample again.
private const string m_CallbackURL = "http://localhost:<port>/callback.aspx";


default.aspx.cs: 
        This file implements the main window of the application.
        It contains all the code that works with the OAuth Provider.It uses the 
        RestSharp API in order to perform REST requests and OAuth authetication. You will 
        find information on the the RestSharp API here: http://restsharp.org/.
	It's available here for download: https://github.com/restsharp/RestSharp. 
        If you are using Visual Studio, you can use the NuGet Package Manager to 
        download and install the latest version of the RestSharp API into your 
        solution: http://www.nuget.org/

callback.aspx.cs:
        This file is the callback url to get the verifier code when using in-band.



