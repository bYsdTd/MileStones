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

		public void Tick(int delta_frame)
		{
			
		}
	}	
}
