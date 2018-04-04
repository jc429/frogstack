using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tide : MonoBehaviour {
	[SerializeField]
	bool enableTide;
	[SerializeField]
	int tideUpper;
	[SerializeField]
	int tideLower;
	[SerializeField]
	bool startAtHighTide;
	[SerializeField]
	bool startInMotion;

	Vector3 highTide;
	Vector3 lowTide;
	[Range(-1,1)]
	int tideShift = 0;
	Vector3 tideStart;
	Vector3 tideEnd;
	float tideShiftTime = 0;
	float tideShiftDuration = 1f;
	float tideTileSpeed = 0.3f;			//how long it takes to traverse one tile 

	// Use this for initialization
	void Start () {
		highTide = transform.position;
		highTide.y = tideUpper;
		lowTide = transform.position;
		lowTide.y = tideLower; 
		//tideShiftDuration = tideShiftDuration * (Mathf.Abs(tideUpper - tideLower));
		if(startAtHighTide){
			transform.position = highTide;
			if(startInMotion){
				tideShift = -1;
				tideStart = highTide;
				tideEnd = lowTide;
			}
		}
		else{
			transform.position = lowTide;
			if(startInMotion){
				tideShift = 1;
				tideStart = lowTide;
				tideEnd = highTide;
			}
		}
		Debug.Log(tideLower);
	}
	
	// Update is called once per frame
	void Update () {
		if(!enableTide){
			return;
		}
		if(tideShift != 0){	
			tideShiftTime += Time.deltaTime;
			Vector3 v = Vector3.Lerp(tideStart,tideEnd,tideShiftTime/tideShiftDuration);
			if(tideShiftTime >= tideShiftDuration){
				v = tideEnd;
				tideShiftTime = 0;
				tideShift = 0;
			}
			transform.position = v;
		}
		
		if(Input.GetMouseButtonDown(1)){
			if(tideShift == 0){
				if(transform.position == lowTide){
					SetTideShift(1);
				}
				else{
					SetTideShift(-1);
				}
			}
			else{
				SetTideShift(-1*tideShift);
			}
		}

		if(Input.GetMouseButtonDown(0)){
			SetTideShift(0);
		}
		
	}

	public void SetTideShift(int dir){
		dir = Mathf.Clamp(dir,-1,1);
		if(dir == tideShift){
			return;
		}
		
		tideStart = transform.position;
		if(dir < 0){
			tideEnd = lowTide;
			tideShift = dir;
		}
		else if(dir > 0){
			tideEnd = highTide;
			tideShift = dir;
		}
		else{	
			tideEnd = transform.position;
			if(tideShift > 0){
				tideEnd.y = Mathf.CeilToInt(transform.position.y);
			}
			else if(tideShift < 0){
				tideEnd.y = Mathf.FloorToInt(transform.position.y);
			}
		}
		tideShiftTime = 0;
		tideShiftDuration = Mathf.Abs(tideEnd.y - tideStart.y)*tideTileSpeed;
	}

}
