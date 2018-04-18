using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {
	SpriteRenderer _sprite;
	AudioSource _audio;
	NoSpawn _nospawn;
	bool levelComplete = false;
	public float postLevelTimer = 1.5f;
	// Use this for initialization
	void Start () {
		_audio = GetComponent<AudioSource>();
		_nospawn = GetComponent<NoSpawn>();
		_sprite = GetComponent<SpriteRenderer>();
		postLevelTimer = 0.5f;
		levelComplete = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (levelComplete && postLevelTimer > 0) {
			postLevelTimer -= Time.deltaTime;
			if (postLevelTimer <= 0) {
				if(GameManager.DEBUG_MODE && GameManager._restartOnLevelCompletion){
					GameManager.managerInstance.ResetLevel();
				}
				else{
					GameManager.managerInstance.AdvanceLevel();
				}
			}
		}
		if (_nospawn.IsActivated()) {
			_sprite.color = Color.red;
			if (!levelComplete) {
				levelComplete = true;
				GameManager.managerInstance.LockPlayerMovement();
				GameManager.managerInstance.HaltBGM();
				_audio.Play();

			}
		}
	}
}
