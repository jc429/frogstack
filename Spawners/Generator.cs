using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : CycleObj {

	[SerializeField]
	GameObject genObject;	
	GameObject lastGenerated;

	// Use this for initialization
	void Start () {
		countdown = offset;
	}
	
	// Update is called once per frame
	new void Update () {
		base.Update();
	}

	public override void StartAction() {
		Generate();
	}

	public void Generate() {
		GameObject obj = GameObject.Instantiate(genObject);
		obj.transform.position = this.transform.position;
		lastGenerated = obj;
	}

	public void SetGenObj(GameObject obj){
		genObject = obj;
	}

	public GameObject GetLastGenerated(){
		return lastGenerated;
	}
}
