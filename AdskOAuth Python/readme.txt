 Copyright (c) Autodesk, Inc. All rights reserved 

 Python Autodesk Oxygen Sample
 by Cyrille Fauvel - Autodesk Developer Network (ADN)
 July 2013

 Permission to use, copy, modify, and distribute this software in
 object code form for any purpose and without fee is hereby granted, 
 provided that the above copyright notice appears in all copies and 
 that both that copyright notice and the limited warranty and
 restricted rights notice below appear in all supporting 
 documentation.

 AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
 AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
 MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC. 
 DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
 UNINTERRUPTED OR ERROR FREE.
 
 This sample uses the stagging Oxygen server to demo the '3 legs' process to authentify
 a user on the Autodesk Cloud infrastructure.
 
 After installing Python on your system, you may need to install the 'httplib2' and 'oauth2' libraries
 if your distribution does not yet include them. Copy the libraies into your Python 'site-packages' folder
 or use the installer coming with the libray distributions.

 You can get the httplib2 library from:
 https://code.google.com/p/httplib2/

 You can get the oauth2 library from:
 https://github.com/simplegeo/python-oauth2

 The '3 legs' process is as follow:
 a- Get a 'request token' from the system
 b- Authorize the received token. Note here that Autodesk currently require you to manual log on Oxygen
      for authorization. This is why the sample is using your default browser for logging.
 c- Get an 'access token' and a session
 
 The sample also does a log-out at the end to complete the sample.
