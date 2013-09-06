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

$fname =realpath (dirname (__FILE__)) . '/access_token.txt' ;
$access =unserialize (file_get_contents ($fname)) ;
unlink ($fname) ;

//- To disable the SSL check to avoid an exception with invalidate certificate on the server,
//- use the cURL CURLOPT_SSL_VERIFYPEER option and set it to false.

//- Once you are done, log-out
try {
	//- Change the command as the Google API does not implement that one
	$options ['access_token_uri'] =BaseUrl . 'OAuth/InvalidateToken' ;

	OAuthStore::instance ()->addServerToken (ConsumerKey, 'access', $access ['oauth_token'], $access ['oauth_token_secret'], 0, $options) ;
	$request =new OAuthRequester ($options ['access_token_uri'], 'GET', $access) ;
	$access =$request->doRequest (0, array ( CURLOPT_SSL_VERIFYPEER => 0, )) ;
} catch (Exception $e) {
	echo "OAuth/InvalidateToken\n", 'Caught exception: ',  $e->getMessage (), "\n";
	exit ;
}

//- Done
//exit ;
?>
