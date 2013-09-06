////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved 
// Written by Daniel Du 2013 - ADN/Developer Technical Services
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

package com.autodesk.adn.oauthsampleandroid;

import java.net.URLDecoder;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.View;
import android.webkit.WebView;
import android.webkit.WebViewClient;

import com.autodesk.adn.common.AdskOAuthApi;
import com.autodesk.adn.common.TokenExt;

public class WebViewerActivity extends Activity {

	private WebView webview;

	private TokenExt requestToken;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_account);
		
		//get the URL and requestToken, which is passed from MainActivity
		
		Intent it = getIntent();
		String url = it.getStringExtra("URL");
		requestToken = (TokenExt)it.getSerializableExtra("RequestToken");
		
		Log.i("auth url : ",url);

		//initialize the webviewer for authentication
		
		init();
		
		//load the url in webviewer for authentication
		
		load(url,webview);
		    
		
	}

	public void init(){
		
		//initialize the web client
		
		webview = (WebView) findViewById(R.id.webView1);
		webview.clearCache(true);
		
		//enable JavaScript to parse the html, getting the verifier pin from html when "oob"
		
		webview.getSettings().setJavaScriptEnabled(true);
		webview.addJavascriptInterface(new JavascriptInterface(), "INPUTVALUE"); 
		
		webview.setWebViewClient(new WebViewClient(){

			@Override
			public boolean shouldOverrideUrlLoading(WebView view, String url) {

				//For in band with callback url, if authentication passed, the service provider(autodesk oAuth server) 
				// redirects to the callback url
				
				// check for our fake callback url
		        if(url.contains("http://127.0.0.1/")){
		          
		          //authorization complete, hide the webview for now.
		        	
		        	webview.setVisibility(View.GONE);
		           
		          //get the verifier pin from callback url
		        	
		          Uri uri = Uri.parse(url);
		          String verifier = uri.getQueryParameter("oauth_verifier");

		          //start AccessToken activity and pass the verifier to it. 
		          
		          Intent intent=new Intent(WebViewerActivity.this,AccessTokenActivity.class);
		          intent.putExtra("verifier", verifier);
			      startActivity(intent);
			        
			      return true;
		        }
		        else{
				  return super.shouldOverrideUrlLoading(view, url);
		        }
			}
			
			@Override
			public void onPageFinished(WebView view, String url){
				 Log.i("onPageFinished", url);
				 
				 //For out-of-band,  if authentication passed, the service provider(autodesk oauth server)
				 // redirects to allowurl, in which a verifier PIN is returned in an input box
				 // we need to parse the html page with javescript to get the verifier PIN
				 
				 url = URLDecoder.decode(URLDecoder.decode(url));
				 if(url.equalsIgnoreCase(AdskOAuthApi.getAllowUrl(requestToken))) {
					 /*
			         * This call inject JavaScript into the page which just finished loading.
			         */
			        view.loadUrl("javascript:window.INPUTVALUE.getVerifierPIN(document.getElementsByTagName('input')[0].value);");
			      
			        //now refer to JavaScriptInterface.java for android implementation of getVerifierPIN
			        //and refer to http://developer.android.com/guide/webapps/webview.html for more information 
			        
			        //start AccessToken activity
			        
			        Intent intent=new Intent(WebViewerActivity.this,AccessTokenActivity.class);
			        startActivity(intent);
				}
				
				
				
				super.onPageFinished(view, url); 
			}
			
			
			
		});
	}

	//load the url into web view
	public void load(final String url,final WebView view){  
        if(url==null||"".equals(url)){  
            return;  
        }  
        new Thread(){  
            public void run() {  
                view.loadUrl(url);  
            }  
        }.start();  
    }  
	
	
	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.account, menu);
		return true;
	}

}
