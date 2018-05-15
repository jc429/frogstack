using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : TriggerObj {
	SpriteRenderer _sprite;
	AudioSource _audio;

 

	[SerializeField]
	Sprite[] sprites;

	// Use this for initialization
	protected override void Start2 () {
		_sprite = GetComponentInChildren<SpriteRenderer>();
		_audio = GetComponent<AudioSource>();
		_sprite.sprite = sprites[0];
		GameManager.managerInstance.RegisterGem(this);
	}
	
	// Update is called once per frame
	void Update () {
		UpdateActivity();
		if(IsCollected()){
			_sprite.sprite = sprites[1];
		}
		else{
			_sprite.sprite = sprites[0];
		}
	}

	protected override void TriggerEnter(){
		if(_audio != null){
			_audio.Play();
		}
	}

	public bool IsCollected() {
		return objs > 0;
	}
}
