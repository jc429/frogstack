using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Layers {

	public static LayerMask GetGroundMask(bool ignorePlatforms) {
		LayerMask groundmask = 0;
		//have fun hard coding layers here 
		groundmask |= 1 << 0;	//default
		groundmask |= 1 << 8;	//frogs
		groundmask |= 1 << 10;	//invisible
		if (!ignorePlatforms) {
			groundmask |= 1 << 12;	//platforms
		}
		return groundmask;
	}

	public static LayerMask GetGroundMaskNoFrogs() {
		LayerMask groundmask = 0;
		groundmask |= 1 << 10;
		groundmask |= 1 << 12;
		return groundmask;
	}

	public static LayerMask GetWaterMask() {
		return 1 << 4;
	}

	public static LayerMask GetFrogMask() {
		return 1 << 8;
	}

	public static LayerMask GetPuzzleObjectMask() {
		return 1 << 15;
	}
	public static LayerMask GetObjectMask() {		//frogs and also puzzleobjects 
		LayerMask omask = 0;
		omask |= 1 << 8;
		omask |= 1 << 15;
		return omask;
	}
}
