package org.scribe.model;

import java.io.Serializable;

/**
 * Represents an OAuth token (either request or access token) and its secret
 * 
 * @author Pablo Fernandez
 */
public class Token implements Serializable
{
  private static final long serialVersionUID = 715000866082812683L;
	
  private final String token;
  private final String secret;
  private final String rawResponse;

  //added by Daniel Du 2013-07-05
  //Number of seconds until the access token expires
  private final int oauth_expires_in = 0;;
  //A token, separate from the access token, that can be used to acquire a new access token w/o user interaction
  private final String oauth_session_handle = "";
  //Duration that the Consumer is authorized to access. When this expires 
  //the access token cannot be refreshed and requires full user authorization to get a new access token.
  private final int oauth_authorization_expires_in = 0;
  /**
   * Default constructor
   * 
   * @param token token value
   * @param secret token secret
   */
  public Token(String token, String secret)
  {
    this(token, secret, null);
  }

  public Token(String token, String secret, String rawResponse)
  {
    this.token = token;
    this.secret = secret;
    this.rawResponse = rawResponse;
  }

  public String getToken()
  {
    return token;
  }

  public String getSecret()
  {
    return secret;
  }

  public String getRawResponse()
  {
    if (rawResponse == null)
    {
      throw new IllegalStateException("This token object was not constructed by scribe and does not have a rawResponse");
    }
    return rawResponse;
  }

  @Override
  public String toString()
  {
    return String.format("Token[%s , %s]", token, secret);
  }
}
