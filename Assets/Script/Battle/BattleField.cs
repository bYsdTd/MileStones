using UnityEngine;
using System.Collections;
using System.IO;

public class BattleField 
{
	MapSaveData	map_data;
	BattleGridRenderer _battle_grid_renderer;

	public void SetBattleGridRenderer(BattleGridRenderer battle_grid_renderer)
	{
		_battle_grid_renderer = battle_grid_renderer;

		_battle_grid_renderer.battle_field = this;

		_battle_grid_renderer.Init(map_data.map_width, map_data.map_height, map_data.map_step);

	}

	public void LoadMap(string path)
	{
		// 地址先写死
		if(!File.Exists(path))
		{
			Debug.LogError("地图数据不存在:" + path);
			return;
		}

		using (FileStream stream = File.Open(path, FileMode.Open))
		{
			map_data = new MapSaveData();
			var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			map_data = (MapSaveData)binaryFormatter.Deserialize(stream);
		}
	}

	public bool IsBlock(int x, int y)
	{
		if(map_data == null)
		{
			Debug.LogError("地图阻挡数组没有初始化! ");
			return false;
		}

		if(x < 0 || x >= map_data.map_width ||
			y < 0 || y >= map_data.map_height)
		{
			Debug.LogError("IsBlock 地图坐标不再范围内 x: " + x + " y: " + y);
			return false;
		}

		return map_data.grid_array[x, y];
	}
}
