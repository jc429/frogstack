using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorMovement : Movement {
	public float moveSpeed;


	public int beltsTouching = 0;
	public int undeletable = 3;
	bool stopAtInt = false;

	// Use this for initialization
	void Start () {
		moving = (moveSpeed != 0);
	}
	
	// Update is called once per frame
	new void Update () {
		//base.Update();
		moving = (moveSpeed != 0);
		Vector3 origin = transform.position + Vector3.up;
		LayerMask gmask = Layers.GetGroundMask(true);

		CheckStack();
		if (stackAbove != null) {
			checkAbove = false;
		}
		Vector3 v = transform.position;
		v.x += moveSpeed * Time.deltaTime;
		transform.position = v;

		if (moveSpeed != 0 && Physics.Raycast(origin, Vector3.right * (Mathf.Sign(moveSpeed)), raylen, gmask)) {
			//	undeletable--;
			stopAtInt = true;
			Debug.DrawRay(transform.position, Vector3.right * (Mathf.Sign(moveSpeed)), Color.red, raylen);
		}
		else
			Debug.DrawRay(transform.position, Vector3.right * (Mathf.Sign(moveSpeed)), Color.yellow, raylen);

		if (stopAtInt) {
			float xint = Mathf.Round(v.x);
			if (Mathf.Abs(xint - v.x) <= 0.05f) {
				v.x = xint;
				transform.position = v;
				SetMoveSpeed(0);
				stopAtInt = false;
			}
		}

		if (undeletable <= 0) {
		/*	if (beltsTouching > 0) {
				undeletable = 3;
			}
			else {*/
				float xint = Mathf.Round(v.x);
				if (Mathf.Abs(xint - v.x) <= 0.05f) {
					v.x = xint;
					transform.position = v;
					if (stackAbove != null) {
						Debug.Log("Peace");
						stackAbove.Unlink();
					}
				//	Unlink(Vector3.up);
					EndLife();
				}
			/*}*/
		}


		if (beltsTouching <= 0 && moveSpeed == 0) {
			undeletable--;
		}
		else if (beltsTouching <= 0) {
			RaycastHit r;
			if (Physics.Raycast(transform.position, Vector3.right * (Mathf.Sign(moveSpeed)), out r, raylen, gmask)) {
				if (r.collider.tag != "Conveyor") {
					stopAtInt = true;
				}
				ConveyorBelt cb = r.collider.GetComponent<ConveyorBelt>();
				if (cb == null) {
					cb = r.collider.GetComponentInParent<ConveyorBelt>();
				}
				if (cb != null /*&& cb.moveSpeed != this.moveSpeed*/) {
					stopAtInt = true;
				}
			}
			else {
				stopAtInt = true;
			}
		}
		else if (beltsTouching == 1) {
			RaycastHit r;
			if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out r, raylen, 1 << 10)) {
				
				ConveyorBelt cb = r.collider.GetComponent<ConveyorBelt>();
				if (cb == null) {
					cb = r.collider.GetComponentInParent<ConveyorBelt>();
				}
				if (cb != null) {
					if(moveSpeed != cb.moveSpeed
					&& (!Physics.Raycast(origin, Vector3.right * (Mathf.Sign(cb.moveSpeed)), raylen, gmask)
						|| cb.moveSpeed == 0)) {
						SetMoveSpeed(cb.moveSpeed);
					}
				}
			}
		}
		
		
	}

	public void SetMoveSpeed(float f) {
		if (f == 0) {
			Vector3 v = transform.position;
			float xint = Mathf.Round(v.x);
			if (Mathf.Abs(xint - v.x) <= 0.1f) {
				v.x = xint;
				transform.position = v;
			}
		}
		
		moveSpeed = f;
		stopAtInt = false;
		//Debug.Log("Speed: "+ f);
	}

	void EndLife() {
		gameObject.SetActive(false);
		GameObject.Destroy(this.gameObject);
	}

	void OnTriggerEnter(Collider other) {
		if (other.GetComponentInParent<ConveyorBelt>() != null) {
			beltsTouching++;
		}
	}
	void OnTriggerExit(Collider other) {
		if (other.GetComponentInParent<ConveyorBelt>() != null) {
			beltsTouching--;
		}
	}
	
	public override void  Unlink(){
 		base.Unlink();
		EndLife();
	}
}
