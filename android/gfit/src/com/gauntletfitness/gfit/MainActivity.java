package com.gauntletfitness.gfit;

import android.app.Activity;
import android.os.Bundle;
import android.view.KeyEvent;
import android.view.Menu;
import android.webkit.WebChromeClient;
import android.webkit.WebSettings;
import android.webkit.WebSettings.RenderPriority;
import android.webkit.WebView;
import android.webkit.WebViewClient;

public class MainActivity extends Activity {

	private WebView view;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);
		view = (WebView) findViewById(R.id.webview);
		if (savedInstanceState != null) {
			view.restoreState(savedInstanceState);
		} else {
			//view.clearCache(true);
			view.loadUrl("http://gfit.azurewebsites.net/");
		}
		view.setWebViewClient(new WebViewClient());
		view.setWebChromeClient(new WebChromeClient());
		view.getSettings().setRenderPriority(RenderPriority.HIGH);
		view.getSettings().setCacheMode(WebSettings.LOAD_NO_CACHE);
		view.getSettings().setDomStorageEnabled(true);
		view.getSettings().setAppCacheEnabled(true);
		view.getSettings().setDatabaseEnabled(true);
		view.getSettings().setDomStorageEnabled(true);
		WebSettings webSettings = view.getSettings();
		webSettings.setJavaScriptEnabled(true);
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		getMenuInflater().inflate(R.menu.main, menu);
		return true;
	}

	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {
		// Check if the key event was the Back button and if there's history
		if ((keyCode == KeyEvent.KEYCODE_BACK) && view.canGoBack()) {
			view.goBack();
			return true;
		}
		// If it wasn't the Back key or there's no web page history, bubble up
		// to the default
		// system behavior (probably exit the activity)
		return super.onKeyDown(keyCode, event);
	}

	protected void onSaveInstanceState(Bundle outState) {
		view.saveState(outState);
	}

}
