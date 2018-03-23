using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour{
	public Font[] fonts;
	// Use this for initialization
	void Start () {
		for(int i = 0; i < fonts.Length; i++){
			fonts[i].material.mainTexture.filterMode = FilterMode.Point;
		}
	}
	
}
