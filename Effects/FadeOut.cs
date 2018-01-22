using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour {
	public bool fadeOnStart;
	public bool destroyWhenComplete;
	public float fadeDuration = 1f;
	SpriteRenderer _sprite;
	bool fading;
	float fadeTime;
	// Use this for initialization
	void Start () {
		_sprite = GetComponent<SpriteRenderer>();
		if (fadeOnStart) {
			BeginFade();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (fading) {
			fadeTime -= Time.deltaTime;

			if (fadeTime <= 0) {
				fading = false;
				if (destroyWhenComplete) {
					GameObject.Destroy(this.gameObject);
				}
			}

			Color c = _sprite.color;
			c.a = Mathf.Lerp(0, 1, fadeTime / fadeDuration);
			_sprite.color = c;
			
		}
	}

	public void BeginFade() {
		fadeTime = fadeDuration;
		fading = true;
	}
}
