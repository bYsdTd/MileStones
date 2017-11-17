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

		public BLUnitBase GetUnit(int unit_id)
		{
			if(all_unit_list.ContainsKey(unit_id))
			{
				return all_unit_list[unit_id];
			}

			return null;
		}

		public BLUnitHero CreateHeroUnit(string gds_name, int id, BLIntVector3 pos, int team_id)
		{
			GDSKit.unit unit_gds = GDSKit.unit.GetInstance(gds_name);

			BLUnitHero hero_unit = new BLUnitHero();

			// 属性相关设置
			hero_unit.gds_name = gds_name;
			hero_unit.unit_type = UnitType.Hero;
			hero_unit.unit_id = id;
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

			return hero_unit;
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

			while(enumerator.MoveNext())
			{
				BLUnitBase unit = enumerator.Current.Value;
				unit.Tick();
			}
		}

		public BLIntVector3 GetRandomPosition(BLIntVector3 born_point)
		{
			int offset_x = Random.Range(3, 8);
			int offset_y = Random.Range(-3, 3);

			return new BLIntVector3(born_point.x + offset_x * 1000, born_point.y, born_point.z + offset_y * 1000 );
		}

		public void InitUnit()
		{
			BLIntVector3 born_point1 = new BLIntVector3(5000, 0, 30000);

			BLUnitHero hero1 = CreateHeroUnit("soldier", 0, GetRandomPosition(born_point1), 1);
			BLUnitHero hero2 = CreateHeroUnit("soldier", 1, GetRandomPosition(born_point1), 1);

			UnitManager.Instance().CreateHeroUnit(hero1.gds_name, hero1.unit_id, hero1.position.Vector3Value());
			UnitManager.Instance().CreateHeroUnit(hero2.gds_name, hero2.unit_id, hero2.position.Vector3Value());

		}
	}	
}
