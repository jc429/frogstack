using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CursorNode {
	public int id;
	public Vector2 pos;
	public int westNeighbor;
	public int eastNeighbor;
	public int northNeighbor;
	public int southNeighbor;
}
