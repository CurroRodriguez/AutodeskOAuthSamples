<?php
/*
 Copyright (c) Autodesk, Inc. All rights reserved 

 PHP Autodesk Oxygen WEB Sample
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
 
*/

include_once "vendor/oauth/OAuthStore.php" ;
include_once "vendor/oauth/OAuthRequester.php" ;

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

$fname =realpath (dirname (__FILE__)) . '/token.txt' ;
$token =unserialize (file_get_contents ($fname)) ;
unlink ($fname) ;

//- To disable the SSL check to avoid an exception with invalidate certificate on the server,
//- use the cURL CURLOPT_SSL_VERIFYPEER option and set it to false.

//- 3rd leg: Get the 'access token' and session
$access ='' ;
try {
	//- The following line is for reference only, the call to OAuthRequester::requestRequestToken() included that step. But you may need
	//- to call that line with the correct parameters in you run that 'leg' in a different PHP session.
	OAuthStore::instance ()->addServerToken (ConsumerKey, 'request', $token ['oauth_token'], $token ['oauth_token_secret'], 0, array ()) ;
	
	//- $access will contain the server response in case you modified the Google library like documented above
	$access =OAuthRequester::requestAccessToken (ConsumerKey, $token ['oauth_token'], 0, 'POST', $options, array ( CURLOPT_SSL_VERIFYPEER => 0, )) ;
	//- If you did not modify OAuthRequester::requestAccessToken() function as documented above, do this instead
	// $access =array (
	// 	'oauth_token' => OAuthStore::instance ()->getSecretsForSignature ('', 0) ['token'],
	// 	'oauth_token_secret' => OAuthStore::instance ()->getSecretsForSignature ('', 0) ['token_secret'],
	// ) ;
	
	/*
	define ('OAUTH_HOST', 'http://' . $_SERVER ['SERVER_NAME']) ;
	$request =new OAuthRequester (OAUTH_HOST . $_SERVER ['PHP_SELF'], 'POST', $token) ;
    $access =$request->doRequest (0) ;
	*/
		
	//- In this sample, we save the token to a file, and use it in the Refresh example
	$fname =realpath (dirname (__FILE__)) . '/access_token.txt' ;
	file_put_contents ($fname, serialize ($access)) ;
		
} catch (Exception $e) {
	echo "OAuth/AccessToken\n", 'Caught exception: ',  $e->getMessage (), "\n";
	exit ;
}

//- Done
//exit ;
?>
