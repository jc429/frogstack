using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogCounter : MonoBehaviour {
	public Vector3 inPos;
	public Vector3 outPos;
	RectTransform _rtransform;
	float moveDuration = 0.8f;
	float moveTime;
	bool moving;
	bool moveIn;
	// Use this for initialization
	void Start () {
		_rtransform = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
		if (moving) {
			moveTime += Time.deltaTime;
			if (moveIn) {
				_rtransform.localPosition = Vector3.Lerp(outPos, inPos, (moveTime / moveDuration));
			}
			else {
				_rtransform.localPosition = Vector3.Lerp(inPos, outPos, (moveTime / moveDuration));
			}
			if (moveTime >= moveDuration) {
				moving = false;
			}
		}
		if (Input.GetMouseButtonDown(0)) {
			MoveIn();
		}
		else if (Input.GetMouseButtonDown(1)) {
			MoveOut();
		}
	}

	public void MoveIn() {
		if (_rtransform.localPosition == inPos) {
			return;
		}
		if (moving) {
			moveIn = true;
			moveTime = moveDuration - moveTime;
			return;
		}
		moving = true;
		moveIn = true;
		moveTime = 0;
	}

	public void MoveOut() {
		if (_rtransform.localPosition == outPos) {
			return;
		}
		if (moving) {
			moveIn = false;
			moveTime = moveDuration - moveTime;
			return;
		}
		moving = true;
		moveIn = false;
		moveTime = 0;
	}
}
