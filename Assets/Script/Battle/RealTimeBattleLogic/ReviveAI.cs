using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReviveAI 
{
	public HeroUnit my_unit;

	public BuildingUnit FindNeareatReviveBuilding()
	{
		var building_list = UnitManager.Instance().GetBuildingUnitList();

		float distance = float.MaxValue;

		BuildingUnit revive_building = null;

		var enumerator = building_list.GetEnumerator();
		while(enumerator.MoveNext())
		{
			BuildingUnit building_unit = enumerator.Current.Value;

			if(building_unit.GetTeamID() == my_unit.GetTeamID())
			{
				float temp = (building_unit._position - my_unit._position).sqrMagnitude;

				if(temp < distance)
				{
					revive_building = building_unit;
				}
			}
		}


		return revive_building;
	}

}
