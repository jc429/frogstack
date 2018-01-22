using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class Cutscene : MonoBehaviour {
	//	public static ScreenTransition instance;
	enum TransitionMode {
		TM_Out,
		TM_In,
	}

	TransitionMode mode;
	public const float transitionDuration = 1;
	[Range(0, transitionDuration)]
	public float transitionTime;
	public bool animating;
	public bool transitionDone;
	float degree = 0.4f;	//how much screen space the black bars should take up
	public Material transitionMat;

	/*	void Awake() {
			if (instance == null) {
				instance = this;
				DontDestroyOnLoad(this.gameObject);
			}
			else if (instance != this) {
				Destroy(this.gameObject);
			}
		}*/

	// Use this for initialization
	void Start() {
		/*if (GameManager.managerInstance != null) {
			GameManager.managerInstance.screenTransition = this;
		}*/
		//transitionTime = 0;
		if (transitionMat != null) {
			transitionMat.SetFloat("_Cutoff", 0);
			transitionMat.SetFloat("_Fade", 1);
		}
	//	StartTransitionIn();
	}

	// Update is called once per frame
	void Update() {
		if (animating) {
			transitionTime += Time.deltaTime;
			if (transitionTime <= transitionDuration && transitionDuration > 0 && transitionMat != null) {
				if (mode == TransitionMode.TM_In)
					transitionMat.SetFloat("_Cutoff", degree * (transitionTime / transitionDuration));
				else if (mode == TransitionMode.TM_Out)
					transitionMat.SetFloat("_Cutoff", degree * (1 - (transitionTime / transitionDuration)));

			}
			if (transitionTime >= transitionDuration) {
				transitionTime = transitionDuration;
				FinishTransition();
			}
		}
	/*	else {
			if (Input.GetMouseButtonDown(0))
				StartTransitionIn();

			if (Input.GetMouseButtonDown(1))
				StartTransitionOut();
		}*/
	}

	void OnRenderImage(RenderTexture src, RenderTexture dst) {
		if (transitionMat != null)
			Graphics.Blit(src, dst, transitionMat);
	}

	public void StartTransitionOut() {
		animating = true;
		transitionTime = 0;
		transitionDone = false;
		mode = TransitionMode.TM_Out;
	}

	public void FinishTransition() {
		if (mode == TransitionMode.TM_In)
			transitionMat.SetFloat("_Cutoff", degree);
		else if (mode == TransitionMode.TM_Out)
			transitionMat.SetFloat("_Cutoff", 0);
		animating = false;
		transitionDone = true;
	}

	public void StartTransitionIn() {
		animating = true;
		transitionTime = 0;
		transitionDone = false;
		mode = TransitionMode.TM_In;
	}

	public bool FadedOut() {
		return (transitionDone && mode == TransitionMode.TM_Out);
	}
}
