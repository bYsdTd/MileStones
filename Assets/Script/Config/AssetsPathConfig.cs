using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssetsPathConfig 
{
	public static Dictionary<string, string> assets_path_config = new Dictionary<string, string>()
	{
		{"ToonSoldier", "HeroUnit/Soldier/ToonSoldier"},
		{"BigTank", "HeroUnit/Tank/BigTank"},
		{"rocket_tank", "HeroUnit/RocketTank/rocket_tank"},
		{"Stealth_Bomber", "HeroUnit/B2/Prefabs/Stealth_Bomber"},
		{"war_plane_1", "HeroUnit/Aircraft/war_plane_1"},
		{"sm_barak", "BuildingUnit/sm_barak"},
		{"UnitSelectCircle", "Effect/UnitSelectCircle"},
		{"fire_effect", "Effect/fire_effect"},
		{"hit_effect1", "Effect/hit_effect1"},
		{"hit_effect2", "Effect/hit_effect2"},
		{"hit_effect3", "Effect/hit_effect3"},
		{"Missil_01", "Effect/Bullet/Missil_01"},
		{"mat_line", "Grid/mat_line"},
	};
}
