using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObj : MonoBehaviour {

	protected int objs = 0;
	protected bool isActive = false;
	protected bool prevActive = false;


	protected List<FrogMovement> connectedFrogs = new List<FrogMovement>();

	// Use this for initialization
	void Start () {
		connectedFrogs = new List<FrogMovement>();
		Start2();
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

	protected virtual void Start2(){}
	protected virtual void Update2(){}

	void OnTriggerEnter(Collider other) {
		FrogMovement fm = other.gameObject.GetComponentInParent<FrogMovement>();
		if(fm != null && !connectedFrogs.Contains(fm)){
			connectedFrogs.Add(fm);
			objs++;
		//	connectedFrog = other.gameObject.GetComponentInParent<FrogMovement>();
		}
	}
	void OnTriggerExit(Collider other) {
		FrogMovement fm = other.gameObject.GetComponentInParent<FrogMovement>();
		if(fm != null && connectedFrogs.Contains(fm)){
			connectedFrogs.Remove(fm);
			objs--;
		//	connectedFrog = null;
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
