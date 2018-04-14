using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VirtualController {


	public static float GetAxisHorizontal(){
		return Input.GetAxisRaw("Horizontal");
	}

	public static float GetAxisVertical(){
		return Input.GetAxisRaw("Vertical");
	}

	public static bool UpDPadPressed(bool held = false){
		if(held){
			return (Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.UpArrow));
		}else{
			return (Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.UpArrow));
		}
	}

	public static bool DownDPadPressed(bool held = false){
		if(held){
			return (Input.GetKey(KeyCode.S)||Input.GetKey(KeyCode.DownArrow));
		}else{
			return (Input.GetKeyDown(KeyCode.S)||Input.GetKeyDown(KeyCode.DownArrow));
		}
	}

	public static bool LeftDPadPressed(bool held = false){
		if(held){
			return (Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.LeftArrow));
		}
		else{
			return (Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.LeftArrow));
		}
	}

	public static bool RightDPadPressed(bool held = false){
		if(held){
			return (Input.GetKey(KeyCode.D)||Input.GetKey(KeyCode.RightArrow));
		}else{
			return (Input.GetKeyDown(KeyCode.D)||Input.GetKeyDown(KeyCode.RightArrow));
		}
	}

	/**********************************************************************************/

	public static bool SpawnButtonPressed(bool held = false){
		if(held){
			return (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift));
		}
		else{
			return (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftShift));
		}
	}

	public static bool CroakButtonPressed(bool held = false){
		if(held){
			return Input.GetKey(KeyCode.C);
		}
		else{
			return Input.GetKeyDown(KeyCode.C);
		}
	}

	public static bool PauseButtonPressed(bool held = false){
		if(held){
			return (Input.GetKey(KeyCode.P) || Input.GetKey(KeyCode.Escape));
		}
		else{
			return (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape));
		}
	}

	public static bool ResetButtonPressed(bool held = false){
		if(held){
			return Input.GetKey(KeyCode.R);
		}
		else{
			return Input.GetKeyDown(KeyCode.R);
		}
	}
}
