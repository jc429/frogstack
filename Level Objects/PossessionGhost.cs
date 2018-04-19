using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessionGhost : TriggerObj {

	[SerializeField] [Range(-1,1)]
	int moveDir;
	float moveSpeed = 1f;

	bool moving; 
	FrogMovement targetFrog;
	int facing;

	SpriteRenderer _sprite;
	// Use this for initialization
	void Start () {
		_sprite = GetComponentInChildren<SpriteRenderer>();
		moving = true;
		facing = moveDir;
		_sprite.flipX = (facing < 0);
	}
	
	// Update is called once per frame
	protected override void Update2 () {
		if(targetFrog != null && targetFrog.IsStable() && !targetFrog.IsInMotion()){
			GameManager.managerInstance.SetCurrentFrog(targetFrog);
			Destroy(this.gameObject);
		}
		if(moving){
			Vector3 v = transform.position;
			v.x += moveDir * moveSpeed * Time.deltaTime;
			transform.position = v;

			Vector3 offset = Vector3.zero;
			offset.y = 0.3f*Mathf.Sin(2*Time.time);
			_sprite.transform.localPosition = offset;
			
		}
		else{
			if(targetFrog != null){
				transform.position = Vector3.MoveTowards(transform.position,targetFrog.transform.position,Mathf.Abs(moveSpeed)*Time.deltaTime);
			}
		}
	}

	protected override void TriggerEnter(){
	//	Debug.Log("Boo!");
		moving = false;
		targetFrog = connectedFrog;
	}
	
}
