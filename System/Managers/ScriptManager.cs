using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManager : MonoBehaviour {
	public ScriptedMovement[] movementList;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void ActivateAndStartScript(int id) {
		if (id < 0 || id >= movementList.Length) {

			return;
		}
		ScriptedMovement sm = movementList[id];
		if (sm == null) {
			return;
		}
		sm.gameObject.SetActive(true);
		sm.StartScripting();

	}
}
