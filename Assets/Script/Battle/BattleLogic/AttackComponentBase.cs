using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
	public class AttackComponentBase
	{
		public BLUnitHero my_unit;
		public BLUnitBase target_unit;
		protected int attack_cool_down = 0;

		public void CalculateCoolDown()
		{
			if(attack_cool_down <= 0)
			{
				attack_cool_down = 0;
			}
			else
			{
				attack_cool_down--;
			}
		}

		virtual public void RenderGizmos()
		{

		}

		virtual public void AttackImplementTick()
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

		virtual public void DoAttack(BLUnitBase attack_target_unit)
		{
		}

	}

}

