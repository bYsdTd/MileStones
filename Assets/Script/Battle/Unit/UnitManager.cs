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

	public HeroUnit CreateHeroUnit(string unit_name, int id, Vector3 pos, int team_id)
	{
		GDSKit.unit unit_gds = GDSKit.unit.GetInstance(unit_name);

		GameObject hero_unit_gameobj = ObjectPoolManager.Instance().GetObject(unit_gds.resource_name);
		hero_unit_gameobj.transform.SetParent(cache_root_unit_node);

		HeroUnit hero_unit = hero_unit_gameobj.GetComponent<HeroUnit>();

		// 属性相关设置
		hero_unit.unit_name = unit_name;
		hero_unit.unit_type = UnitType.Hero;
		hero_unit.unit_id = id;
		hero_unit.revive_cd = unit_gds.revive_cd;
		hero_unit.SetMoveSpeedGrid(unit_gds.move_speed);
		hero_unit.SetAttackRange(unit_gds.attack_range);
		hero_unit.SetAttackVision(unit_gds.attack_vision);
		hero_unit.attack_speed = unit_gds.attack_speed;
		hero_unit.unit_attack = unit_gds.unit_attack;
		hero_unit.unit_hp = unit_gds.unit_hp;
		hero_unit.resource_key = unit_gds.resource_name;
		hero_unit.is_move_attack = unit_gds.is_move_attack;
		hero_unit.is_fly = unit_gds.is_fly;
		hero_unit.can_attack_fly = unit_gds.can_attack_fly;
		hero_unit.can_attack_ground = unit_gds.can_attack_ground;

		int grid_x;
		int grid_y;

		BattleField.battle_field.WorldPositon2Grid(pos, out grid_x, out grid_y);
		hero_unit.SetPosition(BattleField.battle_field.Grid2WorldPosition(grid_x, grid_y));

		hero_unit.SetTeamID(team_id);

		// 攻击AI
		hero_unit.InitAI();

		if(all_unit_list.ContainsKey(hero_unit.unit_id))
		{
			Debug.LogError("相同名字的unit已经在管理器里了 id : " + hero_unit.unit_id);
			return null;
		}

		all_unit_list.Add(hero_unit.unit_id, hero_unit);
		hero_unit_list.Add(hero_unit.unit_id, hero_unit);

		hero_unit.OnInit();

		hero_unit.PlayIdle();

		return hero_unit;
	}

	public void DestroyUnit(string resource_key, int id)
	{
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

	public BuildingUnit CreateBuildingUnit(string unit_name, int id, Vector3 pos, int team_id)
	{
		GDSKit.building unit_gds = GDSKit.building.GetInstance(unit_name);

		GameObject building_unit_gameobj = ObjectPoolManager.Instance().GetObject(unit_gds.resource_name);
		building_unit_gameobj.transform.SetParent(cache_root_unit_node);

		BuildingUnit building_unit = building_unit_gameobj.GetComponent<BuildingUnit>();

		// 属性相关设置
		building_unit.unit_name = unit_name;
		building_unit.unit_type = UnitType.Building;
		building_unit.unit_id = id;
		building_unit.SetAttackVision(unit_gds.vision);
		building_unit.unit_hp = unit_gds.building_hp;
		building_unit.resource_key = unit_gds.resource_name;
		building_unit.can_revive_hero = unit_gds.can_revive_hero;

		int grid_x;
		int grid_y;

		BattleField.battle_field.WorldPositon2Grid(pos, out grid_x, out grid_y);
		building_unit.SetPosition(BattleField.battle_field.Grid2WorldPosition(grid_x, grid_y));

		building_unit.SetTeamID(team_id);


		if(all_unit_list.ContainsKey(building_unit.unit_id))
		{
			Debug.LogError("相同名字的unit已经在管理器里了 id : " + building_unit.unit_id);
			return null;
		}

		all_unit_list.Add(building_unit.unit_id, building_unit);
		buiding_unit_list.Add(building_unit.unit_id, building_unit);

		building_unit.OnInit();

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
}
