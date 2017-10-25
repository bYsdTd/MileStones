using UnityEngine;
using System.Collections;

[System.Serializable]
public class MapSaveData
{
	public int map_width;
	public int map_height;
	public float map_step;

	// 二维数组，表示哪个位置有阻挡
	public bool[,] grid_array;

}