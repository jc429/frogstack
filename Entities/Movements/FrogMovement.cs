﻿using UnityEngine;
using System.Collections;

public class FrogMovement : Movement {
	public bool DEBUG;
	const bool clickEnabled = true;
	//	float deadzone = 0.1f;
	[Range(0.1f, 3)]
	public float speedMultiplier = 1f;
	const float basicHopTime = 0.25f;

	Rigidbody _rigidbody;
	SpriteRenderer _sprite;
	FrogSounds _audio;

	// hopping variables
	Vector3 hopStart;
	Vector3 hopEnd;
	float hopDuration;				//total hop time
	float hopTime;					//elapsed hop time

	public Vector3 targDir = Vector3.forward;
	public Transform moveTarget = null;
	float inputTimer = 0;					//how long you have been holding a direction
	const float inputThreshold = 0.08f;		//how long you need to be holding a direction to move instead of turn
	int moveHoldTimer = 0;					//briefly used to pause movement so you "stick" to a tile for a moment like an irl frog

	bool scriptLock;
	bool activeFrog;

	// Use this for initialization
	void Start() {
		_audio = GetComponent<FrogSounds>();
		_rigidbody = GetComponent<Rigidbody>();
		_rigidbody.freezeRotation = true;
		if (_sprite == null) {
			_sprite = GetComponentInChildren<SpriteRenderer>();
		}
		_sprite.flipX = (facing < 0);

		SetHopSpeed(GameManager.hopspeed);
		watersTouching = 0;

		if (DEBUG) {
			DeactivateFrog();
		}

	}

	// Update is called once per frame
	new void Update() {
		base.Update();
		if (isUnderwater && !scriptLock && !activeFrog && Stable()) {
			Poof();
		}
		/*		if (Input.GetAxis("Horizontal") > deadzone) {
					transform.Translate(moveSpeed * Time.deltaTime, 0, 0);
				}
				else if (Input.GetAxis("Horizontal") < -deadzone) {
					transform.Translate(-moveSpeed*Time.deltaTime,0,0);
				}
				_rigidbody.velocity = new Vector3(_rigidbody.velocity.x, Mathf.Max(_rigidbody.velocity.y, maxvelocity), _rigidbody.velocity.z);
		*/
		if (activeFrog && Input.GetKeyDown(KeyCode.C)) {
			_audio.PlayCroak();
		}

		if (!moving && GroundedWithoutFrogs()) {
			_rigidbody.isKinematic = true;
		}

		BasicMovement();
		if (facing != 0) {
			_sprite.flipX = (facing < 0);
		}
	}


	bool swimming;
	void BasicMovement() {
		float mx = 0, mz = 0;


		if (Grounded() || isUnderwater) {
			//always keep target on same level as us
			moveTarget.position = new Vector3(moveTarget.position.x, gameObject.transform.position.y, moveTarget.position.z);

			if (moveHoldTimer > 0)
				moveHoldTimer--;
			else {
				mx = Input.GetAxisRaw("Horizontal");
				mz = Input.GetAxisRaw("Vertical");
			}
		}

		if ((!isUnderwater && mx == 0) || (mx == 0 && mz == 0)) {
			inputTimer = 0;
		}
		else {
			inputTimer += Time.deltaTime;
		}

		Vector2 inputs;
		if (!moving) {	//if we are not currently moving, we may attempt to move
			inputs.x = PMath.GetSign(mx);
			inputs.y = PMath.GetSign(mz);
			if (mx != 0)
				inputs.y = 0;
			if (!MovementLocked() && PMath.GetSign(mx) != 0) {
				SetFacing(PMath.GetSign(mx));
			}
			/*
			Vector3 v = PMath.RoundToInts(transform.position);
			//if grounded and not moving, subtly slide to the nearest grid position
			if (Grounded() && !Stacked()) {
				//v.y = transform.position.y;
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, v, 0.05f);
			}
			else if (!Stacked()) {
				Vector3 lp = (transform.localPosition);
				lp.x = Mathf.Round(lp.x);
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, lp, 0.05f);
			}
			else {
				if (stackBelow != null && stackBelow.GetComponent<ConveyorMovement>() != null) {
					ConveyorMovement cm = stackBelow.GetComponent<ConveyorMovement>();
					if(cm.beltsTouching == 0 || cm.moveSpeed != 0)
						return;
				}
			}
			*/
			/*if (activeFrog && inputs.y != 0 && !isUnderwater) {
				if (Input.GetKeyDown(KeyCode.W) && stackAbove != null) {
					//TODO: cursor immediately goes to top of stack when pressing up bc each successive frog becomes active - fix
					FrogMovement fm = stackAbove.GetComponent<FrogMovement>();	
					if (fm != null) {
						GameManager.managerInstance.SetCurrentFrog(fm);
					}
				}
				if (Input.GetKeyDown(KeyCode.S) && stackBelow != null) {
					FrogMovement fm = stackBelow.GetComponent<FrogMovement>();
					if (fm != null) {
						GameManager.managerInstance.SetCurrentFrog(fm);
					}
				}
			}*/
			//else{
			StartHop(inputs);
			//}
		}

		if (moving) {
			//transform.position = Vector3.MoveTowards(transform.position, target.position, movespeed);
			BasicHop();

			if (transform.position == hopEnd) {
				//target = null;
				moving = false;
				_rigidbody.velocity = Vector3.zero;
				moveHoldTimer = 3;	//prevents moving too quickly for the game to process

				_rigidbody.useGravity = !isUnderwater;
			
				
			}
		}
		if (isUnderwater) {
			_rigidbody.useGravity = !isUnderwater;
			_rigidbody.velocity = new Vector3(_rigidbody.velocity.x*0.9f, _rigidbody.velocity.y*0.9f);
		}
	}

