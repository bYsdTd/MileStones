using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
	public class AttackNoBulletComponent  : AttackComponentBase
	{
		public override void DoAttack(BLUnitBase attack_target_unit)
		{
			if(attack_target_unit != target_unit)
			{
				target_unit = attack_target_unit;	
			}

			if(attack_cool_down <= 0)
			{
				my_unit.DoAttack(attack_target_unit);

				// 换算成攻击间隔是多少帧
				attack_cool_down = (int)(my_unit.attack_speed / BLTimelineController.MS_PER_FRAME);
			}
		}
	}

}
