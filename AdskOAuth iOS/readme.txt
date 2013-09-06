////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved 
// Written by Adam Nagy 2013 - ADN/Developer Technical Services
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

This sample illustrates how to connect to Autodesk OAuth web service from an
iOS device. The project is using the OAuth library from 
https://github.com/tweetdeck/TDOAuth
The only thing needed to be modified in it was the function 
"URLRequestForPath/POSTParameters" so that I could add 
"oauth_session_handle" to the "Authorization" header of the POST message

In the sample, you will need to replace the following place holders inside
ViewController.m with the appropriate values:

#define O2_OAUTH_KEY     @"your.consumer.key.here"
#define O2_OAUTH_SECRET  @"your.comsumer.secret.key.here"
#define O2_HOST          @"your.oauth.server.here"
