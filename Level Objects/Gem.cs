using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour {
	SpriteRenderer _sprite;
	AudioSource _audio;

	int objs;
 
	public FrogMovement connectedFrog;

	[SerializeField]
	Sprite[] sprites;

	// Use this for initialization
	void Start () {
		_sprite = GetComponentInChildren<SpriteRenderer>();
		_audio = GetComponent<AudioSource>();
		_sprite.sprite = sprites[0];
		connectedFrog = null;
		GameManager.managerInstance.RegisterGem(this);
	}
	
	// Update is called once per frame
	void Update () {
		if(IsCollected()){
			_sprite.sprite = sprites[1];
		}
		else{
			_sprite.sprite = sprites[0];
		}
	}

	void OnTriggerEnter(Collider other) {
		
		if(other.gameObject.GetComponentInParent<FrogMovement>() != null && connectedFrog == null){
			objs++;
			connectedFrog = other.gameObject.GetComponentInParent<FrogMovement>();
			if(_audio != null){
				_audio.Play();
			}
			
		}
	}
	void OnTriggerExit(Collider other) {
		if(other.gameObject.GetComponentInParent<FrogMovement>() == connectedFrog && connectedFrog != null){
			objs--;
			connectedFrog = null;
		}
	}

	public bool IsCollected() {
		return objs > 0;
	}
}
