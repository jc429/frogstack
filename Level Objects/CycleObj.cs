using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleObj : TriggeredObj {
	public int cycle = 1;			//spawn an object every x times 
	public int offset = 0;
	protected int countdown; 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	protected void Update() {
		if (DEBUG && Input.GetMouseButtonDown(1)) {
			AdvanceCycle();
		}
		
	}

	public override void ReceiveButtonPress() {
		AdvanceCycle();
	}

	public override void ReceiveButtonRelease() {
		RevertCycle();
	}

	public void AdvanceCycle() {
		countdown++;
		if (countdown >= cycle) {
			countdown = 0;
			StartAction();
		}
	}

	public void RevertCycle() {
		countdown--;
		if (countdown < 0) {
			countdown = cycle - 1;
		}
	}

	public virtual void StartAction() {

	}

	public virtual void StopAction() {

	}
}
