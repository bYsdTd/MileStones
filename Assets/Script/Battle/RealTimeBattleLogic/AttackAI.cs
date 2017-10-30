using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackAI 
{
	public HeroUnit my_unit;
	public HeroUnit target_unit;

	private float cool_down = 0;

	private HeroUnit FindCanAttackTarget()
	{
		// 找到视野范围内，第一个能攻击到的目标
		if(BattleField.battle_field.real_time_battle_logic.battle_vision_control.vision_enemy_units.ContainsKey(my_unit.GetTeamID()))
		{
			var vision_enemy_units = BattleField.battle_field.real_time_battle_logic.battle_vision_control.vision_enemy_units[my_unit.GetTeamID()];

			var enumerator = vision_enemy_units.GetEnumerator();
			while(enumerator.MoveNext())
			{
				HeroUnit enemy_unit = enumerator.Current;
				if(my_unit.IsCanAttack(enemy_unit))
				{
					return enemy_unit;
				}
			}	
		}

		return null;
	}

	public void Tick(float delta_time)
	{
		if(target_unit == null)
		{
			target_unit = FindCanAttackTarget();

			if(target_unit != null)
			{
				DoAttack(delta_time);
			}
		}
		else
		{
			if(my_unit.IsCanAttack(target_unit))
			{
				DoAttack(delta_time);
			}
			else
			{
				if(my_unit.is_pursue_state)
				{
					// 向目标移动
				}
				else
				{
					
				}
			}
		}
	}

	private void DoAttack(float delta_time)
	{
		if(cool_down <= 0)
		{
			my_unit.PlayAttack(target_unit);

			target_unit.PlayHited();

			cool_down = my_unit.attack_speed;
		}
		else
		{
			cool_down -= delta_time;
		}
	}
}
