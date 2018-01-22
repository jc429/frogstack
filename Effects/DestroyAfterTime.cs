using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour {
	public float timeToLive;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		timeToLive -= Time.deltaTime;
		if (timeToLive <= 0) {
			Destroy(this.gameObject);
		}
	}
}
