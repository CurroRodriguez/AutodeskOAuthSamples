////////////////////////////////////////////////////////////////////////////////
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

Those samples illustrate how to connect to Autodesk OAuth webservice for the 
WinRT platform.

In the samples, you will need to replace the following place holders with 
appropriate values and credentials:

        private string BaseUrl = "https://your.oauth.server.here";
        private string ConsumerKey = "your.consumer.key.here";
        private string ConsumerSecret = "your.comsumer.secret.key.here";


I - AdskOAuthCSharp: a C#/XAML version for WinRT

	This C# sample is using a WinRT specific version of the RestSharp API
	in order to perform REST requests and OAuth authetication.
	It's available here: https://github.com/devinrader/RestSharp

II - AdskOAuthJs: a JavaScript/HTML5 version for WinRT

	This JavaScript sample is using a library provided by John Kristian 
	that you can find here: http://oauth.googlecode.com/svn/code/javascript/









