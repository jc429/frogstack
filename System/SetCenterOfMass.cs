using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCenterOfMass : MonoBehaviour {
	public Rigidbody _rigidbody;
	public Transform centerOfMass;
	// Use this for initialization
	void Start() {
		_rigidbody.centerOfMass = centerOfMass.localPosition;
	}

	// Update is called once per frame
	void Update() {

	}
}
