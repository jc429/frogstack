using UnityEngine;
using System.Collections;

public class LevelPage : MonoBehaviour {
	public int worldID;
	// Use this for initialization
	void Start() {
		foreach (LevelButton lb in GetComponent<RectTransform>().GetComponentsInChildren<LevelButton>()) {
			int lid = lb.levelID;
			lb.SetLocked(!LevelManager.LevelUnlocked(worldID, lid));
		}
	}

	// Update is called once per frame
	void Update() {

	}
}
