using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RenderSettings {

	static bool fullScreen = false;
	static int renderScale = 1;	
	public static int defaultRenderScale = 4;//default to 1280x720? idk 

	public static void SetRenderScale(int scale){
		//max out at 1440p i guess
		scale = Mathf.Clamp(scale,1,8);
		renderScale = scale;
		UpdateWindow();
	}

	public static void SetFullscreen(bool fs){
		fullScreen = fs;
		UpdateWindow();
	}

	public static void ToggleFullscreen(){
		fullScreen = !fullScreen;
		UpdateWindow();
	}

	static void UpdateWindow(){
		Display.main.SetRenderingResolution(320,180);
		if(fullScreen){
			Screen.SetResolution(1920,1080,true);
		}
		else
			Screen.SetResolution(320*renderScale,180*renderScale,fullScreen);
		Debug.Log("Window resolution set to " + Display.main.renderingWidth + " x " + Display.main.renderingHeight);
	}

}
