using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckMovement : Movement {
	Vector3 destination;
	public bool allowMovement;
	float moveSpeed = 2.15f;
	SpriteRenderer _sprite;

	public Bread bread;
	public bool hasBread;
	float breadTimer;
	const float breadDuration = 1.5f;

	// Use this for initialization
	new void Start () {
		checkBelow = false;
		destination = transform.position;
		if (_sprite == null) {
			_sprite = GetComponentInChildren<SpriteRenderer>();
		}
		_sprite.flipX = (facing < 0);
	}
	
	// Update is called once per frame
	new void Update() {
		base.Update();
		if (facing != 0) {
			_sprite.flipX = (facing < 0);
		}
		if (hasBread) {
			breadTimer -= Time.deltaTime;
			if (breadTimer <= 0) {
				hasBread = false;

			}
		}
		else if (allowMovement) {
			if (moving && destination != transform.position) {
				transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);				
			}
			if(moving && transform.position == destination) {
				moving = false;
				hasBread = true;
				breadTimer = breadDuration;
				GameObject.Destroy(bread.gameObject);
				bread = null;
			}
			if(!moving){
				BreadCheck();
			}
		}
		if (Input.GetMouseButtonDown(0) && !moving) {
			FlipFacing();
		}
	}

	void BreadCheck() {
		if (hasBread) return;
		if (facing < 0) {
			if(!BCheck(Vector3.left))
				BCheck(Vector3.right);
		}
		else {
			if(!BCheck(Vector3.right))
				BCheck(Vector3.left);
		}
		

	}

	bool BCheck(Vector3 dir) {
		LayerMask gmask = Layers.GetSolidsMask(true);
		LayerMask bmask = Layers.GetPuzzleObjectMask();
		float raylen = 20;

		RaycastHit breadhit = new RaycastHit();
		RaycastHit wallhit = new RaycastHit();
		Physics.Raycast(transform.position, dir, out breadhit, raylen, bmask);
		Physics.Raycast(transform.position, dir, out wallhit, raylen, gmask);

	//	Debug.Log(breadhit.distance); Debug.Log(breadhit.point);

		if (breadhit.distance < wallhit.distance && breadhit.distance != 0) {
			destination.x = breadhit.collider.transform.position.x;
			destination.x -= dir.x;
			moving = true;
			SetFacing((int)dir.x);
			SetMoveDir((int)dir.x);
			bread = breadhit.collider.GetComponent<Bread>();
			return true;
		}
		else {
			return false;
		}
	}


	public bool HasBread() {
		return hasBread;
	}
}
