using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingUnit : BaseUnit 
{
	public Transform[]	revive_point_list;

	public bool can_revive_hero
	{
		get;
		set;
	}

	class ReviveUnitData
	{
		public int 		unit_id;
		public int		team_id;
		public float 	revive_cool_down;
		public string 	revive_unit_name;
	}

	List<ReviveUnitData>	revive_unit_cool_down_list = new List<ReviveUnitData>();

	public override void Tick (float delta_time)
	{
		for(int i = revive_unit_cool_down_list.Count-1;i >= 0 ; i--)
		{
			ReviveUnitData unit_revive_data = revive_unit_cool_down_list[i];

			unit_revive_data.revive_cool_down -= delta_time;

			if(unit_revive_data.revive_cool_down <= 0)
			{
				DoReviveHeroUnit(unit_revive_data);

				revive_unit_cool_down_list.RemoveAt(i);
			}
		}
	}

	private void DoReviveHeroUnit(ReviveUnitData revive_data)
	{
		int count = revive_point_list.Length;

		if(count > 0)
		{
			int rand_index = Random.Rand(0, count-1);

			Vector3 revive_position = revive_point_list[rand_index].position;

			UnitManager.Instance().CreateHeroUnit(revive_data.revive_unit_name, revive_data.unit_id, revive_position, revive_data.team_id);
		}
		else
		{
			Debug.LogError("建筑没有绑定复活点, 请添加复活点坐标! " + unit_name + " id " + unit_id);
		}
	}

	public void AddReviveUnit(string hero_unit_name, int unit_id, int team_id)
	{
		GDSKit.unit unit_gds = GDSKit.unit.GetInstance(hero_unit_name);
		ReviveUnitData data = new ReviveUnitData();
		data.revive_cool_down = unit_gds.revive_cd;
		data.revive_unit_name = hero_unit_name;
		data.unit_id = unit_id;
		data.team_id = team_id;

		revive_unit_cool_down_list.Add(data);
	}
}
