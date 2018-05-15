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
	Vector3 titleCardInactivePos = new Vector3(375, 0);

	Vector3 levelsSidebarActivePos = new Vector3(-150, 0);
	Vector3 levelsSidebarInactivePos = new Vector3(-305, 0);

	Vector3 levelsAreaActivePos = new Vector3(164,0);
	Vector3 levelsAreaInactivePos = new Vector3(164, -250);

	bool animating;
	bool atTitleScreen;
	float animTime = 0;
	const float animDuration = 0.2f;

	public int selectedWorld;
	public int selectedLevel;

	[SerializeField] 
	Text sidebarTitle;
	[SerializeField] 
	Text sidebarRecord;
	[SerializeField] 
	Image sidebarCleared;
	[SerializeField] 
	Image sidebarGem;
	[SerializeField]
	Sprite gemSpriteActive;
	[SerializeField] 
	Sprite gemSpriteInactive;

	[SerializeField]
	Button firstButton;

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
		atTitleScreen = true;
		UpdateLevelInfo();
		SelectLevel(1,1);
	}

	// Update is called once per frame
	void Update() {
		if (transitioning) {
			if (TransitionManager.TransitionDone()) {
				transitioning = false;
				GoToSelectedLevel();
			}
		}
		if (animating) {
			if (atTitleScreen) {
			//	Debug.Log(titleCard.localPosition);
				animTime += Time.deltaTime;
				if (!PMath.CloseTo(titleCard.localPosition, titleCardInactivePos)) {
					titleCard.localPosition = Vector3.Lerp(titleCardActivePos, titleCardInactivePos, animTime / animDuration);
					if (animTime >= animDuration) {
						animTime = 0;
						titleCard.localPosition = titleCardInactivePos;
					}
				}
				else if (!PMath.CloseTo(levelsArea.localPosition, levelsAreaActivePos)){
					levelsArea.localPosition = Vector3.Lerp(levelsAreaInactivePos, levelsAreaActivePos, animTime / animDuration);
					if (animTime > animDuration) {
						animTime = 0;
						levelsArea.localPosition = levelsAreaActivePos;
					}
				}
				else if(!PMath.CloseTo(levelsSidebar.localPosition, levelsSidebarActivePos)){
					levelsSidebar.localPosition = Vector3.Lerp(levelsSidebarInactivePos, levelsSidebarActivePos, animTime / animDuration);
					if (animTime > animDuration) {
						animTime = 0;
						levelsSidebar.localPosition = levelsSidebarActivePos;
						animating = false;
						atTitleScreen = false;
					}
				}
			}
			else {
				animTime += Time.deltaTime;
				if (!PMath.CloseTo(levelsSidebar.localPosition, levelsSidebarInactivePos)){
					levelsSidebar.localPosition = Vector3.Lerp(levelsSidebarActivePos, levelsSidebarInactivePos, animTime / animDuration);
					if (animTime > animDuration) {
						animTime = 0;
						levelsSidebar.localPosition = levelsSidebarInactivePos;
					}
				}
				else if (!PMath.CloseTo(levelsArea.localPosition, levelsAreaInactivePos)){
					levelsArea.localPosition = Vector3.Lerp(levelsAreaActivePos, levelsAreaInactivePos, animTime / animDuration);
					if (animTime > animDuration) {
						animTime = 0;
						levelsArea.localPosition = levelsAreaInactivePos;
					}
				}
				else if (!PMath.CloseTo(titleCard.localPosition, titleCardActivePos)){
					titleCard.localPosition = Vector3.Lerp(titleCardInactivePos, titleCardActivePos, animTime / animDuration);
					if (animTime > animDuration) {
						animTime = 0;
						titleCard.localPosition = titleCardActivePos;
						animating = false;
						atTitleScreen = true;
					}
				}
			}
		}
	}

	public void EnterLevelSelectAnim() {
		animating = true;
		firstButton.Select();
		animTime = 0;
	}


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
		
		TransitionManager.StartScreenTransitionOut();
		transitioning = true;
	}

	public void GoToSelectedLevel() {
		LevelManager.LoadLevel(selectedWorld, selectedLevel);
	}

	/*********************************************************/

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
		
		if(LevelManager.LevelCompleted(selectedWorld, selectedLevel)){
			if(LevelManager.GetLevelGemCollected(selectedWorld,selectedLevel)){
				sidebarGem.sprite = gemSpriteActive;
			}
			else{
				sidebarGem.sprite = gemSpriteInactive;
			}
		}
	}


}
