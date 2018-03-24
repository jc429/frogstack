using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	GameManager _gameManager;
	public AudioClip[] tracks;
	AudioSource _audioSource;

	// Use this for initialization
	void Awake () {
		_gameManager = GetComponent<GameManager>();
		_audioSource = GetComponent<AudioSource>();
		_audioSource.playOnAwake = false;
		_audioSource.loop = true;
		_audioSource.volume = 0.01f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetTrack(AudioClip clip){
		_audioSource.clip = clip;
	}

	// plays soundtrack via id num
	public void PlayTrack(int num) {
		if (num < 0 || num >= tracks.Length) {
			return;
		}
		_audioSource.clip = tracks[num];
		_audioSource.Play();
	}

	// plays soundtrack via audio clip
	public void PlayTrack(AudioClip clip) {
		_audioSource.clip = clip;
		_audioSource.Play();
	}


	public void Stop() {
		_audioSource.Stop();
	}
}
