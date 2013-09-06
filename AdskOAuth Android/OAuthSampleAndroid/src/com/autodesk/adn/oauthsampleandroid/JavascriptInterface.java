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

import android.util.Log;

public class JavascriptInterface {

	public static String PIN = null;
			
	public void getVerifierPIN(final String html) {
	      Log.i("Verifier:", html);

	      //simple validation here just for demo, you can make more strict validation with regular expression
	      //here to make sure the verifier pin is valid
	      
	      if(html.trim().length() > 0){
	        PIN=html;  
	      }
	}
	
}
