using UnityEngine;
using System.Collections;

public class FrogMovement : Movement {
	[SerializeField]
	bool DEBUG_LOCK_MOVEMENT;
	
	//	float deadzone = 0.1f;
	const float basicHopTime = 0.25f;
	[Range(0.1f, 3)]
	public float speedMultiplier = 1f;

	LayerMask gmask = Layers.GetGroundMask(true);
	LayerMask platmask = Layers.GetGroundMask(false);

	Rigidbody _rigidbody;
	SpriteRenderer _sprite;
	FrogSounds _audio;

	[SerializeField]	
	GameObject smokePrefab;		//when a frog dies this shows up

	// hopping variables
/*	Vector3 hopTimer.start;			//position to hop from
	Vector3 hopTimer.end;				//position to land at
	float hopTimer.duration;			//total hop time
	float hopTimer.time;				//elapsed hop time
*/
	Timer hopTimer;

	public Vector3 targDir = Vector3.forward;
	[SerializeField]
	Transform moveTarget = null;
	float inputTimer = 0;					//how long you have been holding a direction
	const float inputThreshold = 0.08f;		//how long you need to be holding a direction to move instead of turn
	int moveHoldTimer = 0;					//briefly used to pause movement so you "stick" to a tile for a moment like an irl frog

	bool scriptLock;
	bool activeFrog;

	public bool destReached;				//flag set to true on the frame a frog reaches its destination hop

