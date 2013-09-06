/////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved 
// Written by Adam Nagy 2013 - ADN/Developer Technical Services
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

#import "ViewController.h"
#import <UIKit/UIKit.h>

// This is an OAuth library I got from
// https://github.com/tweetdeck/TDOAuth
// The only thing I needed to modify was the
// function "URLRequestForPath/POSTParameters" so that I could
// add "oauth_session_handle" to the "Authorization" header of
// the POST message
#import "TDOAuth.h"

@interface ViewController ()

@end

@implementation ViewController

// Fill in these 3 macros with the correct information
#define O2_OAUTH_KEY     @"your.consumer.key.here"
#define O2_OAUTH_SECRET  @"your.comsumer.secret.key.here"
// Do NOT add the "https://" prefix!
#define O2_HOST          @"your.oauth.server.here"

#define O2_AUTHORIZE     @"https://" O2_HOST @"/OAuth/Authorize"
#define O2_ALLOW         @"https://" O2_HOST @"/OAuth/Allow"
#define LOGIN @"Log In"
#define LOGOUT @"Log Out"

NSString * requestToken;
NSString * requestTokenSecret;
NSString * accessToken;
NSString * accessTokenSecret;
NSString * sessionHandle;
NSString * accessTokenExpires;
NSString * authorizationExpires;

/// This function gets the URL parameters from the string
- (NSMutableDictionary *)getParams:(NSString *)fromString
{
  NSMutableDictionary * params = [NSMutableDictionary dictionary];
  NSArray * split = [fromString componentsSeparatedByString:@"&"];
  for (NSString * str in split)
  {
    NSArray * split2 = [str componentsSeparatedByString:@"="];
    if (split.count > 1)
      [params setObject:split2[1] forKey:split2[0]];
  }
  
  return params;
}

/// Convert the seconds to days/hours/minutes
- (NSString *)convertToDate:(NSString *)seconds
{
  double secs = [seconds doubleValue] / 1000.0;
  double days = floor(secs / 86400);
  secs -= days * 86400;
  double hours = floor(secs / 3600);
  secs -= hours * 3600;
  double minutes = floor(secs / 60);
  
  return [NSString
    stringWithFormat:@"%.0f days, %.0f hours, %.0f minutes",
    days, hours, minutes];
}

/// The first step of authentication is to request a token
- (Boolean)RequestToken
{
  NSMutableDictionary * dict = nil;

  // In case of out-of-band authorization
  // we also need to add the below parameter to
  // the "Authorization" header value
  if ([self isOOB])
  {
    dict = [NSMutableDictionary dictionary];
    [dict setObject:@"oob" forKey:@"oauth_callback"];
  }

  NSURLRequest * req =
  [TDOAuth
   URLRequestForPath:@"/OAuth/RequestToken"
   POSTParameters:nil
   extraParams:dict
   host:O2_HOST
   consumerKey:O2_OAUTH_KEY
   consumerSecret:O2_OAUTH_SECRET
   accessToken:nil
   tokenSecret:nil];
  
  NSURLResponse * response;
  NSError * error = nil;
  NSData * result = [NSURLConnection
    sendSynchronousRequest:req
    returningResponse:&response
    error:&error];
  NSString * s = [[NSString alloc]
    initWithData:result
    encoding:NSUTF8StringEncoding];
  
  // Let's get the tokens we need
  NSMutableDictionary * params = [self getParams:s];
  requestToken = params[@"oauth_token"];
  requestTokenSecret = params[@"oauth_token_secret"];
  
  // If we did not get those params then something went wrong
  if (requestToken == nil)
  {
    [self showMessage:
      @"Failure!<br />Could not get request token!<br />Maybe the credentials are incorrect?"];
    return false;
  }
  
  return true;
}

