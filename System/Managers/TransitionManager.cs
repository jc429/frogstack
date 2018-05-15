using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class TransitionManager{
		
	static ScreenTransition screenTransition;					//transition camera
	static SceneDest sceneDest;								//scene to transition to
	static bool sceneTransitioning;							//are we transitioning?

	public static bool IsTransitioning() {
		if(screenTransition == null){
			return false;
		}
		return sceneTransitioning || screenTransition.FadedOut();
	}

	public static void StartScreenTransitionOut(){
		if(screenTransition != null){
			screenTransition.StartTransitionOut();
		}
	}

	public static bool TransitionDone(){
		return(screenTransition != null && screenTransition.transitionDone);
	}
	
	public static void SetScreenTransition(ScreenTransition st){
		screenTransition = st;
	}

	public static ScreenTransition GetScreenTransition(){
		return screenTransition;
	}

	public static void TransitionToLevel(SceneDest dest){
		if (screenTransition != null) {
			screenTransition.StartTransitionOut();
			sceneTransitioning = true;
			sceneDest = dest;
		//	Debug.Log("Transitioning to " + dest);
		}
		else if(!sceneTransitioning){	//if we dont have a screentransition, just reload the scene ??
			Debug.Log("Transition camera not found");
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}

	public static void GoToDestination(){
		switch(sceneDest){
			case SceneDest.SD_CurrentScene:
				Debug.Log("Loading Scene" + SceneManager.GetActiveScene().buildIndex);
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
				break;

			case SceneDest.SD_NextScene:
				//Debug.Log("Loading Scene" + SceneManager.GetActiveScene().buildIndex + 1);
				LevelManager.LoadNextLevel();
				//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
				break;

			case SceneDest.SD_MainMenu:
				SceneManager.LoadScene(Config.mainMenuSceneNo);
				break;

			case SceneDest.SD_GameEnd:
				SceneManager.LoadScene(Config.gameEndSceneNo);
				break;

			default:
				break;
		}
		sceneTransitioning = false;
		sceneDest = SceneDest.SD_Null;
	}
}
