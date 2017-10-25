using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitManager  
{
	static private UnitManager instance = null;

	static public UnitManager Instance()
	{
		if(instance == null)
		{
			instance = new UnitManager();
		}

		return instance;
	}

	public HeroUnit CreateHeroUnit(int id)
	{
		HeroUnit hero_unit = ObjectPoolManager.Instance().GetObject("KnightWarrior").GetComponent<HeroUnit>();

		hero_unit.Init();

		hero_unit.unit_id = id;

		hero_unit.PlayIdle();

		AddHeroUnit(hero_unit);

		return hero_unit;
	}

	public Dictionary<int, HeroUnit>	hero_unit_list = new Dictionary<int, HeroUnit>();

	public void Tick(float delta_time)
	{
		var enumerator = hero_unit_list.GetEnumerator();

		while(enumerator.MoveNext())
		{
			enumerator.Current.Value.Tick(delta_time);
		}
	}

	public void AddHeroUnit(HeroUnit unit)
	{
		if(hero_unit_list.ContainsKey(unit.unit_id))
		{
			Debug.LogError("相同名字的unit已经在管理器里了 id : " + unit.unit_id);
			return;
		}

		hero_unit_list.Add(unit.unit_id, unit);
	}
}
