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
public class GameManager : MonoBehaviour {
	public AudioManager audioManager;

	SceneDest sceneDest;
	bool transitioning;

	bool ERASE_ALL_DATA_ON_START = false;

	public Score score;
	public GameObject frogPrefab;
	public static GameManager managerInstance;
	public ScreenTransition screenTransition;
	public int frogCount;
	public int hopCount;

	float spawnTimer;
	float spawnCooldown = 0.2f;
	public FrogMovement currentFrog;
	List<FrogMovement> oldFrogs;
	List<NoSpawn> noSpawnZones;

	public Vector3 spawnPos;

	public BackgroundMusic bgm;
	public PauseMenu pauseMenu;
	public OptionsMenu optionsMenu;
	public bool paused;

	public GameObject cursorPrefab;
	GameObject cursor;
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
		audioManager = GetComponent<AudioManager>();
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
		if (transitioning) {
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
				transitioning = false;
				sceneDest = SceneDest.SD_Null;
			}
		}
		else{	//if not transitioning 
			if (spawnTimer > 0) {
				spawnTimer -= Time.deltaTime;
			}
			if (Input.GetKeyDown(KeyCode.Space)
			|| Input.GetKeyDown(KeyCode.LeftShift)) {
			//	Debug.Log("arg");
				SpawnFrog();
			}
			if (currentFrog != null && !currentFrog.isUnderwater) {
				if (Input.GetKeyDown(KeyCode.W) && currentFrog.stackAbove != null) {
					FrogMovement fm = currentFrog.stackAbove.GetComponent<FrogMovement>();
					if (fm != null) {
						SetCurrentFrog(fm);
					}
				}
				else if (Input.GetKeyDown(KeyCode.S) && currentFrog.stackBelow != null) {
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

	void SpawnFrog() {
		if (frogPrefab == null) {
			Debug.Log("Error - no frog!");
		}
		if (spawnTimer > 0) {
			return;
		}
		if (currentFrog != null) {
			if (!currentFrog.Stable()) {
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
		currentFrog.transform.position = spawnPos;
		frogCount = (oldFrogs.Count + 1);
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
			currentFrog.DeactivateFrog();
			if (!oldFrogs.Contains(currentFrog)) {
				oldFrogs.Add(currentFrog);
			}
			currentFrog = f;
			f.ActivateFrog();
			if (oldFrogs.Contains(currentFrog)) {
				oldFrogs.Remove(currentFrog);
			}
			if (cursor != null) {
				cursor.GetComponent<AudioSource>().Play();
			}
		}
		if (cursor == null) {
			if (cursorPrefab == null) {
				Debug.Log("Error: no cursor prefab");
				return;
			}
			cursor = GameObject.Instantiate(cursorPrefab);
		}

		cursor.transform.parent = currentFrog.transform;
		cursor.transform.localPosition = Vector3.zero;
		cursor.SetActive(true);
	}

	public void AddNoSpawnZone(NoSpawn nsp){
		noSpawnZones.Add(nsp);
	}

	public void SetSpawnPoint(Vector3 pos) {
	//	Debug.Log("Spawn Point Set" + pos);
		pos.z = 0;
		spawnPos = pos;
		if (currentFrog == null) {
			SpawnFrog();
		}
	}



	public static bool IsTransitioning() {
		return managerInstance.transitioning || managerInstance.screenTransition.FadedOut();
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
		if (screenTransition != null) {
			screenTransition.StartTransitionOut();
			transitioning = true;
			sceneDest = SceneDest.SD_CurrentScene;
		}
		else if(!transitioning)
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

	}

	public void AdvanceLevel() {
		CompleteLevel();
		if (currentFrog != null) {
			currentFrog.GetComponent<FrogMovement>().LockMovement();
		}
		if (screenTransition != null) {
			screenTransition.StartTransitionOut();
			transitioning = true;
			
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
			transitioning = true;
			sceneDest = SceneDest.SD_MainMenu;
		}
		else
			SceneManager.LoadScene(0);
	}


	public void LockPlayerMovement(){
		if (currentFrog == null) {
			Debug.Log("no player!");
			return;
		}
		currentFrog.GetComponent<FrogMovement>().LockMovement();
	}

	public void SetBGMSrc(BackgroundMusic src) {
		bgm = src;
	}
	public void HaltBGM() {
		audioManager.Stop();
	}

	public static float hopspeed = 1;
	public static void SetHopSpeed(float speed) {
		hopspeed = Mathf.Clamp(speed, 0.5f, 3);
		if (managerInstance.currentFrog != null) {
			managerInstance.currentFrog.GetComponent<FrogMovement>().SetHopSpeed(hopspeed);
		}
	}

	public void ClearAllData() {
		Debug.Log("clearing all data");
		LevelManager.EraseAllLevelData();
		LevelManager.LoadLevelData();
		ResetLevel();
	}
}



