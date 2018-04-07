using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour {


	int objs;
	bool isActive;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}



	
	void OnTriggerEnter(Collider other) {
		objs++;
		if(other.gameObject.GetComponentInParent<FrogMovement>() != null){
			Debug.Log("ye");
			other.gameObject.GetComponentInParent<FrogMovement>().StoreSpringJump();
		}
	}
	void OnTriggerExit(Collider other) {
		objs--;
	}

	public bool Activated() {
		return isActive;
	}
}
