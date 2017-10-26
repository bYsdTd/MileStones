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
	private List<Vector3> postion_at_node = null;

	public override void OnStart ()
	{
		base.OnStart ();

		path_nodes = BattleField.battle_field.FindPath(start_grid_x, start_grid_y, end_grid_x, end_grid_y);

//		for(int i = 0; i < path_nodes.Count; ++i)
//		{
//			Debug.Log(path_nodes[i]._x + "  " + path_nodes[i]._y);
//		}

		if(path_nodes != null)
		{
			time_at_node = new List<float>();
			postion_at_node = new List<Vector3>();

			for(int node_index = 0; node_index < path_nodes.Count; ++node_index)
			{
				Vector3 world_position = BattleField.battle_field.Grid2WorldPosition(path_nodes[node_index]._x, path_nodes[node_index]._y);
				postion_at_node.Add(world_position);

				if(node_index == 0)
				{
					time_at_node.Add(0);
					continue;
				}

				AStarNode pre_node = path_nodes[node_index-1];
				AStarNode current_node = path_nodes[node_index];

				float distance = BattleField.battle_field.DistanceByGridXY(pre_node._x, pre_node._y, current_node._x, current_node._y);
				float time_needed = distance / hero_unit.move_speed;

				time_at_node.Add(time_needed);
			}
		}

		hero_unit.PlayMove();
	}

	public override void OnEnd ()
	{
		base.OnEnd ();

		hero_unit.PlayIdle();
	}

	public override bool Tick (float delta_time)
	{
		base.Tick (delta_time);

		if(path_nodes == null)
		{
			return true;	
		}

		time_elapsed += delta_time;

		//int frame_elapsed = (int)(time_elapsed / BattleField.TIME_PER_FRAME);

		//current_frame = start_frame + frame_elapsed;

		float node_time_all = 0;

		int current_pre_node_index = -1;
		float time_elapsed_from_pre_node = time_elapsed;

		for(int i = 1; i < time_at_node.Count; ++i)
		{
			node_time_all += time_at_node[i];

			time_elapsed_from_pre_node -= time_at_node[i-1];

			if(time_elapsed < node_time_all)
			{
				current_pre_node_index = i-1;
				break;
			}
		}

		if(current_pre_node_index < 0)
		{
			OnEnd();

			return true;
		}

		Vector3 current_start_pos = postion_at_node[current_pre_node_index];
		Vector3 current_end_pos = postion_at_node[current_pre_node_index + 1];
		float current_time_span = time_at_node[current_pre_node_index + 1];

		Vector3 current_pos = Vector3.Lerp(current_start_pos, current_end_pos, time_elapsed_from_pre_node/current_time_span);

		Vector3 dir = current_end_pos - current_start_pos;

		hero_unit.SetPosition(current_pos);
		hero_unit.SetDirection(dir);

		return false;
	}
}
