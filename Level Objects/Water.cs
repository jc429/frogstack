using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {
	
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if (other.GetComponentInParent<GameEntity>() != null)
			other.GetComponentInParent<GameEntity>().watersTouching++;
	}
	void OnTriggerExit(Collider other) {
		if (other.GetComponentInParent<GameEntity>() != null)
			other.GetComponentInParent<GameEntity>().watersTouching--;
	}
}
