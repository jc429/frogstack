using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScore : MonoBehaviour {
	public Text frogtxt;
	public Text hoptxt;
	// Use this for initialization
	void Start () {
		frogtxt.text = "" + LevelManager.GetTotalFrogs();
		hoptxt.text = "" + LevelManager.GetTotalActions();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
