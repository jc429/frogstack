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
	public static bool DEBUG_MODE = true;
	
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

	Gem currentGem;										//each level has a gem to collect
	

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
		if (DEBUG_MODE && ERASE_ALL_DATA_ON_START) {
			LevelManager.EraseAllLevelData();
		}
		audioManager = GetComponentInChildren<AudioManager>();

		LevelManager.LoadLevelData();
		SceneManager.sceneLoaded += StartLevel;

		oldFrogs = new List<FrogMovement>();
		noSpawnZones = new List<NoSpawn>();
		spawnZones = new List<SpawnZone>();
		CleanLevel();


 
	}

	void Start() {
	//	screenTransition = GameObject.FindGameObjectWithTag("EffectsCamera").GetComponent<ScreenTransition>();
	//	screenTransition.StartTransitionIn();
	//	Screen.SetResolution(288, 160, false);
	//	SpawnFrog();
		if(currentGem == null && DEBUG_MODE){
			Debug.Log("error - no gem detected in this level");
		}
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
			if (SpawnTriggered()) {
				AttemptSpawn();
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

			if(DEBUG_MODE && Input.GetKeyDown(KeyCode.RightAlt)){
				ScreenCapture.CaptureScreenshot("C:/Users/edibl_000/Pictures/games/frog stack/GameCap/test.png");
				
				Debug.Log("Screen capture saved!");
			}
			if(DEBUG_MODE){
				if(Input.GetKeyDown(KeyCode.Keypad0)){
					RenderSettings.ToggleFullscreen();
				}
				if(Input.GetKeyDown(KeyCode.Keypad1)){
					RenderSettings.SetRenderScale(1);
				}
				if(Input.GetKeyDown(KeyCode.Keypad2)){
					RenderSettings.SetRenderScale(2);
				}
				if(Input.GetKeyDown(KeyCode.Keypad3)){
					RenderSettings.SetRenderScale(3);
				}
				if(Input.GetKeyDown(KeyCode.Keypad4)){
					RenderSettings.SetRenderScale(4);
				}
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



	/********************* Frog Spawning *********************/
	
	bool SpawnTriggered(){
		return Input.GetKeyDown(KeyCode.Space)
			|| Input.GetKeyDown(KeyCode.LeftShift);
	}

	void AttemptSpawn(){
		if(currentFrog == null){
			SpawnFrog(spawnPos);
		}
		else if(currentFrog.Stable()){
			bool insz = false; 
			foreach(SpawnZone sz in spawnZones){
				if(sz.Activated() && PMath.CloseTo(currentFrog.transform.position,sz.transform.position)){
					Vector3 top = currentFrog.transform.position + currentFrog.DistanceToStackTop();
					if(Physics.Raycast(top,Vector3.up,0.6f,Layers.GetGroundMask(false))){
						//fail bc we hit ceiling
						Debug.Log("theres a ceiling in the way");
						return;
					}
					spawnQueued = true;
					queuedPos = sz.transform.position;
					queuedPos.z = 0;
					Vector3 v = currentFrog.transform.position + new Vector3(0,1);
					currentFrog.HopToTarget(v);
					insz = true;
					break;
				}
				else if(sz.Activated()){	
					foreach(FrogMovement f in oldFrogs){
						if(PMath.CloseTo(f.transform.position,sz.transform.position)){
							Vector3 top = f.transform.position + f.DistanceToStackTop();
							if(Physics.Raycast(top,Vector3.up,0.6f,Layers.GetGroundMask(false))){
								//fail bc we hit ceiling
								Debug.Log("theres a ceiling in the way");
								return;
							}
							spawnQueued = true;
							queuedPos = sz.transform.position;
							queuedPos.z = 0;
							SetCurrentFrog(f);
							Vector3 v = f.transform.position + new Vector3(0,1);
							f.HopToTarget(v);
							insz = true;
							break;
						}
					}
					break;
				}
				else{Debug.Log("nani");}
			}
			if(!insz){
				SpawnFrog(spawnPos);
			}
		}
		else{
			Debug.Log("unstable");
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
				currentFrog.Disappear();
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
		currentFrog.Poof();

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
		spawnZones.Clear();
		frogCount = 0;
		hopCount = 0;
	}

	public static void SetFrogCounter(FrogCounter fc){
		managerInstance.frogCounter = fc;
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

	/********************* Spawn Zones *********************/

	//add a no-spawn zone to the list
	public void AddNoSpawnZone(NoSpawn nsp){
		noSpawnZones.Add(nsp);
	}

	public void AddSpawnZone(SpawnZone sz){
		spawnZones.Add(sz);
	//	Debug.Log("Spawn zone registered");
	}

	public void SetSpawnPoint(Vector3 pos) {
	//	Debug.Log("Spawn Point Set" + pos);
		pos.z = 0;
		spawnPos = pos;
		if (currentFrog == null) {
			SpawnFrog(spawnPos);
		}
	}

	/********************* Gems *********************/
	
	public void RegisterGem(Gem g){
		currentGem = g;
	}

	public bool CheckGemCollected(){
		return currentGem.IsCollected();
	}

	/********************* Screen Transitions *********************/

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


	/********************* Level Management *********************/

	public void StartLevel(Scene scene, LoadSceneMode mode) {
		if(screenTransition != null)
			screenTransition.StartTransitionIn();
		
	}

	public void ResetGame() {

	}

	public void CleanLevel(){
		spawnZones.Clear();
		noSpawnZones.Clear();
		oldFrogs.Clear();	
		currentGem = null;
	}

	public void ResetLevel() {
		Debug.Log("Resetting");
		if (currentFrog != null) {
			currentFrog.GetComponent<FrogMovement>().LockMovement();
		}

		CleanLevel();


		if (screenTransition != null) {
			screenTransition.StartTransitionOut();
			sceneTransitioning = true;
			sceneDest = SceneDest.SD_CurrentScene;
		}
		else if(!sceneTransitioning){
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

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
		bool collected = currentGem.IsCollected();
		LevelManager.SetRecord(LevelManager.curWorld, LevelManager.curLevel, frogCount, hopCount, collected);
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

	public void ClearAllData() {
		Debug.Log("clearing all data");
		LevelManager.EraseAllLevelData();
		LevelManager.LoadLevelData();
		ResetLevel();
	}

	public static AudioManager GetAudioManager(){
		return managerInstance.audioManager;
	}

	
	public void HaltBGM() {
		audioManager.Stop();
	}

}



