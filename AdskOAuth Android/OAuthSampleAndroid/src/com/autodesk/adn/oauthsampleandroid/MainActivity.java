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

package com.autodesk.adn.oauthsampleandroid;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.view.Menu;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.TextView;
import android.widget.Toast;

import com.autodesk.adn.common.Network;
import com.autodesk.adn.common.AdskOAuthUtil;
import com.autodesk.adn.common.TokenExt;

public class MainActivity extends Activity {

	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);
		
		//get the button and set onclick event for it
		
		Button btnRequestToken = (Button)findViewById(R.id.btnRequestToken);
		btnRequestToken.setOnClickListener(new OnClickListener(){

			@Override
			public void onClick(View arg0) {
				
				//check network connection, we need to access internet.
				//make sure have permission for internet access in AndroidManifest.xml with 
				// <uses-permission android:name="android.permission.INTERNET" />
				
				if (!Network.isAvailable(getBaseContext())) {
					Toast.makeText(getApplicationContext(),
							"network is not available",
							Toast.LENGTH_LONG).show();
					
					return;
				}
				
				//execute the back-ground thread to get the request token and authentication url
				//In Android 4.x, it is required to run another thread to access Internet,
				//rather than the UI thread
				Thread bgThread = new Thread(backGroudThread);
				bgThread.start();
				
				
			}
			
			
			
		});
	}

	//create a back-ground thread to access internet.

	Runnable backGroudThread = new Runnable(){

		@Override
		public void run() {
			
			//get the consumer key and secret key from UI
			
			TextView tvConsumerKey = (TextView)findViewById(R.id.txtConsumerKey);
			TextView tvConsumerSecret = (TextView)findViewById(R.id.txtConsumerSecretKey);
			CheckBox chkIsOOB = (CheckBox)findViewById(R.id.chkIsOOB);
			
			String consumerKey = tvConsumerKey.getText().toString().trim();
			String consumerSecret = tvConsumerSecret.getText().toString().trim();
			String callbackUrl = chkIsOOB.isChecked() ? "oob" : "http://127.0.0.1/";

			//initialize the oAuth utility with consumer key, secret key and callback url.
			//if out-of-band, the callback url is "oob"
			//if in-band, we use a fake callback url : "http://127.0.0.1/"
			
			AdskOAuthUtil.init(consumerKey, consumerSecret,callbackUrl);
			
			// Obtain the request token. This is the first in the series of steps
	        // need to use the autodesk oauth API to authorize access to a resource
			final TokenExt requestToken = AdskOAuthUtil.getRequestToken();
			if(null == requestToken) {
				Toast.makeText(getApplicationContext(),
						"error during request token.",
						Toast.LENGTH_LONG).show();
				return;
			}
			
			//Get the Authorization Url
			
			final String authUrl = AdskOAuthUtil.getAuthorizationUrl();
			
			//back to UI thread, start browser in another activity to authenticate
			
			runOnUiThread(new Runnable(){

				@Override
				public void run() {

					//pass the AuthorizationUrl to the browser in another activity for authentication
					//it asks users to input username and password for authentication
					
					startWebViewer(authUrl, requestToken);
					
				}
				
			});
		}
		
	};
	
	// start a new activity to launch the auth url for authentication 
	
	private void startWebViewer(String url, TokenExt requestToken){
		
		//pass the URL and request token to WebViewerActivity for latter use
		
    	Intent it = new Intent(this, WebViewerActivity.class)
    		.putExtra("URL", Uri.parse(url).toString())
    		.putExtra("RequestToken", requestToken);
    	
        startActivity(it);
	}
	
	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.main, menu);
		return true;
	}

}
