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

 This sample uses the stagging Oxygen server to demo the '3 legs' process to authentify
 a user on the Autodesk Cloud infrastructure.
 
 After installing PHP on your system, you need to install the Google PHP OAuth library for this sample
 The Google OAuth Client library found here: (http://code.google.com/p/oauth-php/)
 
 You also need to modify the Google OAuth library code like this:
    file oauth\OAuthRequester.php line #285, add
	   return ($token) ;

 The '3 legs' process is as follow:
 a- Get a 'request token' from the system
 b- Authorize the received token. Note here that Autodesk currently require you to manual log on Oxygen
      for authorization. This is why the sample is using your default browser for logging.
 c- Get an 'access token' and a session
 
 The sample also does not do a log-out at the end in order to save and reuse the token in other WEB sessions
 Instead it tries to save it in a file
 
*/

define ('ConsumerKey', 'Your Consumer Key') ;
define ('ConsumerSecret', 'Your Consumer Secret Key') ;
define ('BaseUrl', 'The Oauth server URL') ; // https://accounts.autodesk.com/ or https://accounts-staging.autodesk.com/

define ('OAUTH_TMP_DIR', function_exists ('sys_get_temp_dir') ? sys_get_temp_dir () : realpath ($_ENV ["TMP"])) ;

//- Check at what Leg3 we are
if ( !empty ($_POST) && !empty ($_POST ['step']) ) {
	$step =$_POST ['step'] ;
} else if ( !empty ($_GET) && !empty ($_GET ['step']) ) {
	$step =$_GET ['step'] ;
} else {
	unset ($step) ;
}

//- Call the Leg function
if ( isset ($step) ) {
	call_user_func ("Oauth_{$step}") ;
}

//- Leg functions
function Oauth_start () { //- Request token
	include ('AdskOAuthWebNoExtLeg1-2.php') ;
	exit ;
}

function Oauth_leg3 () { //- Access token
	global $access ;
	include ('AdskOAuthWebNoExtLeg3.php') ;
	//exit ;
}

function Oauth_refresh () { //- Refresh Access token
	global $access ;
	include ('AdskOAuthWebNoExtRefresh.php') ;
	//exit ;
}

function Oauth_logout () { //- Log-out
	include ('AdskOAuthWebNoExtExpire.php') ;
	header ('Location: ' . $_SERVER ['PHP_SELF']) ;
	exit ;
}

?>
<html>
	<header>
	</header>
	<body>
		<div><pre>
 This sample uses the stagging Oxygen server to demo the '3 legs' process to authentify
 a user on the Autodesk Cloud infrastructure.
 
 After installing PHP on your system, you need to install the Google PHP OAuth library for this sample
 The Google OAuth Client library found here: (http://code.google.com/p/oauth-php/)
 
 You also need to modify the Google OAuth library code like this:
    file oauth\OAuthRequester.php line #285, add
	   return ($token) ;

 The '3 legs' process is as follow:
 a- Get a 'request token' from the system
 b- Authorize the received token. Note here that Autodesk currently require you to manual log on Oxygen
    for authorization. This is why the sample is using your default browser for logging.
 c- Get an 'access token' and a session
 
 The sample also does not do a log-out at the end in order to save and reuse the token in other WEB sessions
 Instead it tries to save it in a file		
		</pre></div>
		
		<div>
		<?
			if ( isset ($step) ) {
				switch ( $step ) {
					case 'leg3':
					case 'refresh':
						?><h4>OAuth/AccessToken response</h4>
						<pre><?
							echo "<pre>OAuth/AccessToken response\n",
								"\toauth_token: {$access ['oauth_token']}\n",
								"\toauth_token_secret: {$access ['oauth_token_secret']}\n",
								"\toauth_session_handle: {$access ['oauth_session_handle']}\n",
								"\toauth_authorization_expires_in: {$access ['oauth_authorization_expires_in']}\n",
								"\toauth_expires_in: {$access ['oauth_expires_in']}\n",
								"\tx_oauth_user_name: {$access ['x_oauth_user_name']}\n",
								"\tx_oauth_user_guid: {$access ['x_oauth_user_guid']}\n",
								"-----------------------\n\n</pre>" ;
						?></pre><?
						//break ;
					//case 'refresh':
						?><hr />
						<form action="<?= $_SERVER ['PHP_SELF'] ?>" method="post">
							<input type="hidden" name="step" value="refresh" />
							<input type="submit" value="Refresh..." />
						</form>
						<form action="<?= $_SERVER ['PHP_SELF'] ?>" method="post">
							<input type="hidden" name="step" value="logout" />
							<input type="submit" value="Log Out..." />
						</form>
						<?
						break ;
				}
			} else {
				?>
				</div><form action="<?= $_SERVER ['PHP_SELF'] ?>" method="post">
					<input type="hidden" name="step" value="start" />
					<input type="submit" value="Start..." />
				</form>
				<?
			}
		?>
		</div>
		
	</body>
</html>
