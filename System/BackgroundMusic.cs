using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour {
	AudioSource _audio;
	public AudioClip[] clips;
	// Use this for initialization
	void Start () {
		_audio = GetComponent<AudioSource>();
		GameManager.managerInstance.SetBGMSrc(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PlayBGM() {
		_audio.clip = clips[0];
		_audio.Play();
	}
	public void PlayVictory() {
		_audio.clip = clips[1];
		_audio.Play();
	}
	public void PlayDeath() {
		_audio.clip = clips[2];
		_audio.Play();
	}
	public void HaltBGM() {
		_audio.Stop();
	}
}
