using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogSounds : MonoBehaviour {
	public AudioClip[] clips;
	AudioSource _src;
	// Use this for initialization
	void Start () {
		_src = GetComponent<AudioSource>(); 
		
		PlayCroak();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PlayJumpSound() {
		_src.clip = clips[0];
		_src.Play();
	}

	public void PlayCroak() {
		_src.clip = clips[1];
		_src.Play();

	}
}
