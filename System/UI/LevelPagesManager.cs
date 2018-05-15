using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelPagesManager : MonoBehaviour {
	RectTransform _rectTransform;
	const int pageSeparation = 1800/4;
	public RectTransform[] levelPages;
	public int selectedPage = 0;
	Vector3 startingPosition;

	public Button backButton;
	public Button forwardButton;
	public Button playButton;

	Vector3 moveSrc;
	Vector3 moveDest;
	bool moving;
	float moveTime;
	const float moveDuration = 0.6f;
	int cycleDir;

	void Awake() {
		_rectTransform = GetComponent<RectTransform>();
		startingPosition = _rectTransform.localPosition;
	//	Debug.Log(startingPosition);
	}
	// Use this for initialization
	void Start() {
		selectedPage = 0;
		moving = false;

		if (levelPages.Length == 0)
			Debug.Log("Error: No Pages Found!");
		int pagepos = 0;
		foreach (RectTransform page in levelPages) {
			if (page == null) continue;
			page.localPosition = new Vector3(pagepos, 9);
			pagepos += pageSeparation;
		}
	//	_rectTransform.localPosition = Vector3.zero;
	//	_rectTransform.localPosition = startingPosition;

	}

	// Update is called once per frame
	void Update() {
		if (moving) {
			playButton.interactable = false;
			backButton.interactable = false;
			forwardButton.interactable = false;
			moveTime += Time.deltaTime;
			_rectTransform.localPosition = Vector3.Lerp(moveSrc, moveDest, moveTime / moveDuration);
			if (moveTime >= moveDuration) {
				_rectTransform.localPosition = moveDest;
				moving = false;
				playButton.interactable = true;
				backButton.interactable = (selectedPage > 0);
				forwardButton.interactable = (selectedPage < levelPages.Length - 1);
				if(cycleDir > 0 && forwardButton.interactable){
					forwardButton.Select();
				} 
				else if(cycleDir < 0 && backButton.interactable){
					backButton.Select();
				}
				else{
					playButton.Select();
				}
			//	MainMenuController.controllerInstance.SelectLevel(levelPages[selectedPage].GetComponent<LevelPage>().worldID, 1);
			}
		}
		else {
			playButton.interactable = true;
			backButton.interactable = (selectedPage > 0);
			forwardButton.interactable = (selectedPage < levelPages.Length - 1);
		}
	}

	public void CyclePage(int dir) {
		dir = PMath.GetSign(dir);
		if (selectedPage + dir >= levelPages.Length)
			return;
		if (selectedPage + dir < 0)
			return;
		selectedPage += dir;

		moveSrc = _rectTransform.localPosition;
		moveDest = startingPosition + new Vector3(-1 * pageSeparation * selectedPage, 0);
		moveTime = 0;
		moving = true;
		cycleDir = dir;
	}
}
