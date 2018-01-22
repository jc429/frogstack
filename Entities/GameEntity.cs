using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntity : MonoBehaviour {

	public bool isUnderwater;
	public int watersTouching;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	protected void Update () {
		isUnderwater = (watersTouching > 0);
	}
}
