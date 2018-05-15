using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoSpawn : TriggerObj {
	GameManager gameManager;
	// Use this for initialization
	protected override void Start2 () {
		gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
		gameManager.AddNoSpawnZone(this);
		objs = 0;
	}

	// Update is called once per frame
	void Update () {
		
	}
/*
	void OnTriggerEnter(Collider other) {
		if (other.GetComponent<FrogMovement>() == null) {
			if (other.GetComponentInParent<FrogMovement>() == null) {
				return;
			}
		}
		objs++;
	}
	void OnTriggerExit(Collider other) {
		if (other.GetComponent<FrogMovement>() == null) {
			if (other.GetComponentInParent<FrogMovement>() == null) {
				return;
			}
		}
		objs--;
	}

	public bool Activated() {
		return (objs > 0);
	}*/
}
