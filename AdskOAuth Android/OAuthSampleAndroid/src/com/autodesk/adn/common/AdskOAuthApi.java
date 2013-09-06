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

import java.net.URLEncoder;

import org.scribe.builder.api.DefaultApi10a;
import org.scribe.model.Token;
import org.scribe.model.Verb;


/**
 * Autodesk oAuth RESTful Api
 * 
 * @author podgorj@autodesk.com
 * 
 */

//this class extends the scribe library

public class AdskOAuthApi extends DefaultApi10a {

  //Change this if you are not using staging server, for production server, leave it empty
	
  public static String ENV = "-staging";  

  public static final String ACCOUNTS_DOMAIN_FORMAT_URL = "https://accounts%s.autodesk.com";
  public static final String OAUTH_PROVIDER_ENDPOINT = "/OAuth";
  public static final String ACCESS_TOKEN_ENDPOINT = "/AccessToken";
  public static final String REQUEST_TOKEN_ENDPOINT = "/RequestToken";
  public static final String INVALIDATE_TOKEN_ENDPOINT = "/InvalidateToken";
  public static final String AUTHORIZE_ENDPOINT = "/Authorize";
  public static final String ALLOW_ENDPOINT = "/Allow";

  @Override
  public String getRequestTokenEndpoint() {
    return String.format(ACCOUNTS_DOMAIN_FORMAT_URL + OAUTH_PROVIDER_ENDPOINT
        + REQUEST_TOKEN_ENDPOINT, ENV);
  }

  @Override
  public String getAccessTokenEndpoint() {
    return String.format(ACCOUNTS_DOMAIN_FORMAT_URL + OAUTH_PROVIDER_ENDPOINT
        + ACCESS_TOKEN_ENDPOINT, ENV);
  }

  @Override
  public String getAuthorizationUrl(final Token requestToken) {
    return String.format(ACCOUNTS_DOMAIN_FORMAT_URL + OAUTH_PROVIDER_ENDPOINT + AUTHORIZE_ENDPOINT
        + "?viewmode=mobile&oauth_token=%s", ENV, URLEncoder.encode(requestToken.getToken()));
  }
  

  public String getInvalidateTokenEndpoint() {
	  return String.format(ACCOUNTS_DOMAIN_FORMAT_URL + OAUTH_PROVIDER_ENDPOINT
		        + INVALIDATE_TOKEN_ENDPOINT, ENV);
  }


  public static String getAllowUrl(final TokenExt requestToken) {
    return String.format(ACCOUNTS_DOMAIN_FORMAT_URL + OAUTH_PROVIDER_ENDPOINT + ALLOW_ENDPOINT
        + "?k=%s", ENV, requestToken.getToken());
  }

  
  @Override
  public Verb getAccessTokenVerb()
  {
    return Verb.POST;
  }

  @Override
  public Verb getRequestTokenVerb()
  {
    return Verb.POST;
  }


}
