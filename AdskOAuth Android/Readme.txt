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

This sample illustrates how to connect to Autodesk OAuth webservice 
from an Android application

This sample uses a simplified scribe-java library, at the same time, extend the library 
to support refreshing and invalidating access token.

https://github.com/fernandezpablo85/scribe-java

How to run this sample
1. Open Eclipes,  File -> import, Android -> Existing Android Code into Workspace, set 
	the root directory to the folder of source code

2. Right click the project and select "Run As" -> Android appliocation, run it in a emulator
  or a real devices.

3. Input your correct consumer key and secret key in the first UI, then tap "Authenticate" button to begin.

Source code structure
  com.autodesk.adn.common -- Some utility classes
  com.autodesk.adn.oauthsampleandroid  -- Activity related classes
  
  com.google.* --
  org.scribe.* --  Third party library
 
You may focus on  com.autodesk.adn.oauthsampleandroid and com.autodesk.adn.common for sample of 
using Autodesk OAuth provider for authentication.
  
Also refer to org.scribe.oauth -> OAuth10aServiceImpl.java for the implementation of refreshing/invalidating access token:
  public Token refreshToken(Token expiredToken, String sessionHandle)
  public boolean invalidateToken(Token accessToken, String sessionHandle)
   
	

