using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrogCounter : MonoBehaviour {
	static bool lockPosition = false;

	public Vector3 inPos;
	public Vector3 outPos;
	RectTransform _rtransform;
	float moveDuration = 0.7f;
	float moveTime;
	bool moving;
	bool moveIn;

	float sustainDuration = 1f;
	float sustainTime = 0;

	public Text fcount;
	// Use this for initialization
	void Start () {
		_rtransform = GetComponent<RectTransform>();
		GameManager.SetFrogCounter(this);
		
		if(lockPosition){
			_rtransform.localPosition = inPos;
		}
		else{
			_rtransform.localPosition = outPos;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(!lockPosition){
			if (moving) {
				sustainTime = 0;
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
			else{
				if(_rtransform.localPosition == inPos){
					sustainTime += Time.deltaTime;
					if(sustainTime >= sustainDuration){
						MoveOut();
					}
				}
			}
		}
		
	}

	public void MoveIn() {
		if(lockPosition){
			return;
		}
		if (_rtransform.localPosition == inPos) {
			sustainTime = 0;
			return;
		}
		if (moving) {
			if(moveIn){
				return;
			}
			moveIn = true;
			moveTime = moveDuration - moveTime;
			return;
		}
		moving = true;
		moveIn = true;
		moveTime = 0;
	}

	public void MoveOut() {
		if(lockPosition){
			return;
		}
		if (_rtransform.localPosition == outPos) {
			return;
		}
		if (moving) {
			if(!moveIn){
				return;
			}
			moveIn = false;
			moveTime = moveDuration - moveTime;
			return;
		}
		moving = true;
		moveIn = false;
		moveTime = 0;
	}

	public void UpdateFrogCount(int numfrogs){
		fcount.text = "x" + numfrogs;
		MoveIn();
	}
}
