////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved 
// Written by Daniel Du  2013 - ADN/Developer Technical Services
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

package com.autodesk.adn.common;

import org.scribe.builder.ServiceBuilder;
import org.scribe.model.Token;
import org.scribe.model.Verifier;
import org.scribe.oauth.OAuthService;

public class AdskOAuthUtil {

	
	//create the OAuthService, 
	//refer to https://github.com/fernandezpablo85/scribe-java/wiki/getting-started
	
	private static OAuthService service;
	static Token _requestToken;
	
	public static void init(String consumerKey,String consumerSecret, String callbackUrl){

		//build an OAuthService with consumerKey, secret key and callback url
		
		service =
			        new ServiceBuilder()
					.provider(AdskOAuthApi.class)  //for Autodesk oauth service provider
					.apiKey(consumerKey)
			        .apiSecret(consumerSecret)
			        .callback(callbackUrl)
			        .debug()  //for debug, comment out this line in production environment 
			        .build();
		
	}
	
	//get the request token
	
	public static TokenExt getRequestToken(){
		
		_requestToken =service.getRequestToken();
		return TokenExt.getFrom( _requestToken);
		
	}
	
	//get authentication URL
	
	public static String getAuthorizationUrl(){
		
		final String authUrl = service.getAuthorizationUrl(_requestToken);
		return authUrl;
		
	}
	
	public static TokenExt getAccessToken(String pin){
		
		Token _accessToken = service.getAccessToken(_requestToken, new Verifier(pin));
		return TokenExt.getFrom(_accessToken);
		
	}
	
	public static TokenExt refreshAccessToken(TokenExt accessToken ){
	
		Token t = new Token(accessToken.getToken(),accessToken.getSecret(),accessToken.getRawResponse());
		
		//refreshToken is not part of scribe library, refer to OAuth10aServiceImpl.java for implementation
		
		Token _accessToken = service.refreshToken(t, accessToken.getSessionHandel());
		return TokenExt.getFrom(_accessToken);	
		
	}
	
	public static void invalidateAccessToken(TokenExt accessToken){
		
		Token t = new Token(accessToken.getToken(),accessToken.getSecret(),accessToken.getRawResponse());
		
		//invalidateToken is not part of scribe library, refer to OAuth10aServiceImpl.java for implementation
		
		service.invalidateToken(t, accessToken.getSessionHandel());
		
	}
}
