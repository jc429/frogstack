using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;


public static class LevelManager {
	public const int numWorlds = 3; 
	public const int levelsPerWorld = 12;
	const int levelOffset = 2;	//how many scenes are before level 1 in the game

	public static LevelData levelData;

	public static int curWorld;	//0 - x 
	public static int curLevel;	//0 - y

	public static void LoadLevel(int world, int level) {
		curWorld = world-1;
		curLevel = level-1;
		//do more here 
		int index = (curWorld * levelsPerWorld) + curLevel + levelOffset;
		Debug.Log("Loading Scene " + index);
		SceneManager.LoadScene(index);
	}

	public static void LoadCurrentLevel() {
		int index = (curWorld * levelsPerWorld) + curLevel + levelOffset;
		Debug.Log("Loading Scene " + index);
		SceneManager.LoadScene(index);
	}

	public static void LoadNextLevel() {
		if (curLevel + 1 < levelsPerWorld)
			curLevel += 1;
		else if (curWorld + 1 < numWorlds)
			curWorld += 1;
		else
			Debug.Log("No next level!");
		LoadCurrentLevel();
	}

	public static int GetLevelRecordAttempts(int world, int level) {
		world = Mathf.Clamp(world - 1, 0, numWorlds);
		level = Mathf.Clamp(level - 1, 0, levelsPerWorld);
		//	Debug.Log("showing record for " + world +"-"+ level);
		return levelData.recordActions[world, level];
	}
	public static int GetLevelRecordFrogs(int world, int level) {
		world = Mathf.Clamp(world - 1, 0, numWorlds);
		level = Mathf.Clamp(level - 1, 0, levelsPerWorld);
		//	Debug.Log("showing record for " + world +"-"+ level);
		return levelData.recordFrogs[world, level];
	}

	public static void CompleteCurrentLevel(/*int numactions*/) {
	//	Debug.Log("saved " + numactions + "to " + curWorld + "-" + curLevel);
	/*	if (levelData.recordActions[curWorld, curLevel] > 0)
			numactions = Mathf.Min(numactions, levelData.recordActions[curWorld, curLevel]);
		levelData.recordActions[curWorld, curLevel] = numactions;*/
		Debug.Log("Completed" + curWorld + "-" + curLevel);
		levelData.completedLevels[curWorld, curLevel] = true;
		SaveLevelData();
	}

	public static void UnlockNextLevel() {
	//	int world = Mathf.Clamp(curWorld - 1, 0, numWorlds);
	//	int level = Mathf.Clamp(curLevel - 1, 0, levelsPerWorld);
		int world = curWorld;
		int level = curLevel;
		if (level + 1 < levelsPerWorld)
			level += 1;
		else if (world + 1 < numWorlds)
			world += 1;
		Debug.Log("Unlocked " + world + "-" + level);
		levelData.unlockedLevels[world, level] = true;
		SaveLevelData();
	}

	public static void UnlockLevel(int world, int level) {
		world = Mathf.Clamp(world - 1, 0, numWorlds);
		level = Mathf.Clamp(level - 1, 0, levelsPerWorld);
		levelData.unlockedLevels[world, level] = true;
		SaveLevelData();
	}

	public static bool LevelUnlocked(int world, int level) {
		world = Mathf.Clamp(world - 1, 0, numWorlds);
		level = Mathf.Clamp(level - 1, 0, levelsPerWorld);
	//	Debug.Log(world + "-" + level);
		return levelData.unlockedLevels[world, level];
	}

	public static bool LevelCompleted(int world, int level) {
		world = Mathf.Clamp(world - 1, 0, numWorlds);
		level = Mathf.Clamp(level - 1, 0, levelsPerWorld);
		Debug.Log(world + "-" + level + ":" + levelData.completedLevels[world, level]);

		return levelData.completedLevels[world, level];
	}


	public static void LoadLevelData() {
		if (File.Exists(Application.persistentDataPath + "/levelInfo.dat")) {

			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/levelInfo.dat", FileMode.Open);
			LevelData ld = bf.Deserialize(file) as LevelData;
			if (ld != null) {
				levelData = ld;
			}
			file.Close();
		}
		else{
			Debug.Log("Level Data not found");
			levelData = new LevelData();
			if(!LevelUnlocked(1,1))
				UnlockLevel(1, 1);
		}
	}

	public static void SaveLevelData() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/levelInfo.dat");
		LevelData ld = levelData;

		bf.Serialize(file, ld);
		Debug.Log("Level Saved");
		file.Close();
	}

	public static void EraseAllLevelData() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/levelInfo.dat");
		LevelData ld = new LevelData();
		levelData = ld;
		levelData.unlockedLevels[0, 0] = true;
		bf.Serialize(file, levelData);
		Debug.Log("Level Data Erased :(");
		file.Close();
	}

	public static void SetRecord(int world, int level, int numFrogs, int numActions) {
		Debug.Log("Setting records for" + world + "-" + level + ": " + numFrogs + ", " + numActions);
		world = Mathf.Clamp(world /*- 1*/, 0, numWorlds);
		level = Mathf.Clamp(level /*- 1*/, 0, levelsPerWorld);
		if ((levelData.recordActions[world, level] < 0)
		|| (levelData.recordActions[world, level] > numActions)) {
			levelData.recordActions[world, level] = numActions;
			levelData.recordFrogs[world, level] = numFrogs;
		}
		else if ((levelData.recordActions[world, level] == numActions)
		&& (levelData.recordFrogs[world, level] > numFrogs)) {
			levelData.recordFrogs[world, level] = numFrogs;
		}
	}

	public static int GetTotalFrogs() {
		int total = 0;
		for (int i = 0; i < LevelManager.numWorlds; i++) {
			for (int j = 0; j < LevelManager.levelsPerWorld; j++) {
				if (levelData.recordActions[i, j] > 0) {
					total += levelData.recordFrogs[i, j];
				}
			}
		}
		return total;
	}
	public static int GetTotalActions() {
		int total = 0;
		for (int i = 0; i < LevelManager.numWorlds; i++) {
			for (int j = 0; j < LevelManager.levelsPerWorld; j++) {
				if (levelData.recordActions[i, j] > 0) {
					total += levelData.recordActions[i, j];
				}
			}
		}
		return total;
	}
}

[Serializable]
public class LevelData {
	public bool[,] unlockedLevels = new bool[LevelManager.numWorlds, LevelManager.levelsPerWorld];
	public bool[,] completedLevels = new bool[LevelManager.numWorlds, LevelManager.levelsPerWorld];
	public int[,] recordActions = new int[LevelManager.numWorlds, LevelManager.levelsPerWorld];
	public int[,] recordFrogs = new int[LevelManager.numWorlds, LevelManager.levelsPerWorld];

	public LevelData() {
		for (int i = 0; i < LevelManager.numWorlds; i++) {
			for (int j = 0; j < LevelManager.levelsPerWorld; j++) {
				recordActions[i, j] = -1;
				recordFrogs[i, j] = -1;
			}
		}
	}
}