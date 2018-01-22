using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptedMovement : MonoBehaviour {
	FrogMovement _frogMovement;
	float stepPause;
	float pauseDuration = 0.2f;

	public bool startOnAwake = true;
	public bool looping;
	public Vector2[] steps;

	bool scripting = false;
	int currentStep = 0;
	// Use this for initialization
	void Start () {
		_frogMovement = GetComponent<FrogMovement>();
		_frogMovement.SetScriptLock(true);
		stepPause = pauseDuration;
		if (startOnAwake) {
			scripting = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
	//	_frogMovement.facing
		if (scripting && _frogMovement.Stable() && steps.Length > 0) {
			stepPause -= Time.deltaTime;
			if (stepPause <= 0) {
				stepPause = pauseDuration;
				PerformNextStep();
			}
		}
	}

	void PerformNextStep() {
		if (currentStep >= steps.Length) {
			if (looping) {
				currentStep = 0;
			}
			else {
				return;
			}
		}
		_frogMovement.facing = (int)steps[currentStep].x;
		_frogMovement.StartHop(steps[currentStep], true);
		currentStep++;
	}

	public void StartScripting() {
		scripting = true;
	}

	public void StopScripting() {
		scripting = false;
	}
}
