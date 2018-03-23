using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {
	Text text;
	// Use this for initialization
	void Start() {
//		GameManager.managerInstance.score = this;
		text = GetComponent<Text>();
		//	text.color = PColor.CreateColor(255,255,180);
	}

	// Update is called once per frame
	void Update() {
		SetFrogCount(GameManager.managerInstance.frogCount, GameManager.managerInstance.hopCount);
	}

	public void SetFrogCount(int fc, int hc) {
	//	text.text = "Hops:  " + hc + "\nFrogs: " + fc;
		text.text = "x" + fc;
	}
}
