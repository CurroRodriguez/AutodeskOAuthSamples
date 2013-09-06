package com.autodesk.adn.common;

import android.content.Context;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.util.Log;

public final class Network {

	public static boolean isAvailable(final Context ctx) {
	    try {
	      final ConnectivityManager conMgr =
	          (ConnectivityManager) ctx.getSystemService(Context.CONNECTIVITY_SERVICE);
	      if (null != conMgr) {
	        final NetworkInfo i = conMgr.getActiveNetworkInfo();
	        return (null != i) && i.isConnected() && i.isAvailable();
	      }
	    } catch (final Exception ex) {
	      Log.e("Exception", ex.getMessage(), ex);
	    }
	    return false;
	  }
}
