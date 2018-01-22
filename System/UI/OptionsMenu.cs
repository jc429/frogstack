using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour {
	public GameObject oMenu;
	AudioSource _audio;
	public bool open;
	public GameObject clearMenu;
	// Use this for initialization
	void Start() {
		_audio = GetComponent<AudioSource>();
		GameManager.managerInstance.optionsMenu = this;
		oMenu.SetActive(false);
		open = false;
		if (clearMenu != null) {
			clearMenu.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetSpeed(float spd) {
		GameManager.SetHopSpeed(0.5f*spd);
	}

	public void SetVolume(float vol) {
		AudioListener.volume = vol;
	}

	public void SetMute(bool muted) {
		_audio.Play();
		AudioListener.pause = muted;
	}

	public void Toggle() {
		_audio.Play();
		open = !open;
		oMenu.SetActive(open);
	//	GameManager.managerInstance.paused = open;
	}

	public void OpenOptionsMenu() {
	//	GameManager.managerInstance.paused = true;
		_audio.Play();
		oMenu.SetActive(true);
		open = true;
	}

	public void CloseOptionsMenu() {
	//	GameManager.managerInstance.paused = false;
		_audio.Play();
		oMenu.SetActive(false);
		open = false;
	}

	public void OpenClearMenu() {
		if (clearMenu != null) {
			_audio.Play();
			clearMenu.SetActive(true);
		}
	}
	public void CloseClearMenu() {
		if (clearMenu != null) {
			_audio.Play();
			clearMenu.SetActive(false);
		}
	}

	public void ClearGameData() {
		GameManager.managerInstance.ClearAllData();
	}
}
