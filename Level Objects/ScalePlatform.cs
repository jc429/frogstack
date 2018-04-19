using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalePlatform : MonoBehaviour {

	Movement _platMovement;
	[SerializeField]
	Transform scaleString;

	[SerializeField]
	ScalePlatform connectedPlatform;

	int defaultHeight;
	[SerializeField] [Range(0,10)]
	int maxMovement;

	Vector3 midPos;
	Vector3 maxPos;
	Vector3 minPos;

	Vector3 endPos;

	float moveSpeed = 1f;

	int stackAmt;				//amount of things stacked above us
	// Use this for initialization
	void Start () {
		_platMovement = GetComponent<Movement>();
		defaultHeight = Mathf.RoundToInt(transform.position.y);
		midPos = transform.position;
		minPos = new Vector3(transform.position.x,defaultHeight - maxMovement);
		maxPos = new Vector3(transform.position.x,defaultHeight + maxMovement);
		endPos = transform.position;

		if(connectedPlatform == null){
			Debug.Log("WARNING: scale platform not connected to anything!");
		}
	}
	
	// Update is called once per frame
	void Update() {
		Vector3 s = scaleString.localScale;
		s.y = (maxPos.y - transform.position.y);
		scaleString.localScale = s; 
		stackAmt = _platMovement.CountToStackTop();
		if((_platMovement.stackAbove || connectedPlatform._platMovement.stackAbove) && !_platMovement.moving){
			if(connectedPlatform != null){
				if(stackAmt > connectedPlatform.stackAmt){
					endPos = minPos;
					connectedPlatform.endPos = connectedPlatform.maxPos;
				}
			/*	else if(stackAmt < connectedPlatform.stackAmt){
					endPos = maxPos;
				}*/
				else if(stackAmt == connectedPlatform.stackAmt){
					endPos = midPos;
					connectedPlatform.endPos = connectedPlatform.midPos;
				}
			}
			else{
				endPos = minPos;
			}

			if(stackAmt > connectedPlatform.stackAmt){ //only do all this once 
				RaycastHit r, r2 = new RaycastHit();
				LayerMask gmask = Layers.GetSolidsMask(false);
				float raylen = transform.position.y - minPos.y;
				bool hit = Physics.Raycast(transform.position, Vector3.down, out r, raylen, gmask);
				bool hit2 = false;
				if(connectedPlatform != null){
					Vector3 otherStackTop = connectedPlatform.transform.position;
					otherStackTop += connectedPlatform._platMovement.DistanceToStackTop();
					hit2 = Physics.Raycast(otherStackTop, Vector3.up, out r2, raylen, gmask);
				}
				if(hit || hit2){
					float dist;
					if(hit && hit2){
						dist = Mathf.Min(r.distance,r2.distance);
					}
					else if(hit2){
						dist = r2.distance;
					}
					else{
						dist = r.distance;
					}
					if(dist < 1f){
						endPos = transform.position;
						connectedPlatform.endPos = connectedPlatform.transform.position;
					}
					else{
						Debug.Log("hit detected at " + dist);
						int movedist = Mathf.RoundToInt(dist - 0.5f);
						endPos.y = transform.position.y - movedist;
						connectedPlatform.endPos.y = connectedPlatform.transform.position.y + movedist;
						Debug.Log("help" + endPos + "," + connectedPlatform.endPos);
					}
				}
			}
			if(endPos.y != transform.position.y){
				_platMovement.moving = true;
				connectedPlatform._platMovement.moving = true;
			}
			
		}
		if(_platMovement.moving){
			if(!PMath.CloseTo(transform.position,endPos)){
				transform.position = Vector3.MoveTowards(transform.position, endPos,moveSpeed*Time.deltaTime);
				if(connectedPlatform != null){
				//	connectedPlatform.transform.position = Vector3.MoveTowards(connectedPlatform.transform.position, connectedPlatform.endPos,moveSpeed*Time.deltaTime);
				}
			}
			else{
				_platMovement.moving = false;
				if(connectedPlatform != null){
					connectedPlatform._platMovement.moving = false;
			//		connectedPlatform.transform.position = connectedPlatform.endPos;
				}
				transform.position = endPos;
			}
		}
	}

}
