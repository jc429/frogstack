using UnityEngine;
using System.Collections;

public static class PColor {

	public static Color CreateColor(int r, int g, int b, int a = 255) {
		float red = (float)r / 255;
		float green = (float)g / 255;
		float blue = (float)b / 255;
		float alpha = (float)a / 255;

		return new Color(red, green, blue, alpha);
	}

}
