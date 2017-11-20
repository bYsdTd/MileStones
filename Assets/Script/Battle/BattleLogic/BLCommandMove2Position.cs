using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
	public class BLCommandMove2Position : BLCommandBase
	{
		public BLIntVector3		dest_position;

		private BLIntVector3	start_position;

		private int				current_path_node_index;

		private List<BLIntVector3>	path_node;

		private BLUnitHero		hero_unit;

		public override void OnInit ()
		{
			base.OnInit();

			command_type = BLCommandType.Move;

			start_position = cast_unit.position;

			path_node = BattleField.battle_field.SearchPath(start_position, dest_position);

			current_path_node_index = 0;

			hero_unit = cast_unit as BLUnitHero;

			hero_unit.pre_position = start_position;

			EventManager.Instance().PostEvent(EventConfig.EVENT_UNIT_START_MOVE, new object[]{ hero_unit.unit_id});
		}

		public override void OnDestroy ()
		{
			base.OnDestroy();

			EventManager.Instance().PostEvent(EventConfig.EVENT_UNIT_END_MOVE, new object[]{ hero_unit.unit_id});
		}

		public override bool Tick ()
		{
			base.Tick ();

			if(current_path_node_index >= path_node.Count)
			{
				return true;
			}

			BLIntVector3 next_node_pos = path_node[current_path_node_index];

			// 接近0.01的时候 就是 10毫米 * 10毫米 
			if(BLIntVector3.DistanceSqr(next_node_pos, hero_unit.position) < 10000)
			{
				++current_path_node_index;

				if(current_path_node_index >= path_node.Count)
				{
					return true;
				}
			}

			next_node_pos = path_node[current_path_node_index];

			BLIntVector3 dir = next_node_pos - hero_unit.position;

			Vector3 d = hero_unit.move_speed * BLTimelineController.MS_PER_FRAME * dir.Normalize();

			BLIntVector3 changed_pos = new BLIntVector3((int)d.x, (int)d.y, (int)d.z);

			hero_unit.pre_position = hero_unit.position;
			hero_unit.position = hero_unit.position + changed_pos;

			return false;
		}
	}	
}
