using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
	public class PursueTargetComponent 
	{
		// 追击的目标
		public BLUnitBase pursue_target;
		public BLUnitHero my_unit;

		private float pursue_cool_down = 0;

		public delegate void PursueEndCallBack(BLUnitBase target);

		public PursueEndCallBack end_callback = null;

		public void Tick(float delta_time)
		{
			if(pursue_target != null)
			{
				if(pursue_target.IsAlive())
				{
					if(my_unit.IsCanAttack(pursue_target))
					{
						//my_unit.Idle();

						if(end_callback != null)
						{
							end_callback.Invoke(pursue_target);
						}
					}
					else
					{
						if(my_unit.IsCanSeeUnit(pursue_target))
						{
							DoPursueTarget(delta_time);
						}
						else
						{
							//my_unit.SetPursueTarget(null);
						}
					}
				}
				else
				{
					//my_unit.SetPursueTarget(null);
				}
			}
		}

		private void DoPursueTarget(float delta_time)
		{
//			if(pursue_cool_down <= 0)
//			{
//				int grid_x;
//				int grid_y;
//
//				if(BattleField.battle_field.WorldPositon2Grid(pursue_target.position, out grid_x, out grid_y))
//				{
//					my_unit.Move(grid_x, grid_y);
//				}
//
//				pursue_cool_down = my_unit._pursue_rate;
//			}
//			else
//			{
//				pursue_cool_down -= delta_time;
//			}
		}
	}
}


