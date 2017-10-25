using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class BattleField 
{
	public static BattleField battle_field = null;

	public static int FRAME_RATE = 30;
	public static float TIME_PER_FRAME = 1.0f / FRAME_RATE;

	public MapSaveData	map_data;
	BattleGridRenderer _battle_grid_renderer;
	BattleFieldInputHandle battle_field_input_handle;


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

	public void InitUnit()
	{
		HeroUnit hero_unit = UnitManager.Instance().CreateHeroUnit(0);
		hero_unit.SetPosition(new Vector3(10, 0, 10));

		hero_unit = UnitManager.Instance().CreateHeroUnit(1);
		hero_unit.SetPosition(new Vector3(12, 0, 10));

		hero_unit = UnitManager.Instance().CreateHeroUnit(2);
		hero_unit.SetPosition(new Vector3(13, 0, 10));

		hero_unit = UnitManager.Instance().CreateHeroUnit(3);
		hero_unit.SetPosition(new Vector3(13, 0, 11));

		hero_unit = UnitManager.Instance().CreateHeroUnit(4);
		hero_unit.SetPosition(new Vector3(12, 0, 12));

	}

	public void InitInputHandle()
	{
		battle_field_input_handle = new BattleFieldInputHandle();

		battle_field_input_handle.RegisterEvent();
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

	// 定义一个距离的映射
	static int[,] distance_map = new int[3,3]
	{
		{14, 10, 14, },
		{10,  0, 10, },
		{14, 10, 14, },
	};

	public Vector3 GridToWorldPosition(int grid_x, int grid_y)
	{
		float half_step = map_data.map_step * 0.5f;
		Vector3 world_positin = new Vector3(grid_x * map_data.map_step + half_step, 0, grid_y * map_data.map_step + half_step);

		return world_positin;
	}

	public float DistanceByGridXY(int x1, int y1, int x2, int y2)
	{
		Vector3 pos1 = GridToWorldPosition(x1, y1);
		Vector3 pos2 = GridToWorldPosition(x2, y2);

		return (pos1 - pos2).magnitude;
	}

	public List<AStarNode> FindPath(int x_start, int y_start, int x_end, int y_end)
	{
		if(map_data == null)
		{
			Debug.LogError("FindPath 地图阻挡数组没有初始化! ");
			return null;
		}

		if(x_start < 0 || x_start >= map_data.map_width ||
			y_start < 0 || y_start >= map_data.map_height ||
			x_end < 0 || x_end >= map_data.map_width ||
			y_end < 0 || y_end >= map_data.map_height)
		{
			Debug.LogError("FindPath 地图坐标不再范围内 x_start: " + x_start + " y_start: " + y_start + " x_end: " + x_end + " y_end: " + y_end);
			return null;
		}

		List<AStarNode> close_list = new List<AStarNode>();
		List<AStarNode> open_list = new List<AStarNode>();

		AStarNode start_node = new AStarNode();
		start_node._x = x_start;
		start_node._y = y_start;

		open_list.Add(start_node);

		while(open_list.Count > 0)
		{
			open_list.Sort(delegate(AStarNode x, AStarNode y) {

				return x._f - y._f;

			});

			AStarNode min_g_node = open_list[0];

			open_list.RemoveAt(0);

			// 放入关闭列表
			close_list.Add(min_g_node);

			// 找周围的节点
			for(int x_offset = -1; x_offset < 2; ++x_offset)
			{
				for(int y_offset = -1; y_offset < 2; ++y_offset)
				{
					if(x_offset == 0 && y_offset == 0)
					{
						continue;
					}

					// node
					int x = min_g_node._x + x_offset;
					int y = min_g_node._y + y_offset;

					if(x < 0 || x >= map_data.map_width || y < 0 || y >= map_data.map_height)
					{
						continue;
					}

					// 如果找到终点了，就结束了
					if(x == x_end && y == y_end)
					{
						// 生成路径
						List<AStarNode> path_nodes = new List<AStarNode>();

						AStarNode path_temp_node = new AStarNode();
						path_temp_node._x = x_end;
						path_temp_node._y = y_end;
						path_temp_node._parent = min_g_node;

						path_nodes.Add(path_temp_node);

						int pre_offset_x = -100;
						int pre_offset_y = -100;

						while(path_temp_node._parent != null)
						{
							var current_node = path_temp_node._parent;

							int offset_x = current_node._x - path_temp_node._x;
							int offset_y = current_node._y - path_temp_node._y;

							if(offset_x == pre_offset_x && offset_y == pre_offset_y)
							{
								path_nodes.RemoveAt(path_nodes.Count-1);

								path_nodes.Add(current_node);
							}
							else
							{
								path_nodes.Add(current_node);	

								pre_offset_x = offset_x;
								pre_offset_y = offset_y;
							}

							path_temp_node = current_node;
						}


						path_nodes.Reverse();


						return path_nodes;
					}

					// 如果是阻挡，不处理
					if(IsBlock(x, y))
					{
						continue;
					}

					// 在开启列表里，检查g的值
					AStarNode node_in_open = null;
					for(int i = 0; i < open_list.Count; ++i)
					{
						AStarNode temp_node = open_list[i];

						if(temp_node._x == x && temp_node._y == y)
						{
							node_in_open = temp_node;
							break;
						}
					}

					// 在关闭列表里，不处理
					AStarNode node_in_close = null;
					for(int i = 0; i < close_list.Count; ++i)
					{
						AStarNode temp_node = close_list[i];

						if(temp_node._x == x && temp_node._y == y)
						{
							node_in_close = temp_node;
							break;
						}
					}

					// 没有处理过的，计算g，h，f，然后加入开启列表

					if(node_in_open == null && node_in_close == null)
					{
						AStarNode new_open_node = new AStarNode();
						new_open_node._x = x;
						new_open_node._y = y;

						new_open_node._g = distance_map[x_offset+1, y_offset+1] + min_g_node._g;
						new_open_node._h = Mathf.Abs(x_end - x) + Mathf.Abs(y_end - y);

						new_open_node._f = new_open_node._g + new_open_node._h;

						new_open_node._parent = min_g_node;

						open_list.Add(new_open_node);


					}
					else if(node_in_open != null)
					{
						int new_g = distance_map[x_offset+1, y_offset+1] + min_g_node._g;

						if(new_g < node_in_open._g)
						{
							node_in_open._g = new_g;
							node_in_open._parent = min_g_node;
						}
					}
					else if(node_in_close != null)
					{
						// 什么都不做
					}

				}
			}
		}


		return null;
	}
}
