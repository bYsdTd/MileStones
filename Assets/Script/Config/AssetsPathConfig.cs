using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssetsPathConfig 
{
	public static Dictionary<string, string> assets_path_config = new Dictionary<string, string>()
	{
		{"ToonSoldier", "HeroUnit/Soldier/ToonSoldier"},
		{"BigTank", "HeroUnit/Tank/BigTank"},
		{"UnitSelectCircle", "Effect/UnitSelectCircle"},
		{"fire_effect", "Effect/fire_effect"},
		{"hit_effect1", "Effect/hit_effect1"},
		{"hit_effect2", "Effect/hit_effect2"},
	};
}
