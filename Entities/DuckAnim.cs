using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckAnim : MonoBehaviour {
	DuckMovement duckMovement;
	SpriteRenderer _sprite;
	public Sprite[] spriteList;
	// Use this for initialization
	void Start() {
		duckMovement = GetComponentInParent<DuckMovement>();
		_sprite = GetComponent<SpriteRenderer>();
		
	}
	
	// Update is called once per frame
	void Update() {
		int frame = 0;
		if (duckMovement.HasBread()) {
			frame = 2;
		}
		_sprite.sprite = spriteList[frame];
	}
}
