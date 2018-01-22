using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {
	public static MainMenuController controllerInstance;
//	public ScreenTransition screenTransition;
	bool transitioning;

	public RectTransform levelsArea;
	public RectTransform levelsSidebar;
	public RectTransform titleCard;

	Vector3 titleCardActivePos = Vector3.zero;
	Vector3 titleCardInactivePos = new Vector3(1500, 0);

	Vector3 levelsSidebarActivePos = new Vector3(-640, 0);
	Vector3 levelsSidebarInactivePos = new Vector3(-1220, 0);

	Vector3 levelsAreaActivePos = new Vector3(640,0);
	Vector3 levelsAreaInactivePos = new Vector3(640, -1000);

	bool animating;
	bool atMainMenu;
	float animTime;
	const float animDuration = 0.2f;


	void Awake() {
		if (controllerInstance == null) {
			controllerInstance = this;
		//	DontDestroyOnLoad(this.gameObject);
		}
		else if (controllerInstance != this) {
			Destroy(this.gameObject);
		}
	}
	// Use this for initialization
	void Start() {
		titleCard.localPosition = titleCardActivePos;
		levelsSidebar.localPosition = levelsSidebarInactivePos;
		levelsArea.localPosition = levelsAreaInactivePos;
		atMainMenu = true;
		UpdateLevelInfo();
		SelectLevel(1,1);
	}

	// Update is called once per frame
	void Update() {
		if (transitioning) {
			if (GameManager.managerInstance.screenTransition != null && GameManager.managerInstance.screenTransition.transitionDone) {
				transitioning = false;
				GoToSelectedLevel();
			}
		}
		if (animating) {
			if (atMainMenu) {
				animTime += Time.deltaTime;
				if (titleCard.localPosition != titleCardInactivePos) {
					titleCard.localPosition = Vector3.Lerp(titleCardActivePos, titleCardInactivePos, animTime / animDuration);
					if (animTime > animDuration) {
						animTime = 0;
						titleCard.localPosition = titleCardInactivePos;
					}
				}
				else if (levelsArea.localPosition != levelsAreaActivePos) {
					levelsArea.localPosition = Vector3.Lerp(levelsAreaInactivePos, levelsAreaActivePos, animTime / animDuration);
					if (animTime > animDuration) {
						animTime = 0;
						levelsArea.localPosition = levelsAreaActivePos;
					}
				}
				else if(levelsSidebar.localPosition != levelsSidebarActivePos){
					levelsSidebar.localPosition = Vector3.Lerp(levelsSidebarInactivePos, levelsSidebarActivePos, animTime / animDuration);
					if (animTime > animDuration) {
						animTime = 0;
						levelsSidebar.localPosition = levelsSidebarActivePos;
						animating = false;
						atMainMenu = false;
					}
				}
			}
			else {
				animTime += Time.deltaTime;
				if (levelsSidebar.localPosition != levelsSidebarInactivePos) {
					levelsSidebar.localPosition = Vector3.Lerp(levelsSidebarActivePos, levelsSidebarInactivePos, animTime / animDuration);
					if (animTime > animDuration) {
						animTime = 0;
						levelsSidebar.localPosition = levelsSidebarInactivePos;
					}
				}
				else if (levelsArea.localPosition != levelsAreaInactivePos) {
					levelsArea.localPosition = Vector3.Lerp(levelsAreaActivePos, levelsAreaInactivePos, animTime / animDuration);
					if (animTime > animDuration) {
						animTime = 0;
						levelsArea.localPosition = levelsAreaInactivePos;
					}
				}
				else if (titleCard.localPosition != titleCardActivePos) {
					titleCard.localPosition = Vector3.Lerp(titleCardInactivePos, titleCardActivePos, animTime / animDuration);
					if (animTime > animDuration) {
						animTime = 0;
						titleCard.localPosition = titleCardActivePos;
						animating = false;
						atMainMenu = true;
					}
				}
			}
		}
	}

	public void EnterLevelSelectAnim() {
		animating = true;
		animTime = 0;
	}

	public int selectedWorld;
	public int selectedLevel;

	public void SelectLevel(int world, int level) {
		selectedWorld = world;
		selectedLevel = level;
		UpdateLevelInfo();
	//	Debug.Log("Level " + selectedWorld + "-" + selectedLevel + " selected.");
	}

	public void LoadSelectedLevel(){
		if (selectedWorld == 0 || selectedLevel == 0) {
			Debug.Log("No level Selected");
			return;
		}
		Debug.Log("Selected level " + selectedWorld + "-" + selectedLevel);
		GameManager.managerInstance.screenTransition.StartTransitionOut();
		transitioning = true;
	}

	public void GoToSelectedLevel() {
		LevelManager.LoadLevel(selectedWorld, selectedLevel);
	}

	/*********************************************************/
	public Text sidebarTitle;
	public Text sidebarRecord;
	public Image sidebarCleared;

	public void UpdateLevelInfo() {
		if (selectedWorld == 0 || selectedLevel == 0) {
			sidebarTitle.text = "";
			sidebarRecord.text = "";
			return;
		}

		sidebarTitle.text = "Lv " + selectedWorld + "-" + selectedLevel;
		if (LevelManager.LevelCompleted(selectedWorld, selectedLevel)
		&& (LevelManager.GetLevelRecordFrogs(selectedWorld, selectedLevel) > 0)) {

			sidebarRecord.text = "Record: \n"
				+ LevelManager.GetLevelRecordFrogs(selectedWorld, selectedLevel) + " frogs\n"
				+ LevelManager.GetLevelRecordAttempts(selectedWorld, selectedLevel) + " hops";
			sidebarCleared.gameObject.SetActive(true);

		}
		else {
			if (LevelManager.GetLevelRecordFrogs(selectedWorld, selectedLevel) <= 0
			&& LevelManager.LevelCompleted(selectedWorld, selectedLevel)) {
				Debug.Log("Error regarding saved score");
			}
			sidebarRecord.text = "";
			sidebarCleared.gameObject.SetActive(false);
		}
	}


}
