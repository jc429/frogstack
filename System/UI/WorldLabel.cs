using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldLabel : MonoBehaviour {
	Text text;
	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		text.text = "World " + (LevelManager.curWorld + 1) + "-" + (LevelManager.curLevel + 1);
	}
}
