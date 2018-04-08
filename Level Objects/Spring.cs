using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour {


	int objs;
	bool isActive;

	GameObject connectedObj;

	[SerializeField]
	GameObject springHead;

	// Use this for initialization
	void Start () {
		connectedObj = null;
	}
	
	// Update is called once per frame
	void Update () {
		if(connectedObj != null){
			Vector3 v = springHead.transform.position;
			v.y = connectedObj.transform.position.y - 0.5f;
			v.y = Mathf.Min(v.y,transform.position.y);
			springHead.transform.position = v;
			
		}
	}



	
	void OnTriggerEnter(Collider other) {
		objs++;
		if(other.gameObject.GetComponentInParent<FrogMovement>() != null){
			connectedObj = other.gameObject;
			other.gameObject.GetComponentInParent<FrogMovement>().StoreSpringJump();
		}
	}
	void OnTriggerExit(Collider other) {
		objs--;
		connectedObj = null;
	}

	public bool Activated() {
		return isActive;
	}
}
