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

	private Transform cache_root_unit_node = null;

	public HeroUnit CreateHeroUnit(int id)
	{
		if(cache_root_unit_node == null)
		{
			GameObject root_node = new GameObject("RootUnitNode");
			cache_root_unit_node = root_node.transform;

			cache_root_unit_node.position = Vector3.zero;
		}

		GameObject hero_unit_gameobj = ObjectPoolManager.Instance().GetObject("KnightWarrior");
		hero_unit_gameobj.transform.SetParent(cache_root_unit_node);

		HeroUnit hero_unit = hero_unit_gameobj.GetComponent<HeroUnit>();

		// 属性相关设置
		hero_unit.unit_id = id;
		hero_unit.SetMoveSpeedGrid(2);
		hero_unit.SetAttackRange(2);
		hero_unit.SetAttackVision(3);


		hero_unit.Init();

		hero_unit.PlayIdle();

		AddHeroUnit(hero_unit);

		return hero_unit;
	}

	public Dictionary<int, HeroUnit>	hero_unit_list = new Dictionary<int, HeroUnit>();

	public HeroUnit GetHeroUnit(int unit_id)
	{
		if(hero_unit_list.ContainsKey(unit_id))
		{
			return hero_unit_list[unit_id];
		}
		else
		{
			return null;
		}
	}

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
