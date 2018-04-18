using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessionGhost : TriggerObj {

	[SerializeField]
	float movespeed;

	bool moving; 
	FrogMovement targetFrog;

	// Use this for initialization
	void Start () {
		moving = true;
	}
	
	// Update is called once per frame
	protected override void Update2 () {
		if(targetFrog != null && targetFrog.IsStable() && !targetFrog.IsInMotion()){
			GameManager.managerInstance.SetCurrentFrog(targetFrog);
			Destroy(this.gameObject);
		}
		if(moving){
			Vector3 v = transform.position;
			v.x += movespeed * Time.deltaTime;
			transform.position = v;
		}
		else{
			if(targetFrog != null){
				transform.position = Vector3.MoveTowards(transform.position,targetFrog.transform.position,Mathf.Abs(movespeed)*Time.deltaTime);
			}
		}
	}

	protected override void TriggerEnter(){
		Debug.Log("Boo!");
		moving = false;
		targetFrog = connectedFrog;
	}
	
}
