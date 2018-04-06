using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZone : MonoBehaviour {
GameManager gameManager;
	public int objs;
	// Use this for initialization
	void Start () {
		gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
		gameManager.AddSpawnZone(this);
		objs = 0;
	}

	void Awake(){
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

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
	}
}
