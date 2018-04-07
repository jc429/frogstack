using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepButton : MonoBehaviour {

	SpriteRenderer _sprite;
	[SerializeField]
	Sprite[] sprites;
	
	public bool requireHold;
	public bool stayPressed;

	[SerializeField]
	TriggeredObj[] connectedObjects;

	int objs;
	bool isActive;

	// Use this for initialization
	void Start () {
		_sprite = GetComponentInChildren<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (stayPressed && isActive) {
			return;
		}
		bool prevActive = isActive;
		isActive = (objs > 0);
		
		_sprite.sprite = (isActive ? sprites[1] : sprites[0]);

		if (isActive && !prevActive) {
			foreach (TriggeredObj to in connectedObjects) {
				to.ReceiveButtonPress();
			}
		}
		else if (!isActive && prevActive) {
			if (requireHold) {
				foreach (TriggeredObj to in connectedObjects) {
					to.ReceiveButtonRelease();
				}
			}
		}


	}



	void OnTriggerEnter(Collider other) {
		objs++;
	}
	void OnTriggerExit(Collider other) {
		objs--;
	}

	public bool Activated() {
		return isActive;
	}

}
