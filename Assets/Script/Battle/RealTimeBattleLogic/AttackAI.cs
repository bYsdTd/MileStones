using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackAI 
{
	public HeroUnit my_unit;

	// 追击组件
	private PursueTargetComponent pursue_target_component = null;

	// 攻击执行组件 
	private AttackComponentBase attack_component = null;

	// 攻击类型的判定unit1 能否打unit2
	public static bool IsCanAttackByAttackType(BaseUnit unit1, BaseUnit unit2)
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
				HeroUnit cast_unit = unit1 as HeroUnit;
				HeroUnit enemy_unit = unit2 as HeroUnit;

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

	static public BaseUnit FindCanAttackTarget(HeroUnit hero_unit)
	{
		// 找到视野范围内，第一个能攻击到的目标
		if(BattleField.battle_field.real_time_battle_logic.battle_vision_control.vision_enemy_units.ContainsKey(hero_unit.GetTeamID()))
		{
			var vision_enemy_units = BattleField.battle_field.real_time_battle_logic.battle_vision_control.vision_enemy_units[hero_unit.GetTeamID()];

			var enumerator = vision_enemy_units.GetEnumerator();
			while(enumerator.MoveNext())
			{
				BaseUnit enemy_unit = enumerator.Current;
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

	public bool HasPursueTarget()
	{
		if(pursue_target_component != null)
		{
			return pursue_target_component.pursue_target != null;	
		}

		return false;
	}

	public void PursueTargetEndCallback(BaseUnit target)
	{
		attack_component.DoAttack(target);
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
		attack_component.InitDebugGizmos();
	}

	public void InitPursueTargetComponent()
	{
		if(pursue_target_component == null)
		{
			pursue_target_component = new PursueTargetComponent();
			pursue_target_component.my_unit = my_unit;

			pursue_target_component.end_callback = PursueTargetEndCallback;
		}
	}

	public void SetPursueTarget(BaseUnit pursue_target)
	{
		if(pursue_target_component != null)
		{
			pursue_target_component.pursue_target = pursue_target;

			attack_component.target_unit = null;	
		}
	}

	public void Tick(float delta_time)
	{
		attack_component.CalculateCoolDown(delta_time);

		if(pursue_target_component != null && pursue_target_component.pursue_target != null)
		{
			pursue_target_component.Tick(delta_time);
		}
		else
		{
			attack_component.AttackImplementTick(delta_time);
		}

		attack_component.RenderGizmos();

	}

}
