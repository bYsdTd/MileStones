using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleVisionControl  
{
	public Dictionary<int, HashSet<HeroUnit>> vision_enemy_units = new Dictionary<int, HashSet<HeroUnit>>();

	public RealTimeBattleLogic my_real_time_logic = null;

	// 每帧更新能看到的单位,
	// 所有的单位共享视野
	public void Tick(float delta_time)
	{
		vision_enemy_units.Clear();

		// 最多8支队伍
		for(int i = 0; i < 8; ++i)
		{
			vision_enemy_units.Add(i + 1, new HashSet<HeroUnit>());	
		}

		var unit_list1 = UnitManager.Instance().hero_unit_list;
		var unit_list2 = UnitManager.Instance().hero_unit_list;

		var enumerator1 = unit_list1.GetEnumerator();

		while(enumerator1.MoveNext())
		{
			var enumerator2 = unit_list2.GetEnumerator();

			while(enumerator2.MoveNext())
			{
				HeroUnit unit1 = enumerator1.Current.Value;
				HeroUnit unit2 = enumerator2.Current.Value;

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

//		List<HeroUnit> my_unit_list = new List<HeroUnit>();
//		List<HeroUnit> enemy_unit_list = new List<HeroUnit>();
//
//		for(int i = 0; i < unit_list.Count; ++i)
//		{
//			HeroUnit unit_temp = unit_list[i];
//
//			if(my_real_time_logic.my_team_id == unit_temp.team_id)
//			{
//				// 看自己的所有单位是否能看到
//
//				my_unit_list.Add(unit_temp);
//			}
//			else
//			{
//				enemy_unit_list.Add(unit_temp);
//			}
//		}
//
//		for(int i = 0; i < enemy_unit_list.Count; ++i)
//		{
//			for(int my_unit_index = 0; my_unit_index < my_unit_list.Count; ++my_unit_index)
//			{
//				HeroUnit enemy_unit = enemy_unit_list[i];
//				HeroUnit my_unit = my_unit_list[my_unit_index];
//
//				if(my_unit.IsCanSeeUnit(enemy_unit))
//				{
//					vision_enemy_units.Add(enemy_unit);
//				}
//			}
//		}
	}
}
