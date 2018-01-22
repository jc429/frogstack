using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {
	// Use this for initialization
	void Start() {
		GameManager.managerInstance.SetSpawnPoint(transform.position);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
