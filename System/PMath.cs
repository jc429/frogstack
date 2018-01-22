﻿using UnityEngine;
using System.Collections;

public static class PMath {

	public static int Expand(float f) {
		if (f > 0)
			return Mathf.CeilToInt(f);
		else if (f < 0)
			return Mathf.FloorToInt(f);
		else
			return 0;
	}

	//returns the sign of an integer, or 0 if the int is 0 (the important part)
	public static int GetSign(float f) {
		if (f == 0)
			return 0;
		return (int)Mathf.Sign(f);
	}

	public static Vector3 RoundToInts(Vector3 v) {
		v.x = Mathf.RoundToInt(v.x);
		v.y = Mathf.RoundToInt(v.y);
		v.z = Mathf.RoundToInt(v.z);
		return v;
	}
}
