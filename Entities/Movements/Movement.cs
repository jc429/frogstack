using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : GameEntity {
	protected const float raylen = 0.75f;
	LayerMask fmask = Layers.GetFrogMask();
	LayerMask objmask = Layers.GetObjectMask();
	public bool stackWithPuzzleObjects = false;

	public bool moving;						//are we moving at all
	bool moveLock;							//for externally locking movement
	protected int moveDir;					//which direction are we moving in
	public int facing = 1;


	public Movement stackAbove;
	public Movement stackBelow;
	public bool checkAbove = true;
	public bool checkBelow = true;
	Vector3 posoffset;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	new protected void Update () {
		base.Update();
		CheckStack();
		if (StackMoving()) {
		//	Debug.Log("Stack moving");
			if (stackBelow != null) {
			//	transform.localPosition = posoffset;	//?????????
			}
			//	Debug.Log("chugga");
			if (stackAbove != null) {
				Vector3 v = new Vector3(moveDir, 0, 0);
				LayerMask gmask = Layers.GetGroundMask(true);
				if (Physics.Raycast(transform.position + Vector3.up, v, raylen, gmask)) {
				//	Debug.Log("owie oof");
				//	stackAbove.Unlink(Vector3.down);
				//	Unlink(Vector3.up);
				}
			}
		}
		if(Stable() && stackBelow != null){
			if(Mathf.Abs(transform.localPosition.x) >= 0.9f){
				Debug.Log("safe to unlink");
				transform.parent = null;
			//	GetComponent<Rigidbody>().isKinematic = false;
				stackBelow = null;	
			 //	Unlink(Vector3.down);
			}
		}
		
	}

	public void CheckStack() {
	//	if (moving) return;
		LayerMask gmask = Layers.GetGroundMask(false);
		RaycastHit r = new RaycastHit();
		LayerMask stackMask = 0;
		if (stackWithPuzzleObjects) {
			stackMask = objmask;
		}
		else {
			stackMask = fmask;
		}
		if (checkBelow && stackBelow == null && !moving) {
			if (Physics.Raycast(transform.position, Vector3.down, out r, raylen, stackMask)
			/*&& !Physics.Raycast(transform.position, Vector3.down, out r, raylen, gmask)*/) {
				Movement other = r.collider.GetComponent<Movement>();
				if (other != null && other.checkAbove && other.stackAbove == null) {
					if (/*!IsInMotion() && */!other.IsInMotion()) {
						stackBelow = other;
						transform.parent = other.transform;
						//	posoffset = transform.localPosition;
						transform.localPosition = Vector3.up;
						//Debug.Log("a");
						GetComponent<Rigidbody>().isKinematic = true;
						Debug.Log("Linking Down, Stationary");
						
						other.stackAbove = this;

					}
					else if (Physics.Raycast(transform.position, new Vector3(other.moveDir, 0), out r, raylen, gmask)) {
						Debug.DrawRay(transform.position, new Vector3(other.moveDir, 0), Color.red, raylen);
					}
					else {
						Debug.DrawRay(transform.position, new Vector3(other.moveDir, 0), Color.white, raylen);
						Debug.Log(other.moveDir);
						stackBelow = other;
						transform.parent = other.transform;
						//	posoffset = transform.localPosition;
						transform.localPosition = Vector3.up;
						//Debug.Log("a");
						GetComponent<Rigidbody>().isKinematic = true;
						Debug.Log("Linking Down, Moving");
					}
					
				}
			}
			else {
				posoffset = Vector3.zero;
				stackBelow = null;
				transform.parent = null;
			//	GetComponent<Rigidbody>().isKinematic = false;
			}
		}
	/*	if (checkAbove && stackAbove == null) {
			r = new RaycastHit();
			if (Physics.Raycast(transform.position, Vector3.up, out r, raylen, stackMask)) {
				Movement other = r.collider.GetComponent<Movement>();
				if (other == null) {
					other = r.collider.GetComponentInParent<Movement>();
				}
				if (other != null && !other.moving && other.checkBelow && other.stackBelow == null) {
					stackAbove = other;
					Debug.Log("Linking Up");
				}
			}
			else {
				stackAbove = null;
			}
		}*/
	}

	//always unlinks downward, call from upper member of stack
	public virtual void Unlink() {
		if (stackBelow == null) {
			return;
		}
		if (stackBelow.stackAbove == this) {
			stackBelow.stackAbove = null;
		}
		transform.parent = null;
		stackBelow = null;
		posoffset = Vector3.zero;
		Rigidbody r = GetComponent<Rigidbody>();
		if (r) {
			r.isKinematic = false;
			r.velocity = Vector3.zero;
		}
	/*	Debug.Log("Un-Linking " + dir.y);
		if (dir.y > 0 && stackAbove != null) {
		//	stackAbove.Unlink(Vector3.down);
			stackAbove.transform.parent = null;
			stackAbove.GetComponent<Rigidbody>().isKinematic = false;
			stackAbove = null;

		}
		if (dir.y < 0 && stackBelow != null) {
		//	stackBelow.Unlink(Vector3.up);
			transform.parent = null;
			stackBelow = null;
			posoffset = Vector3.zero;
			Rigidbody r = GetComponent<Rigidbody>();
			if (r) {
				r.isKinematic = false;
				r.velocity = Vector3.zero;
			}
		}*/
	}

	public bool Stacked() {
		return (stackAbove != null || stackBelow != null);
	}

	public bool StackMoving() {
		Movement m = this;
		while (true) {
			if (m.moving) {
				return true;
			}
			if (m.stackBelow == null) {
				break;
			}
			m = m.stackBelow;
		}
		return false;
	}

	public void SetMoveDir(int x) {
		moveDir = x;
		if (stackAbove != null) {
			stackAbove.SetMoveDir(x);
		}
	}

	public bool IsInMotion() {
		if (moving) {
			return true;
		}
		else if (stackBelow != null) {
			return stackBelow.IsInMotion();
		}
		else {
			return false;
		}
	}

	public void LockMovement() {
		moveLock = true;
	}

	public void UnlockMovement() {
		moveLock = false;
	}

	public bool MovementLocked() {
		return (moveLock || GameManager.managerInstance.paused);
	}

	//returns a vector with distance (in stack objects) from current pos to stack top as Y component
	public Vector3 GetStackTop() {
		Vector3 v = Vector3.zero;
		Movement m = this;
		while (m.stackAbove != null) {
			v.y += 1;
			m = m.stackAbove;
		}
		return v;
	}

	//returns a vector with distance (in stack objects) from current pos to stack bottom as Y component
	public Vector3 GetStackBottom() {
		Vector3 v = Vector3.zero;
		Movement m = this;
		while (m.stackBelow != null) {
			v.y -= 1;
			m = m.stackBelow;
		}
		return v;
	}




	public bool Grounded() {
		bool groundboys;

		//	Debug.Log(groundmask.value);	
		//groundmask = ~groundmask;	
		//who the hell knows if you're supposed to invert the mask? not the documentation! gotta try both!
		bool groundboysL = Physics.Raycast(transform.position + new Vector3(-0.425f, 0, 0), Vector3.down, 0.55f, Layers.GetGroundMask(false));
		bool groundboysM = Physics.Raycast(transform.position, Vector3.down, 0.6f, Layers.GetGroundMask(false));
		bool groundboysR = Physics.Raycast(transform.position + new Vector3(0.425f, 0, 0), Vector3.down, 0.55f, Layers.GetGroundMask(false));
		groundboys = ((groundboysL && groundboysM) || (groundboysM && groundboysR));
		//can't use transform.up either for god knows what reason, you gotta use Vector3's versions 
		Debug.DrawRay(transform.position, 0.6f * Vector3.down, (groundboysM ? Color.green : Color.white));
		Debug.DrawRay(transform.position + new Vector3(-0.425f, 0, 0), Vector3.down * 0.55f, (groundboysL ? Color.green : Color.white));
		Debug.DrawRay(transform.position + new Vector3(0.425f, 0, 0), Vector3.down * 0.55f, (groundboysR ? Color.green : Color.white));
		return groundboys;
	}

	public bool GroundedWithoutFrogs() {
		bool groundboys;

		bool groundboysL = Physics.Raycast(transform.position + new Vector3(-0.425f, 0, 0), Vector3.down, 0.55f, Layers.GetGroundMaskNoFrogs());
		bool groundboysM = Physics.Raycast(transform.position, Vector3.down, 0.6f, Layers.GetGroundMaskNoFrogs());
		bool groundboysR = Physics.Raycast(transform.position + new Vector3(0.425f, 0, 0), Vector3.down, 0.55f, Layers.GetGroundMaskNoFrogs());
		groundboys = ((groundboysL && groundboysM) || (groundboysM && groundboysR));
		return groundboys;
	}

	//returns true if the object is in a stable position (not moving, and either grounded or underwater)
	public bool Stable() {
		return (!moving && (Grounded() || isUnderwater) 
			&& (stackBelow == null || stackBelow.GetComponent<ConveyorMovement>() == null));
	}

	protected void SetFacing(int f) {
		int toface = PMath.GetSign(f);
		if (toface == 0) return;

		if (facing != toface && stackAbove != null) {
			stackAbove.FlipFacing();
		}

		facing = toface;
	}

	protected void FlipFacing() {
		facing = -1 * facing;
		if (stackAbove != null) {
			stackAbove.FlipFacing();
		}
	}
}
