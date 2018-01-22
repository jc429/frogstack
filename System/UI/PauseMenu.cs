using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
	public GameObject pMenu;
	AudioSource _audio;
	bool open;
	// Use this for initialization
	void Start () {
		_audio = GetComponent<AudioSource>();
		GameManager.managerInstance.pauseMenu = this;
		pMenu.SetActive(false);
		open = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Toggle() {
		_audio.Play();
		open = !open;
		pMenu.SetActive(open);
		GameManager.managerInstance.paused = open;
	}

	public void OpenPauseMenu() {
		GameManager.managerInstance.paused = true;
		_audio.Play();
		pMenu.SetActive(true);
		open = true;
	}

	public void ClosePauseMenu() {
		GameManager.managerInstance.paused = false;
		_audio.Play();
		pMenu.SetActive(false);
		open = false;
	}

	public void QuitLevel() {
		ClosePauseMenu();
		GameManager.managerInstance.GoToMainMenu();
	}

	public void RestartLevel() {
		ClosePauseMenu();
		GameManager.managerInstance.ResetLevel();
	}
}