	public void StartHop(Vector2 inputs, bool scripted = false) {
		Vector3 v = PMath.RoundToInts(transform.position);
		//if grounded and not moving, subtly slide to the nearest grid position
		if (Grounded() && !Stacked()) {
			//v.y = transform.position.y;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, v, 0.05f);
		}
		else if (!Stacked()) {
			Vector3 lp = (transform.localPosition);
			lp.x = Mathf.Round(lp.x);
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, lp, 0.05f);
		}
		else {
			if (stackBelow != null && stackBelow.GetComponent<ConveyorMovement>() != null) {
				ConveyorMovement cm = stackBelow.GetComponent<ConveyorMovement>();
				if (cm.beltsTouching == 0 || cm.moveSpeed != 0) {
					return;
				}
			}
		}

		if (isUnderwater) { //if the trigger is telling us we are underwater
			//but if the tile below is water and the tile we are technically still in is NOT water
			if (Physics.Raycast(v, Vector3.down, raylen, Layers.GetWaterMask()) &&
			!Physics.Raycast(v + Vector3.down, Vector3.up, raylen, Layers.GetWaterMask())) {
				v.y -= 1;		//move down into the water	
			}
			transform.position = Vector3.MoveTowards(transform.position, v, 0.05f);
			Vector3 a = _rigidbody.velocity;
			a *= 0.75f;
			_rigidbody.velocity = a;
		}


		if (inputs == Vector2.zero) {
			return;
		}
	//	Debug.Log(inputs);

