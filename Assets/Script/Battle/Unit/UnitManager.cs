﻿using UnityEngine;
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

	private Transform cache_root_unit_node
	{
		get
		{
			if(_cache_root_unit_node == null)
			{
				GameObject root_node = new GameObject("RootUnitNode");
				_cache_root_unit_node = root_node.transform;

				_cache_root_unit_node.position = Vector3.zero;
			}

			return _cache_root_unit_node;
		}
	}

	private Transform _cache_root_unit_node;

	public Transform cache_root_effect_node
	{
		get
		{
			if(_cache_root_effect_node == null)
			{
				GameObject root_node = new GameObject("RootEffectNode");
				_cache_root_effect_node = root_node.transform;

				_cache_root_effect_node.position = Vector3.zero;
			}

			return _cache_root_effect_node;
		}
	}

	static int _unique_id = 0;
	public static int GetUniqueID()
	{
		return _unique_id++;
	}

	private Transform _cache_root_effect_node;

	public void Destroy()
	{
		EventManager.Instance().UnRegisterEvent(EventConfig.EVENT_L2R_START_MOVE, OnStartMove);

		EventManager.Instance().UnRegisterEvent(EventConfig.EVENT_L2R_END_MOVE, OnEndMove);

		EventManager.Instance().UnRegisterEvent(EventConfig.EVENT_L2R_PLAY_ATTACK, OnPlayAttack);

		EventManager.Instance().UnRegisterEvent(EventConfig.EVENT_L2R_PLAY_HIT, OnPlayHit);

		EventManager.Instance().UnRegisterEvent(EventConfig.EVENT_L2R_PLAY_DEAD, OnPlayDead);
	}

	public void Init()
	{
		EventManager.Instance().RegisterEvent(EventConfig.EVENT_L2R_START_MOVE, OnStartMove);

		EventManager.Instance().RegisterEvent(EventConfig.EVENT_L2R_END_MOVE, OnEndMove);

		EventManager.Instance().RegisterEvent(EventConfig.EVENT_L2R_PLAY_ATTACK, OnPlayAttack);

		EventManager.Instance().RegisterEvent(EventConfig.EVENT_L2R_PLAY_HIT, OnPlayHit);

		EventManager.Instance().RegisterEvent(EventConfig.EVENT_L2R_PLAY_DEAD, OnPlayDead);
	}

	public void OnStartMove(object[] all_params)
	{
		int unit_id = (int)all_params[0];
		HeroUnit unit = GetHeroUnit(unit_id);

		if(unit != null)
		{
			unit.is_move = true;
			unit.PlayMove();
		}
	}

	public void OnEndMove(object[] all_params)
	{
		int unit_id = (int)all_params[0];
		HeroUnit unit = GetHeroUnit(unit_id);

		if(unit != null)
		{
			unit.is_move = false;
			unit.PlayIdle();
		}
	}

	public void OnPlayAttack(object[] all_params)
	{
		int unit_id = (int)all_params[0];
		int target_unit_id = (int)all_params[1];

		HeroUnit unit = GetHeroUnit(unit_id);
		BaseUnit target = GetUnit(target_unit_id);

		if(unit != null && target != null)
		{
			unit.PlayAttack(target);	
		}
	}

	public void OnPlayHit(object[] all_params)
	{
		int unit_id = (int)all_params[0];
		BaseUnit unit = GetUnit(unit_id);

		if(unit != null)
		{
			unit.PlayHited();	
		}
	}

	public void OnPlayDead(object[] all_params)
	{
		int unit_id = (int)all_params[0];
		BaseUnit unit = GetUnit(unit_id);

		if(unit != null)
		{
			unit.PlayDead();	
		}
	}

	public HeroUnit CreateHeroUnit(string unit_name, int id, Vector3 pos)
	{
		GDSKit.unit unit_gds = GDSKit.unit.GetInstance(unit_name);

		GameObject hero_unit_gameobj = ObjectPoolManager.Instance().GetObject(unit_gds.resource_name);
		hero_unit_gameobj.transform.SetParent(cache_root_unit_node);

		HeroUnit hero_unit = hero_unit_gameobj.GetComponent<HeroUnit>();

		// 属性相关设置
		hero_unit.unit_name = unit_name;
		hero_unit.unit_type = UnitType.Hero;
		hero_unit.unit_id = id;
		hero_unit.resource_key = unit_gds.resource_name;
		hero_unit.position = pos;

		if(all_unit_list.ContainsKey(hero_unit.unit_id))
		{
			Debug.LogError("相同名字的unit已经在管理器里了 id : " + hero_unit.unit_id);
			return null;
		}

		all_unit_list.Add(hero_unit.unit_id, hero_unit);
		hero_unit_list.Add(hero_unit.unit_id, hero_unit);

		hero_unit.PlayIdle();

		return hero_unit;
	}

	public void DestroyUnit(string resource_key, int id)
	{
		//Debug.Log("删除的unit id : " + id + " 资源: " + resource_key);

		if(!all_unit_list.ContainsKey(id))
		{
			Debug.LogError("要删除的unit不在管理器里 id : " + id + " 资源: " + resource_key);
			return;
		}

		BaseUnit unit = all_unit_list[id];

		unit.OnClear();

		ObjectPoolManager.Instance().ReturnObject(resource_key, unit.gameObject);

		all_unit_list.Remove(id);

		if(unit.unit_type == UnitType.Hero)
		{
			hero_unit_list.Remove(id);	
		}
		else if(unit.unit_type == UnitType.Building)
		{
			buiding_unit_list.Remove(id);
		}
	}

	public BuildingUnit CreateBuildingUnit(string unit_name, int id, Vector3 pos)
	{
		GDSKit.building unit_gds = GDSKit.building.GetInstance(unit_name);

		GameObject building_unit_gameobj = ObjectPoolManager.Instance().GetObject(unit_gds.resource_name);
		building_unit_gameobj.transform.SetParent(cache_root_unit_node);

		BuildingUnit building_unit = building_unit_gameobj.GetComponent<BuildingUnit>();

		// 属性相关设置
		building_unit.unit_name = unit_name;
		building_unit.unit_type = UnitType.Building;
		building_unit.unit_id = id;
		building_unit.resource_key = unit_gds.resource_name;
		building_unit.position = pos;

		if(all_unit_list.ContainsKey(building_unit.unit_id))
		{
			Debug.LogError("相同名字的unit已经在管理器里了 id : " + building_unit.unit_id);
			return null;
		}

		all_unit_list.Add(building_unit.unit_id, building_unit);
		buiding_unit_list.Add(building_unit.unit_id, building_unit);

		return building_unit;
	}

	private Dictionary<int, HeroUnit>	hero_unit_list = new Dictionary<int, HeroUnit>();
	private Dictionary<int, BuildingUnit> buiding_unit_list = new Dictionary<int, BuildingUnit>();

	public Dictionary<int, BaseUnit> all_unit_list = new Dictionary<int, BaseUnit>();

	public Dictionary<int, BuildingUnit> GetBuildingUnitList()
	{
		return buiding_unit_list;
	}

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

	public BaseUnit GetUnit(int unit_id)
	{
		if(all_unit_list.ContainsKey(unit_id))
		{
			return all_unit_list[unit_id];
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

		var building_enum = buiding_unit_list.GetEnumerator();

		while(building_enum.MoveNext())
		{
			building_enum.Current.Value.Tick(delta_time);
		}
	}

	public void UpdateAllFogOfWar()
	{
		var enumerator = all_unit_list.GetEnumerator();

		while(enumerator.MoveNext())
		{
			enumerator.Current.Value.UpdateTeam();
		}
	}
}
