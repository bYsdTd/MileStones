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
}
