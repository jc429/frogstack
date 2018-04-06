﻿#pragma warning disable 0162 //only meant to surpress the warning about unreachable code for erasing data -- TODO DELETE LATER
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

enum SceneDest {
	SD_Null,
	SD_CurrentScene,
	SD_NextScene,
	SD_MainMenu,
	SD_GameEnd,
};

//TODO: Clean this bad boy UP 
public class GameManager : MonoBehaviour {
	public const int _levelCeiling = 10;				//max height allowed 
	public const bool _allowFrogClicking = true;		//allow player to click on frogs to switch control to them (debug tool)
	const bool ERASE_ALL_DATA_ON_START = false;
	
	public static GameManager managerInstance;			//the active instance of the game manager
	AudioManager audioManager;

	ScreenTransition screenTransition;					//transition camera
	SceneDest sceneDest;								//scene to transition to
	bool sceneTransitioning;							//are we transitioning?

	[SerializeField]
	GameObject frogPrefab;								//frog prefab (i.e. what we spawn/play as)
	Vector3 spawnPos;									//position to spawn frogs at
	static float hopspeed = 1;							//how fast frogs hop
	
	FrogCounter frogCounter;							//UI keeping track of the frogs
	int frogCount;
	int hopCount;
	[SerializeField]
	GameObject cursorPrefab;							//cursor that hovers over the currently controlled frog
	GameObject cursorInstance;

	

	float spawnTimer;
	float spawnCooldown = 0.2f;
	public FrogMovement currentFrog;
	List<FrogMovement> oldFrogs;
	List<NoSpawn> noSpawnZones;
	List<SpawnZone> spawnZones;
	public bool spawnQueued;
	Vector3 queuedPos;
	

	public PauseMenu pauseMenu;
	public OptionsMenu optionsMenu;
	public bool paused;

	

