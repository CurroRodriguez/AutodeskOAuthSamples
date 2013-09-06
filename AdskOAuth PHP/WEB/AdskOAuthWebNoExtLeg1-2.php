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
	//- The following line is for reference only, the call to OAuthRequester::requestRequestToken() includes that step
	//OAuthStore::instance ()->addServerToken (ConsumerKey, 'request', $token ['oauth_token'], $token ['oauth_token_secret'], 0, array ()) ;
	$fname =realpath (dirname (__FILE__)) . '/token.txt' ;
	file_put_contents ($fname, serialize ($token)) ;
} catch (Exception $e) {
	echo "OAuth/RequestToken\n", 'Caught exception: ',  $e->getMessage (), "\n";
	exit ;
}

//- 2nd leg: Authorize the token
//- Currently, Autodesk Oxygen service requires you to manually log into the system, so we are using your default browser
try {
	//$url =BaseUrl . "OAuth/Authorize" . "?oauth_token=" . urlencode (stripslashes ($token ['oauth_token'])) ;
	$url =BaseUrl . "OAuth/Authorize" . "?oauth_callback={$_SERVER ['HTTP_REFERER']}?step=leg3&oauth_token=" . urlencode (stripslashes ($token ['oauth_token'])) ;
	header ("Location: $url") ;
} catch (Exception $e) {
	echo "OAuth/Authorize\n", 'Caught exception: ',  $e->getMessage (), "\n";
	exit ;
}

//- The sample continues in AdskOAuthWebNoExtLeg3.php
exit ;
?>