using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BL
{
	public class AttackAI 
	{
		public BLUnitHero my_unit;

//		// 追击组件
//		private PursueTargetComponent pursue_target_component = null;

		// 攻击执行组件 
		private AttackComponentBase attack_component = null;

		// 攻击类型的判定unit1 能否打unit2
		public static bool IsCanAttackByAttackType(BLUnitBase unit1, BLUnitBase unit2)
		{
			// 暂时没有能攻击的建筑
			if(unit1.unit_type == UnitType.Building)
			{
				return false;
			}
			else if(unit1.unit_type == UnitType.Hero)
			{
				if(unit2.unit_type == UnitType.Building)
				{
					return true;
				}
				else if(unit2.unit_type == UnitType.Hero)
				{
					BLUnitHero cast_unit = unit1 as BLUnitHero;
					BLUnitHero enemy_unit = unit2 as BLUnitHero;

					bool can_attack = false;

					if((enemy_unit.is_fly && cast_unit.can_attack_fly) || (!enemy_unit.is_fly) && cast_unit.can_attack_ground )
					{
						can_attack = true;	
					}

					return can_attack;
				}
			}

			return false;
		}

		static public BLUnitBase FindCanAttackTarget(BLUnitHero hero_unit)
		{
			// 找到视野范围内，第一个能攻击到的目标
			var enemys = BattleVisionControl.Instance().GetEnemys(hero_unit.team_id);
			if(enemys != null)
			{
				var enumerator = enemys.GetEnumerator();
				while(enumerator.MoveNext())
				{
					BLUnitBase enemy_unit = enumerator.Current;
					if(hero_unit.IsCanAttack(enemy_unit))
					{
						return enemy_unit;
					}
				}	
			}

			return null;
		}

		public bool HasAttackTarget()
		{
			if(attack_component != null)
			{
				return attack_component.target_unit != null;	
			}

			return false;
		}

		public void InitAttackComponent()
		{
			if(attack_component == null)
			{
				if(my_unit.bullet_speed == 0)
				{
					// 无弹道
					attack_component = new AttackNoBulletComponent();
				}
				else
				{
					attack_component = new AttackBulletComponent();
				}
			}

			attack_component.my_unit = my_unit;
		}

		public void Tick()
		{
			attack_component.CalculateCoolDown();

			attack_component.AttackImplementTick();

			// 先去掉追击的逻辑
//			if(pursue_target_component != null && pursue_target_component.pursue_target != null)
//			{
//				pursue_target_component.Tick(delta_time);
//			}
//			else
//			{
//				attack_component.AttackImplementTick(delta_time);
//			}

			attack_component.RenderGizmos();
		}

//		public bool HasPursueTarget()
//		{
//			if(pursue_target_component != null)
//			{
//				return pursue_target_component.pursue_target != null;	
//			}
//
//			return false;
//		}

//		public void PursueTargetEndCallback(BaseUnit target)
//		{
//			attack_component.DoAttack(target);
//		}



//		public void InitPursueTargetComponent()
//		{
//			if(pursue_target_component == null)
//			{
//				pursue_target_component = new PursueTargetComponent();
//				pursue_target_component.my_unit = my_unit;
//
//				pursue_target_component.end_callback = PursueTargetEndCallback;
//			}
//		}
//
//		public void SetPursueTarget(BaseUnit pursue_target)
//		{
//			if(pursue_target_component != null)
//			{
//				pursue_target_component.pursue_target = pursue_target;
//
//				attack_component.target_unit = null;	
//			}
//		}



	}

}

