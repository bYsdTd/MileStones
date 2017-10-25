using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGrid 
{
	public int _width;
	public int _height;

	// 二维数组，表示哪个位置有阻挡
	public bool[,] _grid_array;


	public void Init(int width, int height)
	{
		_width = width;
		_height = height;

		_grid_array = new bool[width, height];
	}

	public void SetBlock(int x, int y, bool value)
	{
		_grid_array[x,y] = value;
	}

	public bool IsBlock(int x, int y)
	{
		return _grid_array[x, y];
	}

	// 定义一个距离的映射
	static int[,] distance_map = new int[3,3]
	{
		{14, 10, 14, },
		{10,  0, 10, },
		{14, 10, 14, },
	};

	public List<AStarNode> FindPath(int x_start, int y_start, int x_end, int y_end)
	{
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

					if(x < 0 || x >= _width || y < 0 || y >= _height)
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

						while(path_temp_node._parent != null)
						{
							path_nodes.Add(path_temp_node._parent);

							path_temp_node = path_temp_node._parent;
						}


						path_nodes.Reverse();

						return path_nodes;
					}

					// 如果是阻挡，不处理
					if(_grid_array[x,y])
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
