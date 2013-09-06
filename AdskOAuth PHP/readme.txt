 Copyright (c) Autodesk, Inc. All rights reserved 

 PHP Autodesk Oxygen Sample
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
 
 These samples use the stagging Oxygen server to demo the '3 legs' process to authentify
 a user on the Autodesk Cloud infrastructure.
 
 The AdskOAuth.php uses the oauth PHP extension to access the server, whereas the
 AdskOAuthNoExtension.php uses cURL and the Google framework instead.
 
  The PHP Oauth API is documented here:
 http://php.net/manual/en/book.oauth.php

 The '3 legs' process is as follow:
 a- Get a 'request token' from the system
 b- Authorize the received token. Note here that Autodesk currently require you to manual log on Oxygen
      for authorization. This is why the sample is using your default browser for logging.
 c- Get an 'access token' and a session
 
 The samples also do a log-out at the end to complete the sample.

 
 AdskOAuth.php sample
 --------------------------------
 After installing PHP on your system, you may need to install the php_oauth.dll if your 
 distribution does not yet include it. Copy the dll into your PHP extension folder (I.e.: <PHP folder>\ext)
 and add the following lines in your php.ini 
 
   Windows
		[PHP_OAUTH]
		extension=php_oauth.dll
		
   Linux hosts
		[PHP_OAUTH]
		extension=oauth.so

 Getting the PHP oauth extension:
   Windows - You can get precompiled php_oauth.dll for Windows from:
     http://windows.php.net/downloads/pecl/releases/oauth/1.2.3/
   Linux - On Debian host console (or remotely using putty/ssh)
     pecl install oauth
     <may need to install pcre headers (debian based)>
     <apt-get install libpcre3-dev>
     service apache2 restart
   Linux - On Fedora host console (or remotely using putty/ssh)
     pecl install -R /usr/lib/php oauth-0.99.9
     restart apache
   Linux - On CentOS host console (or remotely using putty/ssh)
     pecl install oauth
     server httpd restart
	 

 AdskOAuthNoExtension.php sample
 ---------------------------------------------------
 After installing PHP with cURL support on your system, you need to install the Google PHP OAuth library
 The Google OAuth Client library found here: (http://code.google.com/p/oauth-php/)
 
 You also need to modify the Google OAuth library code like this:
    Distribution: oauth-php-175.tar.gz  
    File oauth\OAuthRequester.php line #285, add
	   return ($token) ;
