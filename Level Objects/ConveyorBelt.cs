using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : ButtonConnectedObj {
	public float moveSpeed = 2;
	[SerializeField]
	ConveyorMovement movPrefab;
//	Animation _animation;
	Animator _animator;
	
	[SerializeField] bool halted;
	float storedSpeed;

	[SerializeField]
	bool reverseOnAction;
	[SerializeField]
	bool toggleOnAction;

	List<Movement> attachedObjs;
	List<ConveyorMovement> movements;
	// Use this for initialization
	void Start () {
	//	_animation = GetComponent<Animation>();
		_animator = GetComponent<Animator>();
		_animator.SetFloat("animSpeed", moveSpeed * -1.4f);
		attachedObjs = new List<Movement>();
		movements = new List<ConveyorMovement>();
		storedSpeed = moveSpeed;
		if (halted) {
			Halt();
		}
	}
	
	// Update is called once per frame
	void Update() {

		if (DEBUG) {
			//if movespeed changes,	_animator.SetFloat("animSpeed", moveSpeed * -1.4f);
			if (Input.GetMouseButtonDown(0)) {
				//	CreateConveyorMovement(this.transform.position+Vector3.up);
				Reverse();
			}
			if (Input.GetMouseButtonDown(1)) {
				//	CreateConveyorMovement(this.transform.position+Vector3.up);
				Toggle();
			}
		}
		foreach (Movement m in attachedObjs) {
			CreateConveyorMovement(m);
		}
	}


	public void SetMoveSpeed(float spd) {
		moveSpeed = spd;
		_animator.SetFloat("animSpeed", moveSpeed * -1.4f);
		foreach (ConveyorMovement cm in movements) {
			if (cm != null) {
				cm.SetMoveSpeed(moveSpeed);
			}
		}
	}

	public override void ReceiveButtonPress() {
		if (reverseOnAction) {
			Reverse();
		}
		if (toggleOnAction) {
			Toggle();
		}
	}

	public override void ReceiveButtonRelease() {
		if (reverseOnAction) {
			Reverse();
		}
		if (toggleOnAction) {
			Toggle();
		}
	}

	public void Reverse() {
		SetMoveSpeed(-moveSpeed);
	}

	public void Halt() {
		storedSpeed = moveSpeed;
		halted = true;
		SetMoveSpeed(0);
	}

	public void Resume() {
		halted = false;
		SetMoveSpeed(storedSpeed);
	}

	public void Toggle() {
		if (halted)
			Resume();
		else
			Halt();
	}

	

	public void CreateConveyorMovement(Movement m) {
		if (m.stackBelow != null) return;
		if (m.moving) return;
		LayerMask gmask = Layers.GetGroundMask(true);
	//	if (Physics.Raycast(m.transform.position, Vector3.right * (Mathf.Sign(moveSpeed)), 0.6f, gmask)) return;

		ConveyorMovement cm = Instantiate(movPrefab) as ConveyorMovement;
		cm.SetMoveSpeed(moveSpeed);
		if (Physics.Raycast(m.transform.position, Vector3.right * (Mathf.Sign(moveSpeed)), 0.6f, gmask)) {
			cm.SetMoveSpeed(0);
		}
		cm.transform.position = m.transform.position + Vector3.down;
		Debug.Log("STACK!");
		//cm.CheckStack();
		cm.CheckStack();
		m.CheckStack();
		m.transform.parent = cm.transform;
		cm.GetComponent<Rigidbody>().isKinematic = true;
		movements.Add(cm);
	}
	
	void OnTriggerEnter(Collider other) {
		//Debug.Log("steppie");
		Movement m = other.GetComponent<Movement>();
		if (m == null) {
			m = other.GetComponentInParent<Movement>();
		}
		if (m == null) {
			Debug.Log("Frog not found");
			return;
		}
		if (!m.stackBelow /*&& moveSpeed != 0*/) {
			LayerMask gmask = Layers.GetGroundMask(true);
			if(!Physics.Raycast(m.transform.position,Vector3.right*(Mathf.Sign(moveSpeed)),0.6f,gmask)){
				CreateConveyorMovement(m);
			}
			attachedObjs.Add(m);

		}
	}
	void OnTriggerExit(Collider other) {
		Movement m = other.GetComponent<Movement>();
		if (m == null) {
			m = other.GetComponentInParent<Movement>();
		}
		if (m != null) {
			attachedObjs.Remove(m);
		}
	}
}
