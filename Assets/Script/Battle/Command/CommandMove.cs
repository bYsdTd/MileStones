using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommandMove : CommandBase 
{
	public int start_grid_x;
	public int start_grid_y;

	public int end_grid_x;
	public int end_grid_y;

	private List<AStarNode> path_nodes = null;

	private List<float> time_at_node = null;

	public override void OnStart ()
	{
		base.OnStart ();

		path_nodes = CommandManager.Instance().battle_field.FindPath(start_grid_x, start_grid_y, end_grid_x, end_grid_y);

		for(int i = 0; i < path_nodes.Count; ++i)
		{
			Debug.Log(path_nodes[i]._x + "  " + path_nodes[i]._y);
		}

		if(path_nodes != null)
		{
//			time_at_node = new List<float>();
//			time_at_node.Capacity = path_nodes.Count;
//
//			time_at_node[0] = 0;
//
//			for(int node_index = 1; node_index < path_nodes.Count; ++node_index)
//			{
//				
//				//time_at_node[node_index] = 
//			}
		}
	}

	public override void OnEnd ()
	{
		base.OnEnd ();
	}

	public override bool Tick (float delta_time)
	{
		base.Tick (delta_time);

		if(path_nodes == null)
		{
			return true;	
		}

		time_elapsed += delta_time;

		int frame_elapsed = (int)(time_elapsed / BattleField.TIME_PER_FRAME);

		current_frame = start_frame + frame_elapsed;

		return false;
	}
}