		if ((inputs.x != 0 || inputs.y != 0) && transform.position == v) {

			targDir = Vector3.zero;
			//targDir.y = transform.position.y;

			if (inputs.x > 0)
				targDir.x = 1;
			if (inputs.x < 0)
				targDir.x = -1;
			if (isUnderwater && targDir.x == 0) {
				if (inputs.y > 0)
					targDir.y = 1;
				if (inputs.y < 0)
					targDir.y = -1;
			}
			SetMoveDir(Mathf.RoundToInt(targDir.x));

			bool canmove = false;
			if (inputTimer > inputThreshold) {
				canmove = !MovementLocked();
			}
			if (scripted) {
				canmove = true;
			}

			if (canmove) {
				swimming = isUnderwater;

				hopStart = transform.position;
				Vector3 targpos = hopStart + new Vector3(targDir.x, targDir.y, targDir.z);
				Vector3 stacktop = hopStart + GetStackTop();

				//raycast to check if we're gonna hit something solid
				LayerMask gmask = Layers.GetGroundMask(true);
				LayerMask platmask = Layers.GetGroundMask(false);

				//if underwater, dont allow swimming up/down into walls
				if (isUnderwater) {
					if (targDir.y < 0 && Physics.Raycast(transform.position, Vector3.down, raylen, platmask)) {
						targpos = hopStart;
					}
					if (targDir.y > 0 && Physics.Raycast(stacktop, Vector3.up, raylen, gmask)) {
						targpos = hopStart;
					}
				}
				
				//if there is a solid wall in front of us, try to jump on top of it
				if (Physics.Raycast(transform.position, new Vector3(targDir.x, 0, 0), raylen, gmask)) {
					//if not at the ceiling
					if (!(transform.position.y > 4.5f)) {
						targpos.y += 1;
						//if theres a solid wall directly above us, cancel the jump
						if (Physics.Raycast(stacktop, Vector3.up, raylen, gmask)) {
							targpos = hopStart;
						}
						//TODO: make this jump not take place underwater
						else if (isUnderwater) {
							swimming = false;
						}
					}
					//if at the ceiling
					else {
						targpos = hopStart;
					}
					//if the wall in front of us is too tall to hop up
					if (Physics.Raycast(stacktop + Vector3.up, new Vector3(targDir.x, 0, 0), raylen, gmask)) {
						targpos = hopStart;
						if (isUnderwater) {
							swimming = true;
						}
					}
				}
				else if (!isUnderwater) {
					//if the space in front of us is free, check if the space below that is free 
					if (!Physics.Raycast(targpos, Vector3.down, raylen, platmask)) {
						//if there is an open space 2 tiles forward from us, and there is a solid tile below that 
						//and the player is not holding "down"
						if (Physics.Raycast(targpos + new Vector3(targDir.x, 0, 0), Vector3.down, raylen, platmask)
						&& !Physics.Raycast(targpos, new Vector3(targDir.x, 0, 0), raylen, gmask)
						&& (Input.GetAxisRaw("Vertical") >= 0)) {
							targpos.x += facing;
						}
						//if the tile in front of us is empty and the tile beneath that is empty
						else if (!Physics.Raycast(targpos, new Vector3(targDir.x, 0, 0), raylen, gmask)
						&& !Physics.Raycast(targpos + Vector3.down, new Vector3(targDir.x, 0, 0), 0.75f, gmask)
						&& !Physics.Raycast(targpos, Vector3.down, 0.75f, gmask)) {
							targpos.y -= 1;
						}
					}
				}
				else if (isUnderwater) {
					LayerMask wmask = Layers.GetWaterMask();
					//if the target pos is not a water tile and there is a water tile below it 
					if (!Physics.Raycast(targpos + Vector3.down, Vector3.up, raylen, wmask)
					&& Physics.Raycast(targpos, Vector3.down, raylen, wmask)
					&& !Physics.Raycast(targpos, Vector3.down, raylen, gmask)) {

						targpos.y -= 1;
						swimming = false;
					}
				}
				//Debug.Log("hop! " + (targpos - hopStart) + swimming);
				if (targpos != hopStart) {
					GameManager.managerInstance.hopCount++;
				}

				if(stackAbove != null && targpos.x != hopStart.x){
					//collision check for all objs stacked above
					Movement m = stackAbove; 
					Vector3 stackstart = hopStart + Vector3.up;
					while (m != null) {
						Vector3 moveoffset = targpos - hopStart;
						moveoffset.y = 0;
						
						if (Physics.Raycast(stackstart, moveoffset, raylen + (Mathf.Abs(moveoffset.x) - 1), gmask)) {
							//	m.stackBelow.Unlink(Vector3.up);
							m.Unlink();
							break;
							//TODO: loop for all stacks above 
							//TODO: fix relinking 
						}
						m = m.stackAbove;
						stackstart += Vector3.up;
					}
				}

				//Debug.Log("jumping from" + hopStart + "to" + targpos);
				//check if target is a viable space to move to
				moveTarget.position = targpos;
				hopEnd = targpos;
				
				//	hopDuration = basicHopTime;
				hopTime = 0;

				if (stackBelow != null) {
					Unlink();
				}
				stackBelow = null;
				transform.parent = null;
				//Debug.Log("'BANANA!' - Minion, 2017");
				_rigidbody.isKinematic = false; //TODO: test if this fixes the "stuck in kinematic" issue when unstacking
				moving = true;
				if (!swimming) {
					_audio.PlayJumpSound();
				}
			}
		}
	}

	void BasicHop() {
		float height = 0.5f;
		hopTime += Time.deltaTime;
		Vector3 currentPos = Vector3.Lerp(hopStart, hopEnd, hopTime / hopDuration);
		//Unlink(Vector3.down);
		if (!swimming) {
			//TODO: change this to make a prettier arc 
			currentPos.y += height * Mathf.Sin(Mathf.Clamp01(hopTime / hopDuration) * Mathf.PI);
		}

		transform.position = currentPos;

		if (hopTime > hopDuration) {
			transform.position = hopEnd;
		}
	}

	public void SetHopSpeed(float speed) {
		speedMultiplier = speed;
		if (speedMultiplier <= 0)
			speedMultiplier = 0.01f;
		hopDuration = (1 / speedMultiplier) * basicHopTime;
	}

	public GameObject smokePrefab;
	public void Poof() {
		GameObject smoke = GameObject.Instantiate(smokePrefab);
		smoke.transform.position = this.transform.position;

		if (stackAbove != null) {
			stackAbove.Unlink();
		}
		gameObject.SetActive(false);

	}
	


	

	public void ActivateFrog() {
		activeFrog = true;
		UnlockMovement();
		if (_rigidbody != null) {
			_rigidbody.isKinematic = false;
		}
	}

	public void DeactivateFrog() {
		LockMovement();
		if (_rigidbody != null) {
			_rigidbody.isKinematic = true;
		}
		activeFrog = false;
	}

	public void SetScriptLock(bool s){
		scriptLock = s;
	}

	void OnMouseDown() {
		if (clickEnabled) {
			GameManager.managerInstance.SetCurrentFrog(this);
		}
	}
}





///////////////*****************************************************///////////////////////
