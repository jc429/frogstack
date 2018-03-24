using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfo : MonoBehaviour {
	public int worldNum;		//unused, but helpful info to set
	public int levelNum;		//unused, but helpful info to set
	public AudioClip bgm;		//music to play in the bg of this level
	public GameObject bg;		//maybe used for shaking things? idk
	// Use this for initialization
	void Start () {
		if(bgm != null)
			GameManager.GetAudioManager().PlayTrack(bgm);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
