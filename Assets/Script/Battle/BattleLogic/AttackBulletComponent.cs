using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
	public class AttackBulletComponent : AttackComponentBase 
	{
		List<BulletComponent> all_bullets = new List<BulletComponent>();

		// 实际攻击逻辑
		public override void DoAttack(BLUnitBase attack_target_unit)
		{
			if(attack_cool_down <= 0)
			{
				my_unit.DoAttack(attack_target_unit, false);


				BulletComponent bullet = BLBulletManager.Instance().CreateBullet();
				bullet.bullet_speed = my_unit.bullet_speed;
				bullet.Start(my_unit.position, attack_target_unit.position, HitCallBack);

				// 换算成攻击间隔是多少帧
				attack_cool_down = (int)(my_unit.attack_speed / BLTimelineController.MS_PER_FRAME);
			}
		}

		public void HitCallBack(BulletComponent	bullet_componet)
		{
			// 结算伤害
			var all_enemys = BattleVisionControl.Instance().GetEnemys(my_unit.team_id);

			int hit_radius_sqr = my_unit.aoe_radius * my_unit.aoe_radius;

			var enumerator = all_enemys.GetEnumerator();
			while(enumerator.MoveNext())
			{
				BLUnitBase unit = enumerator.Current;

				if(unit.IsAlive() && (unit.position - bullet_componet.end_position).SqrMagnitude() <= hit_radius_sqr)
				{
					unit.OnDamage(my_unit.attack_power);
				}
			}

//			my_unit.AddEffect(bullet_componet.end_position, "hit_effect3");
//

		}
	}

}