	// Use this for initialization
	new void Start() {
		base.Start();
		_audio = GetComponent<FrogSounds>();
		_rigidbody = GetComponent<Rigidbody>();
		_rigidbody.freezeRotation = true;
		if (_sprite == null) {
			_sprite = GetComponentInChildren<SpriteRenderer>();
		}
		_sprite.flipX = (facing < 0);

		SetHopSpeed(GameManager.GetHopSpeed());
		watersTouching = 0;

		if (DEBUG_LOCK_MOVEMENT) {
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
			StartHop(inputs);
			
		}

		if (moving) {
			//transform.position = Vector3.MoveTowards(transform.position, target.position, movespeed);
			BasicHop();
			//Debug.Log(Time.deltaTime);
			if (transform.position == hopTimer.end) {
				//target = null;
				moving = false;
				destReached = true;
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
		if (Grounded() && !stackBelow){
			//v.y = transform.position.y;
			transform.position = Vector3.MoveTowards(transform.position, v, 0.05f);
		}
		else if (!stackBelow) {
			Vector3 lp = (transform.localPosition);
			lp.x = Mathf.Round(lp.x);
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, lp, 0.05f);
		}
		else {
			if (stackBelow != null && stackBelow.GetComponent<ConveyorMovement>() != null) {
				ConveyorMovement cm = stackBelow.GetComponent<ConveyorMovement>();
				if (cm.beltsTouching == 0 || cm.moveSpeed != 0) {
					//Debug.Log("we in motion");
					return;
				}
			}
			else if(stackBelow != null && !stackBelow.IsInMotion()){
				Vector3 lp =  PMath.RoundToInts(transform.localPosition);
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, lp, 0.05f);
			}
		}

		//if we're not the active frog, there's nothing left to do here 
		if(!activeFrog){	
			return;
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
		//Debug.Log(inputs);

		if ((inputs.x != 0 || inputs.y != 0) && PMath.CloseTo(transform.position,v)) {
			transform.position = v;
			targDir = Vector3.zero;
			//targDir.y = transform.position.y;

			// set targdir to int values based on input axes 
			if (inputs.x > 0)
				targDir.x = 1;
			if (inputs.x < 0)
				targDir.x = -1;
			if (isUnderwater && targDir.x == 0 && !scripted) {
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

				hopTimer.start = transform.position;
				Vector3 targpos = hopTimer.start + new Vector3(targDir.x, targDir.y, targDir.z);
				Vector3 stacktop = hopTimer.start + DistanceToStackTop();

				//raycast to check if we're gonna hit something solid

				//if underwater, dont allow swimming up/down into walls
				if (isUnderwater) {
					if (targDir.y < 0 && Physics.Raycast(transform.position, Vector3.down, raylen, platmask)) {
						targpos = hopTimer.start;
					}
					if (targDir.y > 0 && Physics.Raycast(stacktop, Vector3.up, raylen, gmask)) {
						targpos = hopTimer.start;
					}
				}
				
				//if there is a solid wall in front of us, try to jump on top of it
				if (Physics.Raycast(hopTimer.start, new Vector3(targDir.x, 0, 0), raylen, gmask)) {
					//if not at the ceiling
					if (!(transform.position.y > GameManager._levelCeiling)) {
						//try hopping upward
						targpos.y += 1;
						
						//if theres a solid wall directly above us, cancel the jump
						if (Physics.Raycast(stacktop, Vector3.up, raylen, gmask)) {
							Debug.Log("owie oof");
							targpos = hopTimer.start;
						}
						//TODO: make this jump not take place underwater
						else if (isUnderwater) {
							swimming = false;
						}
					//	Debug.Log("Hopping up");
					}
					//if at the ceiling, we're not allowed to move
					else {
						targpos = hopTimer.start;
					}
					//if the wall in front of us is too tall to hop up, cancel the jump
					//TODO: change to raycast for every frog in the stack 
					if (Physics.Raycast(hopTimer.start + Vector3.up, new Vector3(targDir.x, 0, 0), raylen, gmask)) {
						targpos = hopTimer.start;

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

				HopToTarget(targpos);
			}
		}
	}

	public void HopToTarget(Vector3 targpos){
		hopTimer.start = transform.position;
		destReached = false;
		//if there are falling frogs between us and our destination, cancel the hop until theyre gone
		if(!CheckValidMove(hopTimer.start,targpos)){
			targpos = hopTimer.start;
			return;
		}

		AttemptStackMovement(hopTimer.start,targpos - hopTimer.start,raylen,gmask);

		//Debug.Log("hop! " + (targpos - hopTimer.start) + swimming);
		if (targpos != hopTimer.start) {
			GameManager.managerInstance.AddToHopCount();
		}
		else{
			SetMoveDir(0);
		}

		if(stackAbove != null && targpos.x != hopTimer.start.x){
			//horizontal collision check for all objs stacked above
			Movement m = stackAbove; 
			Vector3 stackstart = hopTimer.start + Vector3.up;
			while (m != null) {
				Vector3 moveoffset = targpos - hopTimer.start;
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

		//Debug.Log("jumping from" + hopTimer.start + "to" + targpos);
		//check if target is a viable space to move to
		moveTarget.position = targpos;
		hopTimer.end = targpos;
		
		//	hopTimer.duration = basicHopTime;
		hopTimer.time = 0;

		if (stackBelow != null) {
			Unlink();
		}
		stackBelow = null;
		transform.parent = null;
		_rigidbody.isKinematic = false; //TODO: test if this fixes the "stuck in kinematic" issue when unstacking
		moving = true;
		if (!swimming) {
			_audio.PlayJumpSound();
		}
	}

	void BasicHop() {
		if(Time.deltaTime > 0.1f){
			Debug.Log("Time since last frame too long, ignoring frame (" + Time.deltaTime +")");
			return;
		}
		float height = 0.5f;
		hopTimer.time += Time.deltaTime;
		Vector3 currentPos = Vector3.Lerp(hopTimer.start, hopTimer.end, hopTimer.time / hopTimer.duration);
		//Unlink(Vector3.down);
		if (!swimming) {
			//TODO: change this to make a prettier arc 
			currentPos.y += height * Mathf.Sin(Mathf.Clamp01(hopTimer.time / hopTimer.duration) * Mathf.PI);
		}

		transform.position = currentPos;

		if (hopTimer.time > hopTimer.duration) {
			transform.position = hopTimer.end;
		}
	}

	public void SetHopSpeed(float speed) {
		speedMultiplier = speed;
		if (speedMultiplier <= 0)
			speedMultiplier = 0.01f;
		hopTimer.duration = (1 / speedMultiplier) * basicHopTime;
	}

	public void Poof() {
		GameObject smoke = GameObject.Instantiate(smokePrefab);
		smoke.transform.position = this.transform.position;

		if (stackAbove != null) {
			stackAbove.Unlink();
		}
		gameObject.SetActive(false);

	}
	

	bool CheckValidMove(Vector3 start, Vector3 end){
		bool valid = true;
		int len = Mathf.RoundToInt(end.x - start.x);
		for(int i = 0; i < Mathf.Abs(len); i++){
			int n = Mathf.RoundToInt(start.x + (Mathf.Sign(len) * (1+i)));
			if(!GameManager.managerInstance.CheckColumnFree(n)){
				valid = false;
				break;
			}
		}
		return valid;
	}

	public int AttemptStackMovement(Vector3 hopStart, Vector3 movementDir, float raylen, LayerMask gmask){
		if(movementDir.x == 0){
			return 0;
		}
		//Vector3 stacktop = hopStart + GetStackTop();
		int stacknum = Mathf.RoundToInt(DistanceToStackTop().y);
		int breakpoint = 0;
		bool stackbroken = false;
		while(!stackbroken){
			if(breakpoint > stacknum){
				//Debug.Log("stack top all clear");
				break;
			}
			Vector3 raystart = hopStart + new Vector3(0,breakpoint);
			if(movementDir.y > 0){
				//if the stack is hopping upward, shift the raycasts up one tile
				raystart.y += 1;
				//Debug.Log("hop up");
			}
			if(Physics.Raycast(raystart,new Vector3(movementDir.x,0,0),raylen,gmask)){
				stackbroken = true;
				//Debug.Log("stack broken at " + raystart);
				Movement m = this;
				for(int i = 0; i < breakpoint; i++){
					m = m.stackAbove;
					if(m == null){
						break;
					}
				}
				if(m != null){
					m.Unlink();				
				}
				break;
			}
			breakpoint++;
		}
		return breakpoint;
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
		if (GameManager._allowFrogClicking && !IsInMotion()) {
			GameManager.managerInstance.SetCurrentFrog(this);
		}
	}
}





///////////////*****************************************************///////////////////////