/// The second step is to authorize the user using the
/// Autodesk login system
- (void)Authorize
{
  // Get rid of the URL encoding (%3D, etc)
  NSString * requestToken_clean = [requestToken
    stringByReplacingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
  
  NSString * path =
  [NSString
   stringWithFormat:@"%@?oauth_token=%@&viewmode=mobile",
   O2_AUTHORIZE,
   requestToken_clean ];
  
  // In case of out-of-band authorization, let's show the
  // authorization page which will provide the user with a PIN
  // in the default browser
  // Then here in our app request the user to type in that PIN
  if ([self isOOB])
  {
    [[UIApplication sharedApplication]
      openURL:[NSURL URLWithString:path]];
  
    UIAlertView * alert = [[UIAlertView alloc]
      initWithTitle:@"Authorization PIN"
      message:@"Please type here the authorization PIN!"
      delegate:self
      cancelButtonTitle:@"Done"
      otherButtonTitles:nil];
    alert.alertViewStyle = UIAlertViewStylePlainTextInput;
    [alert show];
  }
  // Otherwise let's load the page in our web viewer so that
  // we can catch the URL that it gets redirected to
  else
  {
    NSURLRequest * req = [NSURLRequest
      requestWithURL:[NSURL URLWithString:path]
      cachePolicy:NSURLRequestUseProtocolCachePolicy
      timeoutInterval:TDOAuthURLRequestTimeout];
    
    [self.webView loadRequest:req];
  }
}

/// The third step is to authenticate using the request tokens
/// Once you get the access token and access token secret
/// you need to use those to make your further REST calls
/// Same in case of refreshing the access tokens or invalidating
/// the current session. To do that we need to pass in
/// the acccess token and access token secret as the accessToken and
/// tokenSecret parameter of the [TDOAuth URLRequestForPath] function
- (void)AccessToken:(Boolean)refresh PIN:(NSString *)PIN
{
  NSString * tokenParam = requestToken;
  NSString * tokenSecretParam = requestTokenSecret;
  NSMutableDictionary * dict = [NSMutableDictionary dictionary];
  
  // If we already got access tokens and now just try to refresh
  // them then we need to provide the session handle 
  if (refresh)
  {
    [dict setObject:sessionHandle forKey:@"oauth_session_handle"];
    tokenParam = accessToken;
    tokenSecretParam = accessTokenSecret;
  }
  
  // If we used out-of-band authorization
  // then we got a PIN that we need now
  if (PIN != nil)
    [dict setObject:[PIN pcen] forKey:@"oauth_verifier"];

  NSURLRequest * req =
  [TDOAuth
   URLRequestForPath:@"/OAuth/AccessToken"
   POSTParameters:nil
   extraParams:dict
   host:O2_HOST
   consumerKey:O2_OAUTH_KEY
   consumerSecret:O2_OAUTH_SECRET
   accessToken:tokenParam
   tokenSecret:tokenSecretParam];
  
  NSURLResponse * response;
  NSError * error = nil;
  NSData * result = [NSURLConnection
    sendSynchronousRequest:req
    returningResponse:&response
    error:&error];
  NSString * s = [[NSString alloc]
    initWithData:result
    encoding:NSUTF8StringEncoding];
  
  // Let's get the tokens we need
  NSMutableDictionary * params = [self getParams:s];
  accessToken = params[@"oauth_token"];
  accessTokenSecret = params[@"oauth_token_secret"];
  sessionHandle = params[@"oauth_session_handle"];
  accessTokenExpires = params[@"oauth_expires_in"];
  authorizationExpires = params[@"oauth_authorization_expires_in"];

  // If session handle is not null then we got the tokens
  if (sessionHandle != nil)
  {
    self.RefreshButton.enabled = true;
    self.LogInButton.title = LOGOUT;
    
    if (refresh)
      [self showMessage:@"Success!<br />Managed to refresh token!"];
    else
      [self showMessage:@"Success!<br />Managed to log in and get access token!"];
  }
  else
  {
    self.RefreshButton.enabled = false;
    self.LogInButton.title = LOGIN;

    if (refresh)
      [self showMessage:@"Failure!<br />Could not refresh token!"];
    else
      [self showMessage:@"Failure!<br />Could not get access token!"];
  }
}

/// If we do not want to use the service anymore then
/// the best thing is to log out, i.e. invalidate the tokens we got
- (void)InvalidateToken
{
  NSURLRequest * req =
  [TDOAuth
   URLRequestForPath:@"/OAuth/InvalidateToken"
   POSTParameters:nil
   extraParams:[NSMutableDictionary dictionaryWithObject:sessionHandle forKey:@"oauth_session_handle"]
   host:O2_HOST
   consumerKey:O2_OAUTH_KEY
   consumerSecret:O2_OAUTH_SECRET
   accessToken:accessToken
   tokenSecret:accessTokenSecret];
  
  NSURLResponse * response;
  NSError * error = nil;
  NSData * result = [NSURLConnection
    sendSynchronousRequest:req
    returningResponse:&response
    error:&error];
  NSString * s = [[NSString alloc]
    initWithData:result
    encoding:NSUTF8StringEncoding];
  
  // If Invalidate was successful, we will not get back any data
  if ([s isEqualToString:@""])
  {
    // Set the buttons' state
    self.RefreshButton.enabled = false;
    self.LogInButton.title = LOGIN;
    
    // Clear the various tokens
    requestToken = requestTokenSecret =
    sessionHandle = accessToken = accessTokenSecret = nil;
    
    [self showMessage:@"Success!<br />Managed to log out!"];
  }
  else
  {
    [self showMessage:@"Failure!<br />Could not log out!"];
  }
}

/// When a new URL is being shown in the browser
/// then we can check the URL
/// This is needed in case of in-band authorization
/// which will redirect us to a given
/// URL (O2_ALLOW) in case of success
- (void)webViewDidFinishLoad:(UIWebView *)aWebView
{
  // In case of out-of-band login
  // we do not need to check the callback URL
  // Instead we'll need the PIN that the webpage
  // will provide for the user
  if ([self isOOB])
    return;

  // Let's check if we got redirected to the correct page
  if ([self isAuthorizeCallBack])
  {
    [self AccessToken:false PIN:nil];
  }
}

/// In case of out-of-band authorization this is where we
/// continue once the user got the PIN
- (void)alertView:(UIAlertView *)alertView didDismissWithButtonIndex:(NSInteger)buttonIndex
{
  [self AccessToken:false PIN:[[alertView textFieldAtIndex:0] text]];
}

/// Check if the URL is O2_ALLOW, which means that
/// the user could log in successfully
- (Boolean)isAuthorizeCallBack
{
  NSString * fullUrlString = self.webView.request.URL.absoluteString;
  
  if (!fullUrlString)
    return false;
  
  NSArray * arr = [fullUrlString componentsSeparatedByString:@"?"];
  if (!arr || arr.count!=2)
    return false;
  
  // If we were redirected to the O2_ALLOW URL
  // then the user could log in successfully
  if ([arr[0] isEqualToString:O2_ALLOW])
    return true;
  
  // If we got to this page then
  // probably there is an issue
  if ([arr[0] isEqualToString:O2_AUTHORIZE])
  {
    // If the page contains the word "oauth_problem"
    // then there is clearly a problem
    NSString * content = [self.webView  stringByEvaluatingJavaScriptFromString:@"document.body.innerHTML"];
    if ([content rangeOfString:@"oauth_problem"].location != NSNotFound)
      [self showMessage:@"Failure!<br />Could not log in!<br />Try again!"];
  }
  
  return false;
}

/// Once the application's view got loaded
- (void)viewDidLoad
{
  [super viewDidLoad];
}

- (void)didReceiveMemoryWarning
{
  [super didReceiveMemoryWarning];
  
  // Dispose of any resources that can be recreated.
}

- (IBAction)LogInClick:(id)sender
{
  UIBarButtonItem * button = (UIBarButtonItem *)sender;
  if ([button.title isEqualToString:LOGIN])
  {
    // Test the oxygen system
  
    // Step 1
    if ([self RequestToken])
    {
      // Step 2
      [self Authorize];
    }
  
    // Step 3
    // If Authorize succeeds, then in case of out-of-band authorization
    // the /OAuth/AccessToken will be called from
    // didDismissWithButtonIndex, but in case of in-band authorization
    // it will be called from webViewDidFinishLoad
  }
  else
  {
    [self InvalidateToken];
  }
}

- (IBAction)RefreshClick:(id)sender
{
  [self AccessToken:true PIN:nil];
}

/// WARNING: Out-of-band authorization is shown here
/// only for educational purposes and should only be used
/// if for some reason you cannot use in-band authorization
/// In case of out-of-band authorization the web page
/// will provide a PIN that the user will need to paste in
/// the message box of the iOS app
- (IBAction)OobClick:(id)sender
{
  if ([self isOOB])
    self.OobButton.style = UIBarButtonItemStyleBordered;
  else
    self.OobButton.style = UIBarButtonItemStyleDone;
}

/// Checks if we should use out-of-band authorization
- (Boolean)isOOB
{
  return (self.OobButton.style == UIBarButtonItemStyleDone);
}

/// Utility function to show information to the user
- (void)showMessage:(NSString *)text
{
  NSString * html =
  [NSString stringWithFormat:@"<div style=\"font-size:24px; margin:10%%; text-align:center;\">%@<br />&nbsp;<div style=\"text-align:left; font-size:18px;\">requestToken = %@ <br />requestTokenSecret = %@ <br />accessToken = %@<br />accessTokenSecret = %@<br />sessionHandle = %@<br />accessTokenExpires = %@<br />authorizationExpires = %@</div></div>",
    text,
    [requestToken stringByReplacingPercentEscapesUsingEncoding:NSUTF8StringEncoding],
    [requestTokenSecret stringByReplacingPercentEscapesUsingEncoding:NSUTF8StringEncoding],
    [accessToken stringByReplacingPercentEscapesUsingEncoding:NSUTF8StringEncoding],
    [accessTokenSecret stringByReplacingPercentEscapesUsingEncoding:NSUTF8StringEncoding],
    [sessionHandle stringByReplacingPercentEscapesUsingEncoding:NSUTF8StringEncoding],
    [self convertToDate:accessTokenExpires],
    [self convertToDate:authorizationExpires]];
  
  [self.webView loadHTMLString:html baseURL:nil];
}
@end
