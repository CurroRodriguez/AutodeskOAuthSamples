'''

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

'''

import os, sys, time
import urlparse, urllib, webbrowser

import httplib
import oauth2 as oauth

ConsumerKey ='Your Consumer Key'
ConsumerSecret ='Your Consumer Secret Key'
BaseUrl ='The Oauth server URL' # https://accounts.autodesk.com/ or https://accounts-staging.autodesk.com/

def main (argv =None):
	#- Create your consumer with the proper key/secret.
	consumer =oauth.Consumer (key =ConsumerKey, secret =ConsumerSecret)

	#- Create our client. Prepare the Python OAuth for consuming our Oxygen service
	#- Disable the SSL check to avoid an exception with invalidate certificate on the server
	client =oauth.Client (consumer)
	client.disable_ssl_certificate_validation =True

	#- 1st leg: Get the 'request token'
	#- The OAuth Client request works just like httplib2 for the most part.
	request_token =''
	try:
		resp, content =client.request (BaseUrl + "OAuth/RequestToken", "POST")
		request_token =dict (urlparse.parse_qsl (content))
		print "OAuth/RequestToken"
		print "\toauth_token: %s" % request_token['oauth_token']
		print "\toauth_token_secret: %s" % request_token['oauth_token_secret']
		print "-----------------------"
		print " "
		#- Set the token and secret for subsequent requests.
		token =oauth.Token (request_token ['oauth_token'], request_token ['oauth_token_secret'])
		client.token =token
	except Exception as inst:
		print "OAuth/RequestToken"
		print "Caught exception: " % inst # __str__ allows args to printed directly
		sys.exit ()

	#- 2nd leg: Authorize the token
	#- Currently, Autodesk Oxygen service requires you to manually log into the system, so we are using your default browser
	try:
		values ={ 'oauth_token' : request_token ['oauth_token'] }
		data =urllib.urlencode (values)
		url =BaseUrl + "OAuth/Authorize" + "?" + data
		webbrowser.open_new (url)
		psLine =raw_input ("Press [Enter] when logged" )
	except Exception as inst:
		print "OAuth/Authorize"
		print "Caught exception: " % inst # __str__ allows args to printed directly
		sys.exit ()
	
	#- 3rd leg: Get the 'access token' and session
	try:
		resp, content =client.request (BaseUrl + "OAuth/AccessToken", "POST")
		access_token =dict (urlparse.parse_qsl (content))
		print "OAuth/AccessToken"
		print "\toauth_token: %s" % access_token ['oauth_token']
		print "\toauth_token_secret: %s" % access_token ['oauth_token_secret']
		print "\toauth_session_handle: %s" % access_token ['oauth_session_handle']
		print "\toauth_authorization_expires_in: %s" % access_token ['oauth_authorization_expires_in']
		print "\toauth_expires_in: %s" % access_token ['oauth_expires_in']
		print "\tx_oauth_user_name: %s" % access_token ['x_oauth_user_name']
		print "\tx_oauth_user_guid: %s" % access_token ['x_oauth_user_guid']
		print "-----------------------"
		print " "
		#- Set the token and secret for subsequent requests.
		token =oauth.Token (access_token ['oauth_token'], access_token ['oauth_token_secret'])
		client.token =token
	except Exception as inst:
		print "OAuth/AccessToken"
		print "Caught exception: " % inst # __str__ allows args to printed directly
		sys.exit ()
		
	#- To refresh the 'Access token' before it expires, just call again
	#- resp, content =client.request (BaseUrl + "OAuth/AccessToken", "POST")
	#- Note that at this time the 'Access token' never expires

	#- Do you other work HERE

	#- Once you are done, log-out
	try:
		values ={ 'oauth_session_handle' : access_token ['oauth_session_handle'] }
		data =urllib.urlencode (values)
		client.request (BaseUrl + "OAuth/InvalidateToken" + "?" + data, "POST")
		print "You logged out!"
		#- Clear the token and secret for subsequent requests.
		client.token =None
	except Exception as inst:
		print "OAuth/InvalidateToken"
		print "Caught exception: " % inst # __str__ allows args to printed directly
		sys.exit ()

if __name__ == "__main__":
	main()
