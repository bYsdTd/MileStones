﻿using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class BattleField 
{
	public static BattleField battle_field = null;

	public static int FRAME_RATE = 30;
	public static float TIME_PER_FRAME = 1.0f / FRAME_RATE;

	public MapSaveData	map_data;
	public RealTimeBattleLogic	real_time_battle_logic = null;


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

		using (FileStream stream = File.OpenRead(path))
		{
			map_data = new MapSaveData();
			var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			map_data = (MapSaveData)binaryFormatter.Deserialize(stream);
		}
	}

	public Vector3 GetRandomPosition(Vector3 born_point)
	{
		int offset_x = Random.Range(3, 8);
		int offset_y = Random.Range(-3, 3);

		return new Vector3(born_point.x + offset_x, born_point.y, born_point.z + offset_y );
	}

	public void InitUnit()
	{
		// team 1
		Vector3 born_point1 = new Vector3(5, 0, 30);

		UnitManager.Instance().CreateHeroUnit("soldier", 0, GetRandomPosition(born_point1), 1);
		UnitManager.Instance().CreateHeroUnit("soldier", 1, GetRandomPosition(born_point1), 1);

		UnitManager.Instance().CreateHeroUnit("tank", 2, GetRandomPosition(born_point1), 1);
		UnitManager.Instance().CreateHeroUnit("tank", 3, GetRandomPosition(born_point1), 1);
		UnitManager.Instance().CreateHeroUnit("tank", 4, GetRandomPosition(born_point1), 1);



		UnitManager.Instance().CreateHeroUnit("b2", 5, GetRandomPosition(born_point1), 1);
		UnitManager.Instance().CreateHeroUnit("b2", 6, GetRandomPosition(born_point1), 1);

		UnitManager.Instance().CreateHeroUnit("f15", 7, GetRandomPosition(born_point1), 1);
		UnitManager.Instance().CreateHeroUnit("f15", 8, GetRandomPosition(born_point1), 1);


		UnitManager.Instance().CreateHeroUnit("rocket_car", 9, GetRandomPosition(born_point1), 1);

		UnitManager.Instance().CreateBuildingUnit("base", 51, born_point1, 1);


		// team 2

		Vector3 born_point2 = new Vector3(50, 0, 10);

		HeroUnit base_unit = UnitManager.Instance().CreateHeroUnit("soldier", 101, GetRandomPosition(born_point2), 2);
		base_unit.robot_base_ai = new RobotBaseAI(base_unit, born_point1);
		
		base_unit = UnitManager.Instance().CreateHeroUnit("soldier", 102, GetRandomPosition(born_point2), 2);
		base_unit.robot_base_ai = new RobotBaseAI(base_unit, born_point1);

		base_unit = UnitManager.Instance().CreateHeroUnit("soldier", 103, GetRandomPosition(born_point2), 2);
		base_unit.robot_base_ai = new RobotBaseAI(base_unit, born_point1);

		base_unit = UnitManager.Instance().CreateHeroUnit("tank", 104, GetRandomPosition(born_point2), 2);
		base_unit.robot_base_ai = new RobotBaseAI(base_unit, born_point1);

		base_unit = UnitManager.Instance().CreateHeroUnit("tank", 105, GetRandomPosition(born_point2), 2);
		base_unit.robot_base_ai = new RobotBaseAI(base_unit, born_point1);

		base_unit = UnitManager.Instance().CreateHeroUnit("tank", 106, GetRandomPosition(born_point2), 2);
		base_unit.robot_base_ai = new RobotBaseAI(base_unit, born_point1);

		base_unit = UnitManager.Instance().CreateHeroUnit("b2", 107, GetRandomPosition(born_point2), 2);
		base_unit.robot_base_ai = new RobotBaseAI(base_unit, born_point1);

		base_unit = UnitManager.Instance().CreateHeroUnit("b2", 108, GetRandomPosition(born_point2), 2);
		base_unit.robot_base_ai = new RobotBaseAI(base_unit, born_point1);

		base_unit = UnitManager.Instance().CreateHeroUnit("f15", 109, GetRandomPosition(born_point2), 2);
		base_unit.robot_base_ai = new RobotBaseAI(base_unit, born_point1);

		base_unit = UnitManager.Instance().CreateHeroUnit("f15", 110, GetRandomPosition(born_point2), 2);
		base_unit.robot_base_ai = new RobotBaseAI(base_unit, born_point1);

		UnitManager.Instance().CreateBuildingUnit("base", 151, born_point2, 2);

		Vector3 born_point3 = new Vector3(18, 0, 9);

		UnitManager.Instance().CreateHeroUnit("soldier", 111, GetRandomPosition(born_point3), 2);
		UnitManager.Instance().CreateHeroUnit("soldier", 112, GetRandomPosition(born_point3), 2);
		UnitManager.Instance().CreateHeroUnit("tank", 113, GetRandomPosition(born_point3), 2);

		Vector3 born_point4 = new Vector3(22, 0, 37);

		UnitManager.Instance().CreateHeroUnit("soldier", 114, GetRandomPosition(born_point4), 2);
		UnitManager.Instance().CreateHeroUnit("soldier", 115, GetRandomPosition(born_point4), 2);
		UnitManager.Instance().CreateHeroUnit("tank", 116, GetRandomPosition(born_point4), 2);

		Vector3 born_point5 = new Vector3(30, 0, 21);

		UnitManager.Instance().CreateHeroUnit("soldier", 117, GetRandomPosition(born_point5), 2);
		UnitManager.Instance().CreateHeroUnit("soldier", 118, GetRandomPosition(born_point5), 2);
		UnitManager.Instance().CreateHeroUnit("tank", 119, GetRandomPosition(born_point5), 2);

	}

	public void InitRealTimeLogic()
	{
		// 我的team id，先写死为1
		real_time_battle_logic = new RealTimeBattleLogic();
		real_time_battle_logic.Init(1);
	}

	// 实时操作接口，单位是不是自己的单位
	public bool IsMyTeam(int team_id)
	{
		return real_time_battle_logic.my_team_id == team_id;
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

	public Vector3 Grid2WorldPosition(int grid_x, int grid_y)
	{
		float half_step = map_data.map_step * 0.5f;
		Vector3 world_positin = new Vector3(grid_x * map_data.map_step + half_step, 0, grid_y * map_data.map_step + half_step);

		return world_positin;
	}

	public bool WorldPositon2Grid(Vector3 world_position, out int grid_x, out int grid_y)
	{
		int x = (int)(world_position.x / map_data.map_step);
		int y = (int)(world_position.z / map_data.map_step);

		if(x >= 0 && x < map_data.map_width && y >= 0 && y < map_data.map_height)
		{
			grid_x = x;
			grid_y = y;
			return true;
		}

		grid_x = -1;
		grid_y = -1;

		return false;
	}

	public float DistanceByGridXY(int x1, int y1, int x2, int y2)
	{
		Vector3 pos1 = Grid2WorldPosition(x1, y1);
		Vector3 pos2 = Grid2WorldPosition(x2, y2);

		return (pos1 - pos2).magnitude;
	}

	public List<AStarNode> FindFlyPath(int x_start, int y_start, int x_end, int y_end)
	{
		if(map_data == null)
		{
			Debug.LogError("FindFlyPath 地图阻挡数组没有初始化! ");
			return null;
		}

		if(x_start < 0 || x_start >= map_data.map_width ||
			y_start < 0 || y_start >= map_data.map_height ||
			x_end < 0 || x_end >= map_data.map_width ||
			y_end < 0 || y_end >= map_data.map_height)
		{
			Debug.LogError("FindFlyPath 地图坐标不再范围内 x_start: " + x_start + " y_start: " + y_start + " x_end: " + x_end + " y_end: " + y_end);
			return null;
		}

		int x_offset = x_end >= x_start ? 1 : -1;
		int y_offset = y_end >= y_start ? 1 : -1;

		int x_current = x_start;
		int y_current = y_start;

		List<AStarNode> path_nodes = new List<AStarNode>();

		AStarNode node = new AStarNode();
		node._x = x_start;
		node._y = y_start;
		path_nodes.Add(node);

		do
		{
			if(x_current == x_end && y_current == y_end)
			{
				node = new AStarNode();
				node._x = x_current;
				node._y = y_current;
				path_nodes.Add(node);
				break;
			}

			if(x_current != x_end)
			{
				x_current += x_offset;
			}

			if(y_current != y_end)
			{
				y_current += y_offset;
			}

			node = new AStarNode();
			node._x = x_current;
			node._y = y_current;
			path_nodes.Add(node);
		}
		while(true);

		return path_nodes;
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

	public void Tick(float delta_time)
	{
		if(real_time_battle_logic != null)
		{
			real_time_battle_logic.Tick(delta_time);
		}
	}
}
