using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogAnims : MonoBehaviour {
	FrogMovement _movement;
	SpriteRenderer _sprite;
	[SerializeField]
	Sprite[] spriteList;
	[SerializeField]
	Gradient gradient;
	//0 idle 1 hopping
	// Use this for initialization
	void Start() {
		_sprite = GetComponent<SpriteRenderer>();
		_movement = GetComponentInParent<FrogMovement>();

		//SetRandomColor();
	}

	// Update is called once per frame
	void Update() {
		_sprite.sprite = spriteList[GetAnimFrame()];

		SortSprites();
		
	}

	//returns which frame of the spritesheet to draw
	int GetAnimFrame(){		
		int frame = 0;
		if (!_movement.Grounded()) {
			frame += 1;
		}
		if (_movement.isUnderwater) {
			frame += 2;
		}
		if(_movement.stackBelow != null){
			frame = 4;
		}
		return frame;
	}

	//handles which order to draw everything in
	void SortSprites(){
		if(_movement.stackBelow != null){
			SpriteRenderer subspr = _movement.stackBelow.GetComponentInChildren<SpriteRenderer>();
			if(subspr != null){
				_sprite.sortingOrder = subspr.sortingOrder +1;
			}
		}
		else{
			_sprite.sortingOrder = 0;
		}
	}

	void SetRandomColor() {
		float value = Random.value;
		Color color = gradient.Evaluate(value);
		_sprite.color = color;
	}


}