	// Use this for initialization
	void Awake() {
		if (managerInstance == null) {
			managerInstance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else if(managerInstance != this) {
			Destroy(this.gameObject);
		}
		if (ERASE_ALL_DATA_ON_START) {
			LevelManager.EraseAllLevelData();
		}
		LevelManager.LoadLevelData();
		SceneManager.sceneLoaded += StartLevel;

		oldFrogs = new List<FrogMovement>();
		noSpawnZones = new List<NoSpawn>();
		spawnZones = new List<SpawnZone>();
		audioManager = GetComponentInChildren<AudioManager>();
	}
	void Start() {
	//	screenTransition = GameObject.FindGameObjectWithTag("EffectsCamera").GetComponent<ScreenTransition>();
	//	screenTransition.StartTransitionIn();
	//	Screen.SetResolution(288, 160, false);
	//	SpawnFrog();
	}
	
	// Update is called once per frame
	void Update () {
		if (paused) {
			Time.timeScale = 0;
		}
		else {
			Time.timeScale = 1;
		}
		if (sceneTransitioning) {
			if (screenTransition.FadedOut()) {
				ClearFrogs();	
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
						SceneManager.LoadScene(0);
						break;

					case SceneDest.SD_GameEnd:
						SceneManager.LoadScene(1);
						break;

					default:
						break;
				}
				sceneTransitioning = false;
				sceneDest = SceneDest.SD_Null;
			}
		}
		else{	//if not transitioning 
			if (spawnTimer > 0) {
				spawnTimer -= Time.deltaTime;
			}
			if (Input.GetKeyDown(KeyCode.Space)
			|| Input.GetKeyDown(KeyCode.LeftShift)) {
				if(currentFrog.Stable()){
					bool insz = false; 
					foreach(SpawnZone sz in spawnZones){
						if(sz.Activated()){
							spawnQueued = true;
							queuedPos = sz.transform.position;
							queuedPos.z = 0;
							Vector3 v = currentFrog.transform.position + new Vector3(0,1);
							currentFrog.HopToTarget(v);
							insz = true;
							break;
						}
					}
					if(!insz){
						SpawnFrog(spawnPos);
					}
				}
			}
			if (currentFrog != null && !currentFrog.IsInMotion() && !currentFrog.isUnderwater) {
				if ((Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.UpArrow)) && currentFrog.stackAbove != null) {
					FrogMovement fm = currentFrog.stackAbove.GetComponent<FrogMovement>();
					if (fm != null) {
						SetCurrentFrog(fm);
					}
				}
				else if ((Input.GetKeyDown(KeyCode.S)||Input.GetKeyDown(KeyCode.DownArrow)) && currentFrog.stackBelow != null) {
					FrogMovement fm = currentFrog.stackBelow.GetComponent<FrogMovement>();
					if (fm != null) {
						SetCurrentFrog(fm);
					}
				}
			}
			if (Input.GetKeyDown(KeyCode.P)
			|| Input.GetKeyDown(KeyCode.Escape)) {
				
				if (pauseMenu != null) {
					if (optionsMenu != null && optionsMenu.open) {
						optionsMenu.CloseOptionsMenu();
					}
					else {
						pauseMenu.Toggle();
					}
				}
			}
			if (Input.GetKeyDown(KeyCode.R)) {
				ResetLevel();
			}
		}
	}

	void LateUpdate(){
		if(spawnQueued && currentFrog.destReached){
			currentFrog.destReached = false;
			spawnQueued = false;
			SpawnFrog(queuedPos,true);
		}
	}

	void SpawnFrog(Vector3 pos, bool overrideStabilityCheck = false) {
		if (frogPrefab == null) {
			Debug.Log("Error - no frog!");
		}
		if (spawnTimer > 0) {
			return;
		}
		if (currentFrog != null) {
			if (!currentFrog.Stable() && !overrideStabilityCheck) {
				return;
			}
			else {
				/*if (currentFrog.GetComponent<Movement>().moving)
					Debug.Log("grrrrr");
				else
					Debug.Log("argggg");*/
			}
			foreach (NoSpawn nsp in noSpawnZones) {
				if (nsp.Activated()) {
					Debug.Log("no spawning allowed!");
					return;
				}
			}
			

			if (currentFrog.isUnderwater) {
				currentFrog.Poof();
			}
			currentFrog.GetComponent<FrogMovement>().DeactivateFrog();
			oldFrogs.Add(currentFrog);
		}
	//	Debug.Log("BOOP?");
		currentFrog = Instantiate<GameObject>(frogPrefab).GetComponent<FrogMovement>();
		currentFrog.transform.position = pos;
		UpdateFrogCounter(oldFrogs.Count + 1);
		spawnTimer = spawnCooldown;
		currentFrog.ActivateFrog();
		SetCurrentFrog(currentFrog);


	}

	void ClearFrogs() {
		if (currentFrog != null) {
			GameObject.Destroy(currentFrog.gameObject);
			currentFrog = null;
		}
		foreach (FrogMovement frog in oldFrogs) {
			GameObject.Destroy(frog.gameObject);
		}
		oldFrogs.Clear();
		noSpawnZones.Clear();
		frogCount = 0;
		hopCount = 0;
	}

	public void SetCurrentFrog(FrogMovement f) {
		if (currentFrog != f) {
			if(currentFrog != null){
				currentFrog.DeactivateFrog();
				if (!oldFrogs.Contains(currentFrog)) {
					oldFrogs.Add(currentFrog);
				}
			}
			currentFrog = f;
			f.ActivateFrog();
			if (oldFrogs.Contains(currentFrog)) {
				oldFrogs.Remove(currentFrog);
			}
			if (cursorInstance != null) {
				cursorInstance.GetComponent<AudioSource>().Play();
			}
		}
		if (cursorInstance == null) {
			if (cursorPrefab == null) {
				Debug.Log("Error: no cursor prefab");
				return;
			}
			cursorInstance = GameObject.Instantiate(cursorPrefab);
		}

		cursorInstance.transform.parent = currentFrog.transform;
		cursorInstance.transform.localPosition = Vector3.zero;
		cursorInstance.SetActive(true);
	}


	void UpdateFrogCounter(int num){
		frogCount = num;
		if(frogCounter != null){
			frogCounter.UpdateFrogCount(frogCount);
		}
	}

	public void AddToHopCount(int ct = 1){
		hopCount += ct;
	}


	//add a no-spawn zone to the list
	public void AddNoSpawnZone(NoSpawn nsp){
		noSpawnZones.Add(nsp);
	}

	public void AddSpawnZone(SpawnZone sz){
		spawnZones.Add(sz);
	}

	public void SetSpawnPoint(Vector3 pos) {
	//	Debug.Log("Spawn Point Set" + pos);
		pos.z = 0;
		spawnPos = pos;
		if (currentFrog == null) {
			SpawnFrog(spawnPos);
		}
	}

	public void HaltBGM() {
		audioManager.Stop();
	}


	public bool CheckColumnFree(int col){
		bool free = true;
			foreach(FrogMovement f in oldFrogs){
				if(f.Grounded() || f.stackBelow != null){
					continue;
				}
				if(Mathf.RoundToInt(f.transform.position.x) == col){
					free = false;
					return free;
				}
			}

		return free;
	}
	
	public void LockPlayerMovement(){
		if (currentFrog == null) {
			Debug.Log("no player!");
			return;
		}
		currentFrog.GetComponent<FrogMovement>().LockMovement();
	}
	
	public static void SetHopSpeed(float speed) {
		hopspeed = Mathf.Clamp(speed, 0.5f, 3);
		if (managerInstance.currentFrog != null) {
			managerInstance.currentFrog.GetComponent<FrogMovement>().SetHopSpeed(hopspeed);
		}
	}

	public static float GetHopSpeed(){
		return hopspeed;
	}

	public static bool IsTransitioning() {
		if(managerInstance.screenTransition == null){
			return false;
		}
		return managerInstance.sceneTransitioning || managerInstance.screenTransition.FadedOut();
	}

	public static void StartScreenTransitionOut(){
		if(managerInstance.screenTransition != null){
			managerInstance.screenTransition.StartTransitionOut();
		}
	}

	public static bool TransitionDone(){
		return(managerInstance.screenTransition != null && managerInstance.screenTransition.transitionDone);
	}
	
	public void SetScreenTransition(ScreenTransition st){
		screenTransition = st;
	}


	public void StartLevel(Scene scene, LoadSceneMode mode) {
		if(screenTransition != null)
			screenTransition.StartTransitionIn();
		
	}

	public void ResetGame() {

	}

	public void ResetLevel() {
		Debug.Log("Resetting");
		if (currentFrog != null) {
			currentFrog.GetComponent<FrogMovement>().LockMovement();
		}

		//cleanup
		noSpawnZones.Clear();
		oldFrogs.Clear();	


		if (screenTransition != null) {
			screenTransition.StartTransitionOut();
			sceneTransitioning = true;
			sceneDest = SceneDest.SD_CurrentScene;
		}
		else if(!sceneTransitioning)
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

	}

	public void AdvanceLevel() {
		CompleteLevel();
		if (currentFrog != null) {
			currentFrog.GetComponent<FrogMovement>().LockMovement();
		}
		if (screenTransition != null) {
			screenTransition.StartTransitionOut();
			sceneTransitioning = true;
			
			sceneDest = SceneDest.SD_NextScene;

			//remove later
			if (LevelManager.curLevel >= 7) {
				sceneDest = SceneDest.SD_GameEnd;
			}
		}
	}

	void CompleteLevel() {
		Debug.Log("Level complete!");
		LevelManager.SetRecord(LevelManager.curWorld, LevelManager.curLevel, frogCount, hopCount);
		LevelManager.CompleteCurrentLevel();
		LevelManager.UnlockNextLevel();
	}

	public void GoToMainMenu() {
		if (currentFrog != null) {
			currentFrog.GetComponent<FrogMovement>().LockMovement();
		}
		if (screenTransition != null) {
			screenTransition.StartTransitionOut();
			sceneTransitioning = true;
			sceneDest = SceneDest.SD_MainMenu;
		}
		else
			SceneManager.LoadScene(0);
	}



	//public void SetBGMSrc(BackgroundMusic src) {
	//	bgm = src;
//	}
	

	public void ClearAllData() {
		Debug.Log("clearing all data");
		LevelManager.EraseAllLevelData();
		LevelManager.LoadLevelData();
		ResetLevel();
	}

	public static AudioManager GetAudioManager(){
		return managerInstance.audioManager;
	}

	public static void SetFrogCounter(FrogCounter fc){
		managerInstance.frogCounter = fc;
	}
}



