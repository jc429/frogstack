using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour {

	AudioSource _audio;

	int objs;

	GameObject connectedFrog;

	[SerializeField]
	GameObject springHead;


	// Use this for initialization
	void Start () {
		connectedFrog = null;
		_audio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if(connectedFrog != null){
			Vector3 v = springHead.transform.position;
			v.y = connectedFrog.transform.position.y - 0.5f;
			springHead.transform.position = v;
			
		}

		Vector3 loc = springHead.transform.localPosition;
		loc.y = Mathf.Clamp(loc.y,-0.5f,0);
		springHead.transform.localPosition = loc;
	}



	
	void OnTriggerEnter(Collider other) {
		if(other.transform.position.y < transform.position.y){
			return;
		}

		objs++;
		if(other.gameObject.GetComponentInParent<FrogMovement>() != null){
			connectedFrog = other.gameObject;
			other.gameObject.GetComponentInParent<FrogMovement>().StoreSpringJump();
		}
	}
	void OnTriggerExit(Collider other) {
		objs--;
		if(_audio != null && connectedFrog != null){
			_audio.Play();
		}
		connectedFrog = null;
	}

	public bool Pressed() {
		return objs > 0;
	}
}
