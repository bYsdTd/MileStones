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

	public void OnInit()
	{
		EventManager.Instance().RegisterEvent(EventConfig.EVENT_UNIT_START_MOVE, delegate(object[] all_params) {

			int unit_id = (int)all_params[0];
			HeroUnit unit = GetHeroUnit(unit_id);

			unit.is_move = true;
			unit.PlayMove();

		});

		EventManager.Instance().RegisterEvent(EventConfig.EVENT_UNIT_END_MOVE, delegate(object[] all_params) {
			
			int unit_id = (int)all_params[0];
			HeroUnit unit = GetHeroUnit(unit_id);
			unit.is_move = false;
			unit.PlayIdle();

		});	
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

		int grid_x;
		int grid_y;

		BattleField.battle_field.WorldPositon2Grid(pos, out grid_x, out grid_y);
		hero_unit.position = BattleField.battle_field.Grid2WorldPosition(grid_x, grid_y);

		if(all_unit_list.ContainsKey(hero_unit.unit_id))
		{
			Debug.LogError("相同名字的unit已经在管理器里了 id : " + hero_unit.unit_id);
			return null;
		}

		all_unit_list.Add(hero_unit.unit_id, hero_unit);
		hero_unit_list.Add(hero_unit.unit_id, hero_unit);

		hero_unit.OnInit();

		hero_unit.Idle();

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
		building_unit.attack_vision = unit_gds.vision;
		building_unit.unit_hp = unit_gds.building_hp;
		building_unit.resource_key = unit_gds.resource_name;
		building_unit.can_revive_hero = unit_gds.can_revive_hero;

		int grid_x;
		int grid_y;

		BattleField.battle_field.WorldPositon2Grid(pos, out grid_x, out grid_y);
		building_unit.position = BattleField.battle_field.Grid2WorldPosition(grid_x, grid_y);

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

	public void InitUnit()
	{
//		// team 1
//		Vector3 born_point1 = new Vector3(5, 0, 30);
//
//		CreateHeroUnit("soldier", 0, GetRandomPosition(born_point1), 1);
//		CreateHeroUnit("soldier", 1, GetRandomPosition(born_point1), 1);

//		CreateHeroUnit("tank", 2, GetRandomPosition(born_point1), 1);
//		CreateHeroUnit("tank", 3, GetRandomPosition(born_point1), 1);
//		CreateHeroUnit("tank", 4, GetRandomPosition(born_point1), 1);
//
//
//
//		CreateHeroUnit("b2", 5, GetRandomPosition(born_point1), 1);
//		CreateHeroUnit("b2", 6, GetRandomPosition(born_point1), 1);
//
//		CreateHeroUnit("f15", 7, GetRandomPosition(born_point1), 1);
//		CreateHeroUnit("f15", 8, GetRandomPosition(born_point1), 1);
//
//
//		CreateHeroUnit("rocket_car", 9, GetRandomPosition(born_point1), 1);
//
//		CreateBuildingUnit("base", 51, born_point1, 1);


//		// team 2
//
//		Vector3 born_point2 = new Vector3(50, 0, 10);
//
//		HeroUnit base_unit = CreateHeroUnit("soldier", 101, GetRandomPosition(born_point2), 2);
//		base_unit.robot_base_ai = new RobotBaseAI(base_unit, born_point1);
//
//		base_unit = CreateHeroUnit("soldier", 102, GetRandomPosition(born_point2), 2);
//		base_unit.robot_base_ai = new RobotBaseAI(base_unit, born_point1);
//
//		base_unit = CreateHeroUnit("soldier", 103, GetRandomPosition(born_point2), 2);
//		base_unit.robot_base_ai = new RobotBaseAI(base_unit, born_point1);
//
//		base_unit = CreateHeroUnit("tank", 104, GetRandomPosition(born_point2), 2);
//		base_unit.robot_base_ai = new RobotBaseAI(base_unit, born_point1);
//
//		base_unit = CreateHeroUnit("tank", 105, GetRandomPosition(born_point2), 2);
//		base_unit.robot_base_ai = new RobotBaseAI(base_unit, born_point1);
//
//		base_unit = CreateHeroUnit("tank", 106, GetRandomPosition(born_point2), 2);
//		base_unit.robot_base_ai = new RobotBaseAI(base_unit, born_point1);
//
//		base_unit = CreateHeroUnit("b2", 107, GetRandomPosition(born_point2), 2);
//		base_unit.robot_base_ai = new RobotBaseAI(base_unit, born_point1);
//
//		base_unit = CreateHeroUnit("b2", 108, GetRandomPosition(born_point2), 2);
//		base_unit.robot_base_ai = new RobotBaseAI(base_unit, born_point1);
//
//		base_unit = CreateHeroUnit("f15", 109, GetRandomPosition(born_point2), 2);
//		base_unit.robot_base_ai = new RobotBaseAI(base_unit, born_point1);
//
//		base_unit = CreateHeroUnit("f15", 110, GetRandomPosition(born_point2), 2);
//		base_unit.robot_base_ai = new RobotBaseAI(base_unit, born_point1);
//
//		CreateBuildingUnit("base", 151, born_point2, 2);
//
//		Vector3 born_point3 = new Vector3(18, 0, 9);
//
//		CreateHeroUnit("soldier", 111, GetRandomPosition(born_point3), 2);
//		CreateHeroUnit("soldier", 112, GetRandomPosition(born_point3), 2);
//		CreateHeroUnit("tank", 113, GetRandomPosition(born_point3), 2);
//
//		Vector3 born_point4 = new Vector3(22, 0, 37);
//
//		CreateHeroUnit("soldier", 114, GetRandomPosition(born_point4), 2);
//		CreateHeroUnit("soldier", 115, GetRandomPosition(born_point4), 2);
//		CreateHeroUnit("tank", 116, GetRandomPosition(born_point4), 2);
//
//		Vector3 born_point5 = new Vector3(30, 0, 21);
//
//		CreateHeroUnit("soldier", 117, GetRandomPosition(born_point5), 2);
//		CreateHeroUnit("soldier", 118, GetRandomPosition(born_point5), 2);
//		CreateHeroUnit("tank", 119, GetRandomPosition(born_point5), 2);

	}
}
