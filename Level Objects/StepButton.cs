using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepButton : TriggerObj {

	SpriteRenderer _sprite;
	[SerializeField]
	Sprite[] sprites;
	
	public bool requireHold;
	public bool stayPressed;
	bool isPressed;

	[SerializeField]
	ButtonConnectedObj[] connectedObjects;

	

	// Use this for initialization
	void Start () {
		_sprite = GetComponentInChildren<SpriteRenderer>();
	}
	
	// Update is called once per frame
	protected override void Update2 () {
		if (stayPressed && isPressed) {
			return;
		}
		isPressed = isActive;
		
		_sprite.sprite = (isPressed ? sprites[1] : sprites[0]);

		/*if (isActive && !prevActive) {
			foreach (ButtonConnectedObj to in connectedObjects) {
				to.ReceiveButtonPress();
			}
		}
		else if (!isActive && prevActive) {
			if (requireHold) {
				foreach (ButtonConnectedObj to in connectedObjects) {
					to.ReceiveButtonRelease();
				}
			}
		}*/


	}

	protected override void TriggerEnter(){
		foreach (ButtonConnectedObj to in connectedObjects) {
			to.ReceiveButtonPress();
		}
	}
	protected override void TriggerExit(){
		foreach (ButtonConnectedObj to in connectedObjects) {
			to.ReceiveButtonRelease();
		}
	}

}
