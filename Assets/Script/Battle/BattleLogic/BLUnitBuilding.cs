using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
	public class BLUnitBuilding : BLUnitBase
	{
		public bool can_revive_hero { set; get; }

		class ReviveUnitData
		{
			public int 		unit_id;
			public int 		revive_need_frames;
			public string 	revive_unit_gds_name;
		}

		List<ReviveUnitData>	revive_unit_cool_down_list = new List<ReviveUnitData>();

		public override void Tick ()
		{
			base.Tick();

			for(int i = revive_unit_cool_down_list.Count-1;i >= 0 ; i--)
			{
				ReviveUnitData unit_revive_data = revive_unit_cool_down_list[i];

				unit_revive_data.revive_need_frames--;

				if(unit_revive_data.revive_need_frames <= 0)
				{
					unit_revive_data.revive_need_frames = 0;
				}
			}
		}

//		private void DoReviveHeroUnit(ReviveUnitData revive_data)
//		{
//			int count = revive_point_list.Length;
//
//			if(count > 0)
//			{
//				int rand_index = Utils.Random.Rand(0, count-1);
//
//				Vector3 revive_position = revive_point_list[rand_index].position;
//
//				//			UnitManager.Instance().CreateHeroUnit(revive_data.revive_unit_name, revive_data.unit_id, revive_position, revive_data.team_id);
//			}
//			else
//			{
//				Debug.LogError("建筑没有绑定复活点, 请添加复活点坐标! " + unit_name + " id " + unit_id);
//			}
//		}


		public void AddReviveUnit(string hero_unit_name, int unit_id, int revive_need_frames)
		{
			ReviveUnitData data = new ReviveUnitData();
			data.revive_need_frames = revive_need_frames;
			data.revive_unit_gds_name = hero_unit_name;
			data.unit_id = unit_id;

			revive_unit_cool_down_list.Add(data);
		}

		public void RemoveUnit(int unit_id)
		{
			for(int i = revive_unit_cool_down_list.Count-1;i >= 0 ; i--)
			{
				ReviveUnitData unit_revive_data = revive_unit_cool_down_list[i];

				if(unit_revive_data.unit_id == unit_id)
				{
					revive_unit_cool_down_list.RemoveAt(i);

					return;
				}
			}
		}

		private ReviveUnitData GetUnitReviveData(int unit_id)
		{
			ReviveUnitData revive_unit_data = null;

			for(int i = revive_unit_cool_down_list.Count-1;i >= 0 ; i--)
			{
				ReviveUnitData revive_data = revive_unit_cool_down_list[i];

				if(revive_data.unit_id == unit_id)
				{
					revive_unit_data = revive_data;
					break;
				}
			}

			return revive_unit_data;
		}

		public int GetUnitReviveRemainFrames(int unit_id)
		{
			ReviveUnitData revive_unit_data = GetUnitReviveData(unit_id);

			if(revive_unit_data != null)
			{
				return revive_unit_data.revive_need_frames;
			}
			else
			{
				return 0;
			}
		}

		public bool IsUnitCanRevive(int unit_id)
		{
			ReviveUnitData revive_unit_data = GetUnitReviveData(unit_id);

			if(revive_unit_data != null)
			{
				return revive_unit_data.revive_need_frames <= 0;
			}
			else
			{
				return false;
			}
		}
	}	
}
