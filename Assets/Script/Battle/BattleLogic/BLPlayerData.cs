using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
	public class BLPlayerData
	{
		// 目前team id 就是player id，就是在这个房间的唯一标识
		// 之后如果有结盟的话，就需要增加一个字段，用作玩家的唯一标识
		public int team_id { set; get; }
		public int player_id { set; get; }

		// 资源量, 建筑自动增长
		public int resource_count { set; get; }


		public List<BLPlayerHeroData>	all_hero_data = new List<BLPlayerHeroData>();

		public string[] hero_config = new string[]{

			"soldier",
			"soldier",
			"tank",
			"tank",
			"b2",
			"f15",
			"rocket_car",
			"rocket_car",
		};

		public void Init(int player_id_, int team_id_)
		{
			team_id = team_id_;
			player_id = player_id_;

			for(int i = 0; i < hero_config.Length; ++i)
			{
				int hero_index = i;
				BLPlayerHeroData hero_data = new BLPlayerHeroData();

				hero_data.hero_gds_name = hero_config[hero_index];
				hero_data.build_site_id = BLUnitManager.Instance().GetBaseID(player_id);

				BLUnitBase unit = BLUnitManager.Instance().GetUnit(hero_data.build_site_id);

				if(unit != null)
				{
					Debug.Assert(unit.unit_type == UnitType.Building);
					BLUnitBuilding build_unit = unit as BLUnitBuilding;

					int unit_id = BLUnitManager.Instance().GetHeroUnitID(player_id, hero_index);
					build_unit.AddReviveUnit(hero_data.hero_gds_name, unit_id, 0);
				}

				all_hero_data.Add(hero_data);
			}
		}

		public void Destroy()
		{
			
		}

		public BLPlayerHeroData GetHeroData(int hero_index)
		{
			return all_hero_data[hero_index];
		}

		public bool IsHeroAlreadyPut(int hero_index)
		{
			int unit_id = BLUnitManager.Instance().GetHeroUnitID(player_id, hero_index);

			return BLUnitManager.Instance().GetUnit(unit_id) != null;
		}

		public bool IsCanPutHero(int hero_index)
		{
			if(IsHeroAlreadyPut(hero_index))
			{
				// 已经放置了这个将军
				return false;
			}

			BLPlayerHeroData hero_data = GetHeroData(hero_index);
			int unit_id = BLUnitManager.Instance().GetHeroUnitID(player_id, hero_index);

			// 检查资源

			// 检查复活状态
			BLUnitBase unit = BLUnitManager.Instance().GetUnit(hero_data.build_site_id);
			Debug.Assert(unit.unit_type == UnitType.Building);
			BLUnitBuilding build_unit = unit as BLUnitBuilding;

			return build_unit.IsUnitCanRevive(unit_id);
		}
	}
}

