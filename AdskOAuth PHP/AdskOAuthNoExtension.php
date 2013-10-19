<?php
/*
 Copyright (c) Autodesk, Inc. All rights reserved 

 PHP Autodesk Oxygen Sample
 by Cyrille Fauvel - Autodesk Developer Network (ADN)
 August 2013

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
 
 After installing PHP on your system, you need to install the Google PHP OAuth library
 The Google OAuth Client library found here: (http://code.google.com/p/oauth-php/)
 
 You also need to modify the Google OAuth library code like this:
    file oauth\OAuthRequester.php line #285, add
	   return ($token) ;

 The PHP Oauth API is documented here:
 http://php.net/manual/en/book.oauth.php

 The '3 legs' process is as follow:
 a- Get a 'request token' from the system
 b- Authorize the received token. Note here that Autodesk currently require you to manual log on Oxygen
      for authorization. This is why the sample is using your default browser for logging.
 c- Get an 'access token' and a session
 
 The sample also does a log-out at the end to complete the sample.
 
 In case you want to use Fiddler to debug the HTTP request, you can add
 CURLOPT_PROXY => '127.0.0.1:8888'
 to the curl options.
 But you would also need to modify the library code tocope with the Fiddler header.
 Modify oauth/OAuthRequester.php
  a- requestRequestToken() line # 161
  b- requestAccessToken() line # 248
 with this
 	$text	= $oauth->curl_raw($curl_options);
if ( isset ($curl_options [CURLOPT_PROXY]) ) {
	$text =substr ($text, 4) ;
	$text =strstr ($text, 'HTTP') ;
}		
	if (empty($text))

*/

include_once "vendor/oauth/OAuthStore.php" ;
include_once "vendor/oauth/OAuthRequester.php" ;

define ('DefaultBrowser' ,'"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" ') ;

define ('ConsumerKey', 'Your Consumer Key') ;
define ('ConsumerSecret', 'Your Consumer Secret Key') ;
define ('BaseUrl', 'The Oauth server URL') ; // https://accounts.autodesk.com/ or https://accounts-staging.autodesk.com/

define ('OAUTH_TMP_DIR', function_exists ('sys_get_temp_dir') ? sys_get_temp_dir () : realpath ($_ENV ["TMP"])) ;

//- Prepare the PHP OAuth for consuming our Oxygen service
$options =array (
	'consumer_key' => ConsumerKey,
	'consumer_secret' => ConsumerSecret,
	'server_uri' => BaseUrl,
	'request_token_uri' => BaseUrl . 'OAuth/RequestToken',
	'authorize_uri' => BaseUrl . 'OAuth/Authorize',
	'access_token_uri' => BaseUrl . 'OAuth/AccessToken',
) ;
OAuthStore::instance ('Session', $options) ;

//- To disable the SSL check to avoid an exception with invalidate certificate on the server,
//- use the cURL CURLOPT_SSL_VERIFYPEER option and set it to false.

//- 1st leg: Get the 'request token'
$token ='' ;
try {
	$token_ =OAuthRequester::requestRequestToken (ConsumerKey, 0, array (), 'POST', $options, array ( CURLOPT_SSL_VERIFYPEER => 0, )) ;
	$tmpRes =OAuthStore::instance ()->getSecretsForSignature ('', 0) ;
	$token_secret =$tmpRes ['token_secret'] ;
	$token =array (
		'oauth_token' => $token_ ['token'],
		'oauth_token_secret' => $token_secret,
	) ;
	echo "OAuth/RequestToken response\n",
		"\toauth_token: {$token ['oauth_token']}\n",
		"\toauth_token_secret: {$token ['oauth_token_secret']}\n",
		"-----------------------\n\n" ;
	//- The following line is for reference only, the call to OAuthRequester::requestRequestToken() includes that step
	//OAuthStore::instance ()->addServerToken (ConsumerKey, 'request', $token ['oauth_token'], $token ['oauth_token_secret'], 0, array ()) ;
} catch (Exception $e) {
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
	//- The following line is for reference only, the call to OAuthRequester::requestRequestToken() included that step. But you may need
	//- to call that line with the correct parameters in you run that 'leg' in a different PHP session.
	//OAuthStore::instance ()->addServerToken (ConsumerKey, 'request', $token ['oauth_token'], $token ['oauth_token_secret'], 0, array ()) ;
	
	//- $access will contain the server response in case you modified the Google library like documented above
	$access =OAuthRequester::requestAccessToken (ConsumerKey, $token ['oauth_token'], 0, 'POST', $options, array ( CURLOPT_SSL_VERIFYPEER => 0, )) ;
	//- If you did not modify OAuthRequester::requestAccessToken() function as documented above, do this instead
	// $access =array (
	// 	'oauth_token' => OAuthStore::instance ()->getSecretsForSignature ('', 0) ['token'],
	// 	'oauth_token_secret' => OAuthStore::instance ()->getSecretsForSignature ('', 0) ['token_secret'],
	// ) ;
	echo "OAuth/AccessToken response\n",
		"\toauth_token: {$access ['oauth_token']}\n",
		"\toauth_token_secret: {$access ['oauth_token_secret']}\n",
		"\toauth_session_handle: {$access ['oauth_session_handle']}\n",
		"\toauth_authorization_expires_in: {$access ['oauth_authorization_expires_in']}\n",
		"\toauth_expires_in: {$access ['oauth_expires_in']}\n",
		"\tx_oauth_user_name: {$access ['x_oauth_user_name']}\n",
		"\tx_oauth_user_guid: {$access ['x_oauth_user_guid']}\n",
		"-----------------------\n\n" ;
} catch (Exception $e) {
	echo "OAuth/AccessToken\n", 'Caught exception: ',  $e->getMessage (), "\n";
	exit ;
}

//- To refresh the 'Access token' before it expires, just call again
	/*OAuthStore::instance ()->addServerToken (ConsumerKey, 'access', $access ['oauth_token'], $access ['oauth_token_secret'], 0, $options) ;
	$request =new OAuthRequester ($options ['access_token_uri'], 'POST', $access) ;
	$ret =$request->doRequest (0, array ( CURLOPT_SSL_VERIFYPEER => 0, )) ;
	$ret =explode ('&', $ret ['body']) ;
	foreach ( $ret as $key => $value ) {
		$entry =explode ('=', $value) ;
		$access [$entry [0]] =rawurldecode ($entry [1]) ;
	}*/
// and save the access token again


//- Do you other work HERE


//- Once you are done, log-out
try {
	//OAuthStore::instance ('Session', $options) ;
	
	//- Change the command as the Google API does not implement that one
	$options ['access_token_uri'] =BaseUrl . 'OAuth/InvalidateToken' ;

	OAuthStore::instance ()->addServerToken (ConsumerKey, 'access', $access ['oauth_token'], $access ['oauth_token_secret'], 0, $options) ;
	$request =new OAuthRequester ($options ['access_token_uri'], 'GET', $access) ;
	$access =$request->doRequest (0, array ( CURLOPT_SSL_VERIFYPEER => 0, )) ;
	
	echo "You logged out!\n" ;
} catch (Exception $e) {
	echo "OAuth/InvalidateToken\n", 'Caught exception: ',  $e->getMessage (), "\n";
	exit ;
}

//- Done
exit ;
