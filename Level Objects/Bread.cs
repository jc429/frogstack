using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bread : GameEntity {
	SpriteRenderer _sprite;
	Rigidbody _rigidbody;
	// Use this for initialization
	void Start () {
		_sprite = GetComponentInChildren<SpriteRenderer>();
		_rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	new void Update () {
		base.Update();
		_rigidbody.useGravity = !isUnderwater;
		_rigidbody.isKinematic = isUnderwater;
		if (isUnderwater) {
			_rigidbody.velocity = Vector3.zero;
			Vector3 v = new Vector3();
			v.y = 0.125f * Mathf.Sin(Time.time*1.5f) + 0.5f;
			_sprite.transform.localPosition = v;
		}
	}
}
