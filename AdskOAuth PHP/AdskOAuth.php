<?php
/*
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
 
 This sample uses the staging Oxygen server to demo the '3 legs' process to authentify
 a user on the Autodesk Cloud infrastructure.
 
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
     
 The PHP Oauth API is documented here:
 http://php.net/manual/en/book.oauth.php

 The '3 legs' process is as follow:
 a- Get a 'request token' from the system
 b- Authorize the received token. Note here that Autodesk currently require you to manual log on Oxygen
      for authorization. This is why the sample is using your default browser for logging.
 c- Get an 'access token' and a session
 
 The sample also does a log-out at the end to complete the sample.
 
*/

define ('DefaultBrowser' ,'"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" ') ;

define ('ConsumerKey', 'Your Consumer Key') ;
define ('ConsumerSecret', 'Your Consumer Secret Key') ;
define ('BaseUrl', 'The Oauth server URL') ; // https://accounts.autodesk.com/ or https://accounts-staging.autodesk.com/

//- Prepare the PHP OAuth for consuming our Oxygen service
//- Disable the SSL check to avoid an exception with invalidate certificate on the server
$oauth =new OAuth (ConsumerKey, ConsumerSecret, OAUTH_SIG_METHOD_HMACSHA1, OAUTH_AUTH_TYPE_URI) ;
$oauth->enableDebug () ;
$oauth->disableSSLChecks () ;

//- 1st leg: Get the 'request token'
$token ='' ;
try {
	$token =$oauth->getRequestToken (BaseUrl . "OAuth/RequestToken") ;
	echo "OAuth/RequestToken response\n",
		"\toauth_token: {$token ['oauth_token']}\n",
		"\toauth_token_secret: {$token ['oauth_token_secret']}\n",
		"-----------------------\n\n" ;
	//- Set the token and secret for subsequent requests.
	$oauth->setToken ($token ['oauth_token'], $token ['oauth_token_secret']) ;
} catch (OAuthException $e) {
	echo "OAuth/RequestToken\n", 'Caught exception: ',  $e->getMessage (), "\n";
	exit ;
}

//- 2nd leg: Authorize the token
//- Currently, Autodesk Oxygen service requires you to manually log into the system, so we are using your default browser
try {
	$url =BaseUrl . "OAuth/Authorize" . "?oauth_token=" . urlencode (stripslashes ($token ['oauth_token'])) ;
	exec (DefaultBrowser . $url) ;
	//- We need to wait for the user to have logged in
	echo "Press [Enter] when logged" ;
	$psLine =fgets (STDIN, 1024) ;
} catch (Exception $e) {
	echo "OAuth/Authorize\n", 'Caught exception: ',  $e->getMessage (), "\n";
	exit ;
}

//- 3rd leg: Get the 'access token' and session
$access ='' ;
try {
	$access =$oauth->getAccessToken (BaseUrl . "OAuth/AccessToken") ;
	echo "OAuth/AccessToken response\n",
		"\toauth_token: {$access ['oauth_token']}\n",
		"\toauth_token_secret: {$access ['oauth_token_secret']}\n",
		"\toauth_session_handle: {$access ['oauth_session_handle']}\n",
		"\toauth_authorization_expires_in: {$access ['oauth_authorization_expires_in']}\n",
		"\toauth_expires_in: {$access ['oauth_expires_in']}\n",
		"\tx_oauth_user_name: {$access ['x_oauth_user_name']}\n",
		"\tx_oauth_user_guid: {$access ['x_oauth_user_guid']}\n",
		"-----------------------\n\n" ;
	//- Set the token and secret for subsequent requests.
	$oauth->setToken ($access ['oauth_token'], $access ['oauth_token_secret']) ;
} catch (OAuthException $e) {
	echo "OAuth/AccessToken\n", 'Caught exception: ',  $e->getMessage (), "\n";
	exit ;
}

//- To refresh the 'Access token' before it expires, just call again
//- $access =$oauth->getAccessToken (BaseUrl . "OAuth/AccessToken") ;
//- Note that at this time the 'Access token' never expires

//- Do you other work HERE

//- Once you are done, log-out
try {
	$access =$oauth->getAccessToken (BaseUrl . "OAuth/InvalidateToken" . "?oauth_session_handle=" . urlencode (stripslashes ($access ['oauth_session_handle']))) ;
	echo "You logged out!\n" ;
	//- Clear the token and secret for subsequent requests.
	$oauth->setToken ('', '') ;
} catch (OAuthException $e) {
	echo "OAuth/InvalidateToken\n", 'Caught exception: ',  $e->getMessage (), "\n";
	exit ;
}

//- Done
exit ;
