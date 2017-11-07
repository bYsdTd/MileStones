using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackComponentBase
{
	public HeroUnit my_unit;
	public BaseUnit target_unit;
	protected float attack_cool_down = 0;

	virtual public void InitDebugGizmos()
	{
		
	}

	public void CalculateCoolDown(float delta_time)
	{
		if(attack_cool_down <= 0)
		{
			attack_cool_down = 0;
		}
		else
		{
			attack_cool_down -= delta_time;
		}
	}

	virtual public void RenderGizmos()
	{
		
	}

	virtual public void AttackImplementTick(float delta_time)
	{
		if(my_unit.is_move_attack || (!my_unit.IsMoveState()))
		{
			if(target_unit == null)
			{
				target_unit = AttackAI.FindCanAttackTarget(my_unit);

				if(target_unit != null)
				{
					DoAttack(target_unit);
				}
			}
			else
			{
				if(target_unit.IsAlive())
				{
					if(my_unit.IsCanAttack(target_unit))
					{
						DoAttack(target_unit);
					}
					else
					{
						// 打不到，清空目标
						target_unit = null;
					}
				}
				else
				{
					target_unit = null;
				}
			}	
		}
	}

	virtual public void DoAttack(BaseUnit attack_target_unit)
	{
	}

}
