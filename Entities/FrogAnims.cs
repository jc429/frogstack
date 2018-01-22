using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogAnims : MonoBehaviour {
	FrogMovement frogMovement;
	SpriteRenderer _sprite;
	public Sprite[] spriteList;
	[SerializeField]
	Gradient gradient;
	//0 idle 1 hopping
	// Use this for initialization
	void Start() {
		_sprite = GetComponent<SpriteRenderer>();
		frogMovement = GetComponentInParent<FrogMovement>();

		//SetRandomColor();
	}

	// Update is called once per frame
	void Update() {
		int frame = 0;
		if (!frogMovement.Grounded()) {
			frame += 1;
		}
		if (frogMovement.isUnderwater) {
			frame += 2;
		}
		_sprite.sprite = spriteList[frame];
	}

	void SetRandomColor() {
		float value = Random.value;
		Color color = gradient.Evaluate(value);
		_sprite.color = color;
	}
}
