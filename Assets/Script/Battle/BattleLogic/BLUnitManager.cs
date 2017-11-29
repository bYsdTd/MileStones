using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
	public class BLUnitManager
	{
		static private BLUnitManager instance = null;

		static public BLUnitManager Instance()
		{
			if(instance == null)
			{
				instance = new BLUnitManager();
			}

			return instance;
		}

		static int _unique_id = 0;
		public static int GetUniqueID()
		{
			return ++_unique_id;
		}

		private Dictionary<int, BLUnitHero>			hero_unit_list = new Dictionary<int, BLUnitHero>();
		private Dictionary<int, BLUnitBuilding> 	buiding_unit_list = new Dictionary<int, BLUnitBuilding>();
		private Dictionary<int, BLUnitBase> 		all_unit_list = new Dictionary<int, BLUnitBase>();

		public Dictionary<int, BLUnitBuilding> GetBuildingList()
		{
			return 	buiding_unit_list;
		}

		public BLUnitBase GetUnit(int unit_id)
		{
			if(all_unit_list.ContainsKey(unit_id))
			{
				return all_unit_list[unit_id];
			}

			return null;
		}

		public Dictionary<int, BLUnitBase>	GetAllUnitList()
		{
			return all_unit_list;	
		}

		public BLUnitHero CreateHeroUnit(int unit_id, string gds_name, BLIntVector3 pos, int team_id)
		{
			
			GDSKit.unit unit_gds = GDSKit.unit.GetInstance(gds_name);

			BLUnitHero hero_unit = new BLUnitHero();

			// 属性相关设置
			hero_unit.gds_name = gds_name;
			hero_unit.unit_type = UnitType.Hero;
			hero_unit.unit_id = unit_id;
			hero_unit.revive_cool_down = unit_gds.revive_cd;
			hero_unit.move_speed = unit_gds.move_speed;
			hero_unit.attack_range = unit_gds.attack_range;
			hero_unit.vision = unit_gds.attack_vision;
			hero_unit.attack_speed = unit_gds.attack_speed;
			hero_unit.attack_power = unit_gds.unit_attack;
			hero_unit.hp = unit_gds.unit_hp;
			hero_unit.max_hp = unit_gds.unit_hp;

			hero_unit.is_move_attack = unit_gds.is_move_attack;
			hero_unit.is_fly = unit_gds.is_fly;
			hero_unit.can_attack_fly = unit_gds.can_attack_fly;
			hero_unit.can_attack_ground = unit_gds.can_attack_ground;
			hero_unit.can_pursue = unit_gds.can_pursue;
			hero_unit.aoe_radius = unit_gds.aoe_radius;
			hero_unit.bullet_speed = unit_gds.bullet_speed;

			hero_unit.position = pos;
			hero_unit.team_id = team_id;

			if(all_unit_list.ContainsKey(hero_unit.unit_id))
			{
				Debug.LogError("相同名字的unit已经在管理器里了 id : " + hero_unit.unit_id);
				return null;
			}

			all_unit_list.Add(hero_unit.unit_id, hero_unit);
			hero_unit_list.Add(hero_unit.unit_id, hero_unit);

			hero_unit.OnInit();

			// 表现层
			HeroUnit unit_renderer = UnitManager.Instance().CreateHeroUnit(hero_unit.gds_name, hero_unit.unit_id, hero_unit.position.Vector3Value());
			// 表现层需要显示迷雾，攻击范围，所以需要这些数据
			unit_renderer.attack_vision = hero_unit.vision * 0.001f;
			unit_renderer.team_id = hero_unit.team_id;
			unit_renderer.attack_range = (hero_unit.attack_range * 0.001f);

			unit_renderer.OnInit();

			return hero_unit;
		}

		public BLUnitBuilding CreateBuildingUnit(int unit_id, string gds_name, BLIntVector3 pos, int team_id)
		{
			GDSKit.building unit_gds = GDSKit.building.GetInstance(gds_name);

			BLUnitBuilding building_unit = new BLUnitBuilding();

			// 属性相关设置
			building_unit.gds_name = gds_name;
			building_unit.unit_type = UnitType.Building;
			building_unit.unit_id = unit_id;
			building_unit.vision = unit_gds.vision;
			building_unit.hp = unit_gds.building_hp;
			building_unit.max_hp = unit_gds.building_hp;
			building_unit.can_revive_hero = unit_gds.can_revive_hero;

			building_unit.position = pos;

			building_unit.team_id = team_id;


			if(all_unit_list.ContainsKey(building_unit.unit_id))
			{
				Debug.LogError("相同名字的unit已经在管理器里了 id : " + building_unit.unit_id);
				return null;
			}

			all_unit_list.Add(building_unit.unit_id, building_unit);
			buiding_unit_list.Add(building_unit.unit_id, building_unit);

			building_unit.OnInit();

			// 表现层
			BuildingUnit unit_renderer = UnitManager.Instance().CreateBuildingUnit(building_unit.gds_name, building_unit.unit_id, building_unit.position.Vector3Value());
			unit_renderer.attack_vision = building_unit.vision * 0.001f;
			unit_renderer.team_id = team_id;

			unit_renderer.OnInit();

			return building_unit;
		}

		public void DestroyUnit(int id)
		{
			if(!all_unit_list.ContainsKey(id))
			{
				Debug.LogError("要删除的unit不在管理器里 id : " + id);
				return;
			}

			BLUnitBase unit = all_unit_list[id];

			unit.OnDestroy();

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

		public void Tick()
		{
			var enumerator = all_unit_list.GetEnumerator();

			List<int> remove_keys = new List<int>();

			while(enumerator.MoveNext())
			{
				BLUnitBase unit = enumerator.Current.Value;

				if(unit.IsAlive())
				{
					unit.Tick();	
				}

				if(unit.mark_delete)
				{
					remove_keys.Add(enumerator.Current.Key);
				}
			}

			for(int i = 0; i < remove_keys.Count; ++i)
			{
				DestroyUnit(remove_keys[i]);
			}
		}

		public BLIntVector3 GetRandomPosition(BLIntVector3 born_point)
		{
			int offset_x = Random.Range(1, 1);
			int offset_y = Random.Range(-1, 1);

			return new BLIntVector3(born_point.x + offset_x * 1000, born_point.y, born_point.z + offset_y * 1000 );
		}

		public int GetBaseID(int player_id)
		{
			return 1000 + player_id;
		}

		public int GetHeroUnitID(int player_id, int hero_index)
		{
			return 1000 + player_id * 100 + hero_index;
		}

		public void InitBase()
		{
			BLIntVector3 born_point1 = BattleField.battle_field.born_point[1];
			BLIntVector3 born_point2 = BattleField.battle_field.born_point[2];

//			BLUnitHero hero1 = CreateHeroUnit("rocket_car", GetRandomPosition(born_point1), 0);
//			BLUnitHero hero2 = CreateHeroUnit("rocket_car", GetRandomPosition(born_point2), 1);

			CreateBuildingUnit(GetBaseID(1), "base", born_point1, 1);
			CreateBuildingUnit(GetBaseID(2), "base", born_point2, 2);
		}
	}	
}
