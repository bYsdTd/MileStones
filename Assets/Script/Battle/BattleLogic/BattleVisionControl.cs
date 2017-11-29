using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BL
{
	public class BattleVisionControl  
	{
		static private BattleVisionControl instance = null;

		static public BattleVisionControl Instance()
		{
			if(instance == null)
			{
				instance = new BattleVisionControl();
			}

			return instance;
		}

		public Dictionary<int, HashSet<BLUnitBase>> vision_enemy_units = new Dictionary<int, HashSet<BLUnitBase>>();

		public HashSet<BLUnitBase> GetEnemys(int team_id)
		{
			return vision_enemy_units[team_id];
		}

		// 每帧更新能看到的单位,
		// 所有的单位共享视野
		public void Tick()
		{
			vision_enemy_units.Clear();

			// 最多8支队伍
			for(int i = 0; i < BattleField.MAX_PLAYER; ++i)
			{
				vision_enemy_units.Add(i + 1, new HashSet<BLUnitBase>());	
			}

			var unit_list1 = BLUnitManager.Instance().GetAllUnitList();
			var unit_list2 = BLUnitManager.Instance().GetAllUnitList();

			var enumerator1 = unit_list1.GetEnumerator();

			while(enumerator1.MoveNext())
			{
				var enumerator2 = unit_list2.GetEnumerator();

				while(enumerator2.MoveNext())
				{
					BLUnitBase unit1 = enumerator1.Current.Value;
					BLUnitBase unit2 = enumerator2.Current.Value;

					if(unit1.team_id != unit2.team_id 
						&& unit1.IsAlive() && unit2.IsAlive() 
						&& unit1.IsCanSeeUnitCheckOnlyMyself(unit2))
					{
						if(!vision_enemy_units[unit1.team_id].Contains(unit2))
						{
							vision_enemy_units[unit1.team_id].Add(unit2);
						}
					}
				}
			}

		}
	}
}
