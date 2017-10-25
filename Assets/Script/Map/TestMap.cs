using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestMap : MonoBehaviour {

	int width = 10;
	int height = 10;
	MapGrid map_grid = null;
	List<AStarNode> path_nodes = new List<AStarNode>();

	int start_x = 3;
	int start_y = 4;

	int end_x = 9;
	int end_y = 9;

	// Use this for initialization
	void Start () 
	{
		map_grid = new MapGrid();
		map_grid.Init(width, height);

		for(int x = 0; x < width; ++x )
		{
			for(int y = 0; y < height; ++y)
			{
				map_grid.SetBlock(x, y, false);		
			}
		}

		map_grid.SetBlock(4, 3, true);
		map_grid.SetBlock(5, 3, true);
		map_grid.SetBlock(6, 3, true);

		map_grid.SetBlock(6, 4, true);

		map_grid.SetBlock(4, 5, true);
		map_grid.SetBlock(5, 5, true);
		map_grid.SetBlock(6, 5, true);

		path_nodes.Clear();
		path_nodes = map_grid.FindPath(start_x, start_y, end_x, end_y);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	bool IsPathNode(int x, int y)
	{
		for(int i = 0; i < path_nodes.Count; ++i)
		{
			if(x == path_nodes[i]._x && y == path_nodes[i]._y)
			{
				return true;
			}
		}

		return false;
	}

	bool IsStartNode(int x, int y)
	{
		return x == start_x && y == start_y;	
	}

	bool IsEndNode(int x, int y)
	{
		return x == end_x && y == end_y;	
	}

	void OnDrawGizmos()
	{
		if(map_grid == null)
		{
			return;
		}

		for(int x = 0; x < width; ++x )
		{
			for(int y = 0; y < height; ++y)
			{

				Vector3 center = new Vector3(x, 0, y);
				Vector3 size = Vector3.one * 0.9f;

				if(IsPathNode(x, y))
				{
					if(IsStartNode(x, y))
					{
						Gizmos.color = Color.blue;
					}
					else if(IsEndNode(x, y))
					{
						Gizmos.color = Color.red;
					}
					else
					{
						Gizmos.color = Color.green;	
					}
				}
				else if(map_grid.IsBlock(x, y))
				{
					Gizmos.color = Color.black;
				}
				else
				{
					Gizmos.color = Color.white;
				}

				Gizmos.DrawCube(center, size);
			}
		}
	}
}
