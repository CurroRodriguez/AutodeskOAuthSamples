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

import java.io.Serializable;
import java.io.UnsupportedEncodingException;
import java.net.URLDecoder;
import java.util.LinkedHashMap;
import java.util.Map;

import org.scribe.model.Token;


//Extended class of Token, including the expiration and session handle 

public class TokenExt implements Serializable {
	 
	private static final long serialVersionUID = 4718021865499780088L;

	private static String token;
	  private static String secret;
	  private static String rawResponse;
	  private static String oauthExpiresIn;
	  private static String oauthAuthorizationExpiresIn;
	  private static String sessionHandle;
	  
	private TokenExt(){
		
	}
	
	public static TokenExt getFrom(Token innerToken){
		
		try{
			TokenExt t = new TokenExt();
			token = innerToken.getToken();
			secret = innerToken.getSecret();
			rawResponse = innerToken.getRawResponse();
			oauthExpiresIn = extractOauthExpiresIn(innerToken);
			oauthAuthorizationExpiresIn = extractOauthAuthorizationExpiresIn(innerToken);
			sessionHandle = extractSessionHandle(innerToken);
	
			return t;
		}
		catch(Exception ex){
			return null;
		}
	}

	
	private static String extractOauthExpiresIn(Token t) {
		

		String oauthExpiresIn = "";
		try {
			oauthExpiresIn = splitQuery(t.getRawResponse()).get("oauth_expires_in");
			return oauthExpiresIn;
			
		} catch (UnsupportedEncodingException e) {
			e.printStackTrace();
			return "";
		}
	}
	
	private static String extractOauthAuthorizationExpiresIn(Token t) {
		
		String oauthAuthorizationExpiresIn = "";
		try {
			oauthAuthorizationExpiresIn = splitQuery(t.getRawResponse()).get("oauth_authorization_expires_in");
			return oauthAuthorizationExpiresIn;
			
		} catch (UnsupportedEncodingException e) {
			e.printStackTrace();
			return "";
		}
	}
	
	private static String extractSessionHandle(Token t) {
		String sessionHandel = "";
		try {
			sessionHandel = splitQuery(t.getRawResponse()).get("oauth_session_handle");
			return sessionHandel;
			
		} catch (UnsupportedEncodingException e) {
			e.printStackTrace();
			return "";
		}
		
	}
	

	private static Map<String, String> splitQuery(String query) throws UnsupportedEncodingException {
	    Map<String, String> query_pairs = new LinkedHashMap<String, String>();
	    String[] pairs = query.split("&");
	    for (String pair : pairs) {
	        int idx = pair.indexOf("=");
	        query_pairs.put(URLDecoder.decode(pair.substring(0, idx), "UTF-8"), URLDecoder.decode(pair.substring(idx + 1), "UTF-8"));
	    }
	    return query_pairs;
	}

	public String getToken() {
		return token;
	}


	public String getSecret() {
		return secret;
	}

	public String getRawResponse() {
		return rawResponse;
	}


	public String getOauthExpiresIn() {
		return oauthExpiresIn;
	}


	public String getOauthAuthorizationExpiresIn() {
		return oauthAuthorizationExpiresIn;
	}


	public String getSessionHandel() {
		return sessionHandle;
	}

	
}
