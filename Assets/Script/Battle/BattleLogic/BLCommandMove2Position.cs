using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
	public class BLCommandMove2Position : BLCommandBase
	{
		public BLIntVector3		dest_position;

		public Vector3			current_dir { set; get; }

		private BLIntVector3		start_position;
		private List<BLIntVector3>	final_path_node = new List<BLIntVector3>();
		private List<int>			frame_at_node = new List<int>();
		private BLUnitHero			hero_unit;

		private int					current_node_index;
		private int					pre_node_index;
		private int					current_frame;

		public override void OnInit ()
		{
			base.OnInit();

			hero_unit = cast_unit as BLUnitHero;

			command_type = BLCommandType.Move;

			start_position = hero_unit.position;

			// 准备数据
			List<BLIntVector3> path_node = BattleField.battle_field.SearchPath(start_position, dest_position);

			frame_at_node.Clear();
			final_path_node.Clear();

			if(path_node != null && path_node.Count > 0)
			{
				BLIntVector3 pre_node = path_node[0];

				int frame = 0;
				frame_at_node.Add(frame);
				final_path_node.Add(pre_node);

				for(int i = 1; i < path_node.Count; ++i)
				{
					BLIntVector3 current_node = path_node[i];

					int magnitude = (current_node - pre_node).Magnitude();
					int need_frame = magnitude / hero_unit.move_speed / BLTimelineController.MS_PER_FRAME;

					if(need_frame > 0)
					{
						frame += need_frame;

						frame_at_node.Add(frame);

						final_path_node.Add(path_node[i]);

						//Debug.Log("frame at node i " + i + " frame " + frame);	
					}

					pre_node = current_node;
				}
			}

			pre_node_index = 0;
			current_node_index = 1;
			current_frame = 0;


			hero_unit.pre_position = start_position;

			current_dir = (hero_unit.position - hero_unit.pre_position).Vector3Value();

			EventManager.Instance().PostEvent(EventConfig.EVENT_UNIT_START_MOVE, new object[]{ hero_unit.unit_id});	
		}

		public override void OnDestroy ()
		{
			base.OnDestroy();

			EventManager.Instance().PostEvent(EventConfig.EVENT_UNIT_END_MOVE, new object[]{ hero_unit.unit_id});
		}

		public override bool Tick ()
		{
			//Debug.Log("current_frame " + current_frame + " current_node_index " + current_node_index + " pre_node_index " + pre_node_index);

			if(final_path_node == null || final_path_node.Count == 0)
			{
				return true;
			}

			if(current_frame == frame_at_node[current_node_index])
			{
				++current_node_index;
				++pre_node_index;
			}

			// 已经到最后一个陆经点
			if(current_node_index == final_path_node.Count)
			{
				return true;
			}

			BLIntVector3 next_position =  final_path_node[current_node_index];
			BLIntVector3 pre_position = final_path_node[pre_node_index];

			int frame_span = frame_at_node[current_node_index] - frame_at_node[pre_node_index];
			int frame_from_pre_node = current_frame - frame_at_node[pre_node_index];

			BLIntVector3 now_position = BLIntVector3.Lerp(pre_position, next_position, frame_from_pre_node, frame_span);

			//Debug.Log("pre_position " + pre_position + " next_position " + next_position + " now_position " + now_position);

			hero_unit.pre_position = hero_unit.position;
			hero_unit.position = now_position;

			current_dir = (next_position - pre_position).Vector3Value();

			hero_unit.dir = current_dir;

			++current_frame;

			return false;
		}
	}	
}
