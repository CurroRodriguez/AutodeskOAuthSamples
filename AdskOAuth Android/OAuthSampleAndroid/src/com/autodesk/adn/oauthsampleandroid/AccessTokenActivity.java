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

import java.util.concurrent.TimeUnit;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import com.autodesk.adn.common.AdskOAuthUtil;
import com.autodesk.adn.common.TokenExt;


public class AccessTokenActivity extends Activity {

	
	private TokenExt accessToken;
	 
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_oauthen_succeed);


		
		//create a back-ground thread to access internet, get the access token
		//In Android 4.x, it is required to run another thread to access internet,
		//rather than the UI thread
		
		Thread bgThread = new Thread(getAccessTokenBackGroudThread);
		bgThread.start();
		
		
		
		//set onclick event for the button to refresh access token
		
		Button btnRefreshAccessToken = (Button)findViewById(R.id.btnExtendSession);
		btnRefreshAccessToken.setOnClickListener(new OnClickListener(){

			@Override
			public void onClick(View v) {

				// To refresh access token, we need to make sure we have a valid access token first
				
				if(null == accessToken){
					Toast.makeText(AccessTokenActivity.this, 
							"there is no access token, get access token first.", 
							Toast.LENGTH_LONG).show();
					return;
				}
				
				//start the back ground thread to refresh the access token
				
				Thread bgThread = new Thread(refreshAccessTokenBackgroundThread);
				bgThread.start();
				

			}
			
		
		});
		
		
		//logout, invalidate access token
		Button btnLogout = (Button)findViewById(R.id.btnLogout);
		btnLogout.setOnClickListener(new OnClickListener(){

			@Override
			public void onClick(View v) {
			
				Thread bgThread = new Thread(invalidateAccessTokenBackgroundThread);
				bgThread.start();
			}
			
		});
	}
	
	
	//back ground thread to get access token
	
	Runnable getAccessTokenBackGroudThread = new Runnable(){

		@Override
		public void run() {
			
			String pin; 
			
			Intent it = getIntent();
			if(null == it.getStringExtra("verifier")){
			  
				//For out-of-band, get verifier pin from JavascriptInterface,
				//which call javascript to parse the html getting the verifier PIN
				
				pin = JavascriptInterface.PIN;
				
			}
			else{
				
				//for in-band, get the verifier pin the intent, which was passed from 
				// webviewer activity
				
				pin = it.getStringExtra("verifier");
			}
			
			//Get access token with PIN, refer to AdskOAuthUtil.java for more information
			
			accessToken = AdskOAuthUtil.getAccessToken(pin);
			
			//back to UI thread, update the UI with access token
			
			runOnUiThread(new Runnable(){

				@Override
				public void run() {
					
					updateUI(accessToken);
				}

	
				
			});
		}
		
	};
	
	
	//back ground thread to refresh access token
	
	Runnable refreshAccessTokenBackgroundThread = new Runnable(){

		@Override
		public void run() {
		
			//refresh access token, refer to AdskOAuthUtil.java for more information
			
			accessToken = AdskOAuthUtil.refreshAccessToken(accessToken);
			
			//back to UI thread, update the UI with refreshed access token
			
			runOnUiThread(new Runnable(){

				@Override
				public void run() {
				
					updateUI(accessToken);
				}

			});
		}
		
	};
	
	
	//back ground thread to invalidate access token
	
	Runnable invalidateAccessTokenBackgroundThread = new Runnable(){

		@Override
		public void run() {
				
			//invalidate access token, refer to AdskOAuthUtil.java for more information
			
			AdskOAuthUtil.invalidateAccessToken(accessToken);
			
			//back to UI thread, return to first activity
			runOnUiThread(new Runnable(){

				@Override
				public void run() {
					
					//You have logged out. Return to first activity
					
			    	Intent it = new Intent(AccessTokenActivity.this,
							MainActivity.class);
							
					startActivity(it);
				}

			});
		}
		
	};
	
	
	//Display the access token information on UI
	
	private void updateUI(TokenExt accessToken) {
		
		if(null == accessToken) {
			Toast.makeText(AccessTokenActivity.this, 
					"there is no access token, get access token first.", 
					Toast.LENGTH_LONG).show();
			
			return;
		}
		
		String msg = "Congratulations, you are athenticated by Autodesk!\n This application uses following token to access the Protected Resources on behalf of you .\n\n";
		TextView tv = (TextView)findViewById(R.id.auth_result_message);
		tv.setText(msg);
		
		TextView txtOauthToken = (TextView)findViewById(R.id.txtOauthToken);
		TextView txtOauthTokenSecret = (TextView)findViewById(R.id.txtOauthTokenSecret);
		TextView txtOauthExpiresIn = (TextView)findViewById(R.id.txtOauthExpiresIn);
		TextView txtOauthAuthorizationExpiresIn = (TextView)findViewById(R.id.txtOauthAuthorizationExpiresIn);
		TextView txtOauthSessionHandle = (TextView)findViewById(R.id.txtOauthSessionHandle);
		
		txtOauthToken.setText(accessToken.getToken());
		txtOauthTokenSecret.setText(accessToken.getSecret());
		txtOauthExpiresIn.setText(calculateTime(Long.parseLong(accessToken.getOauthExpiresIn())));
		txtOauthAuthorizationExpiresIn.setText(calculateTime(Long.parseLong(accessToken.getOauthAuthorizationExpiresIn())));
		txtOauthSessionHandle.setText(accessToken.getSessionHandel());
		

	}
	
	//convert seconds to day/hours/minutes/seconds for easy reading
	
	private String calculateTime(long seconds ){
		int day = (int)TimeUnit.SECONDS.toDays(seconds);        
		 long hours = TimeUnit.SECONDS.toHours(seconds) - (day *24);
		 long minute = TimeUnit.SECONDS.toMinutes(seconds) - (TimeUnit.SECONDS.toHours(seconds)* 60);
		 long second = TimeUnit.SECONDS.toSeconds(seconds) - (TimeUnit.SECONDS.toMinutes(seconds) *60);
	     return  day +  " Days "  + hours + " Hours "  + minute + " Minutes "  + second + " Seconds " ;
	}
	  
}
