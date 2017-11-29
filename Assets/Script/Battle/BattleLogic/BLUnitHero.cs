using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
	public class BLUnitHero : BLUnitBase
	{
		private int		attack_range_;
		private int		attack_range_sqr_;

		public int 		attack_range 
		{ 
			set
			{
				attack_range_ = value;

				attack_range_sqr_ = attack_range_ * attack_range_;
			}

			get
			{
				return attack_range_;
			}
		}


		public int		attack_power { set; get; }
		public int		attack_speed { set; get; }

		public bool		is_move_attack { set; get; }
		public bool 	can_attack_ground { set; get; }
		public bool 	can_attack_fly { set; get; }

		public int 		aoe_radius { set; get; }
		public int 		bullet_speed { set; get; }

		public int		pursue_rate { set; get; }
		public bool 	can_pursue { set; get; }

		public int 		move_speed { set; get; }
		public bool		is_fly { set; get; }

		public int		revive_cool_down { set; get; }

		AttackAI		attack_ai = null;

		public override void OnInit ()
		{
			base.OnInit ();

			InitAI();
		}

		// 距离要可以打到，并且攻击类型符合
		public bool IsCanAttack(BLUnitBase enemy_unit)
		{
			if(!IsCanAttackByAttackType(enemy_unit))
			{
				return false;
			}

			BLIntVector3 distance = (enemy_unit.position - position);

			float distance_square = distance.SqrMagnitude();

			return attack_range_sqr_ >= distance_square;
		}

		// 攻击类型决定的是否可攻击
		public bool IsCanAttackByAttackType(BLUnitBase enemy_unit)
		{
			return AttackAI.IsCanAttackByAttackType(this, enemy_unit);
		}

		public bool IsHaveAttackTarget()
		{
			if(attack_ai != null)
			{
				return attack_ai.HasAttackTarget();
			}

			return false;
		}

//		public bool IsHavePursueTarget()
//		{
//			if(attack_ai != null)
//			{
//				return attack_ai.HasPursueTarget();
//			}
//
//			return false;
//		}

		public bool IsMoveState()
		{
			return (current_command_ != null && current_command_.command_type == TickCommandType.Move);
		}

		public void InitAI()
		{
			attack_ai = new AttackAI();
			attack_ai.my_unit = this;

			// 攻击类型也有不同的组件, 有弹道，没弹道等
			attack_ai.InitAttackComponent();

//			// 根据类型增加是否有追击的组件
//			if(can_pursue)
//			{
//				attack_ai.InitPursueTargetComponent();	
//			}
//
//
//
//			revive_ai = new ReviveAI();
//			revive_ai.my_unit = this;

		}
			
		public void DoAttack(BLUnitBase target_unit, bool is_direct_damage = true)
		{
			// 发送事件到表现层
			EventManager.Instance().PostEvent(EventConfig.EVENT_L2R_PLAY_ATTACK, new object[]{ unit_id, target_unit.unit_id });

			if(is_direct_damage)
			{
				// 计算伤害
				target_unit.OnDamage(attack_power);	
			}
		}

		public override void Tick ()
		{
			base.Tick ();

			if(attack_ai != null)
			{
				attack_ai.Tick();
			}
		}

		public BLUnitBuilding FindNeareatReviveBuilding()
		{
			var building_list = BLUnitManager.Instance().GetBuildingList();
	
			float distance = float.MaxValue;
	
			BLUnitBuilding revive_building = null;
	
			var enumerator = building_list.GetEnumerator();
			while(enumerator.MoveNext())
			{
				BLUnitBuilding building_unit = enumerator.Current.Value;
	
				if(building_unit.team_id == team_id && building_unit.can_revive_hero)
				{
					float temp = (building_unit.position - position).SqrMagnitude();
	
					if(temp < distance)
					{
						revive_building = building_unit;
					}
				}
			}
	
			return revive_building;
		}

		public override void OnDead ()
		{
			base.OnDead ();

			// 处理复活的逻辑
			BLUnitBuilding revive_building = FindNeareatReviveBuilding();

			if(revive_building != null)
			{
				int revive_need_frames = (int)(revive_cool_down / BLTimelineController.MS_PER_FRAME);
				revive_building.AddReviveUnit(gds_name, unit_id, revive_need_frames);	
			}
		}
	}	
}
