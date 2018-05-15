using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour {
	LevelPage _levelPage;
	public Text _buttonText;
	bool isLocked;
	public int levelID;

	void Awake() {
		_levelPage = GetComponent<RectTransform>().parent.GetComponent<LevelPage>();
	}
	// Use this for initialization
	void Start() {
		_buttonText.text = "Level " + _levelPage.worldID + "-" + levelID;
	}

	// Update is called once per frame
	void Update() {

	}

	public void SetLocked(bool locked) {
		isLocked = locked;
		ColorBlock cblock = GetComponent<Button>().colors;
		if (locked) {
			//TODO: Make this prettier
			cblock.normalColor = PColor.CreateColor(64, 64, 88);
		}
		GetComponent<Button>().interactable = !locked;
		GetComponent<Button>().colors = cblock;
	}

	public void LoadAssignedLevel() {
		if (isLocked) return;
		//LevelManager.LoadLevel(_levelPage.worldID, levelID);

		if(MainMenuController.controllerInstance.selectedWorld == _levelPage.worldID 
		&& MainMenuController.controllerInstance.selectedLevel == levelID){
			MainMenuController.controllerInstance.LoadSelectedLevel();
		}
		else{
			MainMenuController.controllerInstance.SelectLevel(_levelPage.worldID, levelID);
		}
	}

}
