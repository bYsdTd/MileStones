using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
	public class BLCommandPutHero : BLCommandBase
	{
		public int hero_index { set; get; }
		public int player_id { set; get; }
		public int team_id { set; get; }

		public override void OnInit ()
		{
			base.OnInit();

			command_type = TickCommandType.PUT_HERO;
		}

		public override void DoCommand ()
		{
			base.DoCommand ();

			//Debug.Log("放置英雄在第: " + BLTimelineController.Instance().current_logic_frame + " 帧.");

			// 暂时用team id 代替player id，在房间内是不重复的
			BLPlayerData player_data = BLPlayerDataManager.Instance().GetPlayerData(player_id);

			// 检查是否为可放置的状态
			if(player_data.IsCanPutHero(hero_index))
			{
				BLPlayerHeroData hero_data = player_data.GetHeroData(hero_index);
				BL.BLIntVector3 born_point = BattleField.battle_field.born_point[player_id];
				int unit_id = BL.BLUnitManager.Instance().GetHeroUnitID(player_id, hero_index);
				string unit_gds_name = hero_data.hero_gds_name;
				BL.BLIntVector3 born_pos = BL.BLUnitManager.Instance().GetRandomPosition(born_point);

				BL.BLUnitManager.Instance().CreateHeroUnit(unit_id, unit_gds_name, born_pos, team_id);

				// 建筑上移除
				BLUnitBase unit = BLUnitManager.Instance().GetUnit(hero_data.build_site_id);
				Debug.Assert(unit.unit_type == UnitType.Building);
				BLUnitBuilding build_unit = unit as BLUnitBuilding;

				build_unit.RemoveUnit(unit_id);

			}
		}
	}	
}
