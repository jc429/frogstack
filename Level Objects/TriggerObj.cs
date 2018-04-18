using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObj : MonoBehaviour {

	protected int objs = 0;
	protected bool isActive = false;
	protected bool prevActive = false;

	protected FrogMovement connectedFrog = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		UpdateActivity();
		Update2();
	}

	protected void UpdateActivity(){
		prevActive = isActive;
		isActive = (objs > 0);
		if(isActive && !prevActive){
			TriggerEnter();
		}
		else if(!isActive && prevActive){
			TriggerExit();
		}
	}

	protected virtual void Update2(){

	}

	void OnTriggerEnter(Collider other) {
		
		if(other.gameObject.GetComponentInParent<FrogMovement>() != null && connectedFrog == null){
			objs++;
			connectedFrog = other.gameObject.GetComponentInParent<FrogMovement>();
		}
	}
	void OnTriggerExit(Collider other) {
		if(other.gameObject.GetComponentInParent<FrogMovement>() == connectedFrog && connectedFrog != null){
			objs--;
			connectedFrog = null;
		}
	}

	public bool IsActivated() {
		return (objs > 0);
	}

	protected virtual void TriggerEnter(){
	
	}

	protected virtual void TriggerExit(){

	}

}
