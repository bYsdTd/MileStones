using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleVisionControl  
{
	public Dictionary<int, HashSet<BaseUnit>> vision_enemy_units = new Dictionary<int, HashSet<BaseUnit>>();

	// 每帧更新能看到的单位,
	// 所有的单位共享视野
	public void Tick(float delta_time)
	{
		vision_enemy_units.Clear();

		// 最多8支队伍
		for(int i = 0; i < 8; ++i)
		{
			vision_enemy_units.Add(i + 1, new HashSet<BaseUnit>());	
		}

		var unit_list1 = UnitManager.Instance().all_unit_list;
		var unit_list2 = UnitManager.Instance().all_unit_list;

		var enumerator1 = unit_list1.GetEnumerator();

		while(enumerator1.MoveNext())
		{
			var enumerator2 = unit_list2.GetEnumerator();

			while(enumerator2.MoveNext())
			{
				BaseUnit unit1 = enumerator1.Current.Value;
				BaseUnit unit2 = enumerator2.Current.Value;

				if(unit1.GetTeamID() != unit2.GetTeamID() 
					&& unit1.IsAlive() && unit2.IsAlive() 
					&& unit1.IsCanSeeUnitCheckOnlyMyself(unit2))
				{
					if(!vision_enemy_units[unit1.GetTeamID()].Contains(unit2))
					{
						vision_enemy_units[unit1.GetTeamID()].Add(unit2);	
					}
				}
			}
		}

	}
}
