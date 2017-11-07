using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBulletComponent : AttackComponentBase 
{
	List<BulletComponent> all_bullets = new List<BulletComponent>();

	// 实际攻击逻辑
	public override void DoAttack(BaseUnit attack_target_unit)
	{
		if(attack_cool_down <= 0)
		{
			my_unit.PlayAttack(attack_target_unit);

			BulletComponent bullet = new BulletComponent();
			bullet.bullet_id = UnitManager.GetUniqueID();
			bullet.bullet_speed = my_unit.bullet_speed;
			bullet.Start(my_unit._position, attack_target_unit._position, HitCallBack);

			all_bullets.Add(bullet);

			attack_cool_down = my_unit.attack_speed;
		}
	}

	public override void AttackImplementTick (float delta_time)
	{
		base.AttackImplementTick (delta_time);

		for(int i = all_bullets.Count-1; i >=0; --i)
		{
			BulletComponent bullet_com = all_bullets[i];

			if(bullet_com.Tick(delta_time))
			{
				all_bullets.RemoveAt(i);
			}
		}
	}

	public void HitCallBack(BulletComponent	bullet_componet)
	{
		// 结算伤害

		my_unit.AddEffect(bullet_componet.end_position, "hit_effect3");

		var all_unit =  UnitManager.Instance().all_unit_list;

		float hit_radius_sqr = my_unit.aoe_radius * my_unit.aoe_radius;

		var enumerator = all_unit.GetEnumerator();
		while(enumerator.MoveNext())
		{
			BaseUnit unit = enumerator.Current.Value;

			if(unit.IsAlive() && unit.GetTeamID() != my_unit.GetTeamID() && (unit._position - bullet_componet.end_position).sqrMagnitude <= hit_radius_sqr)
			{
				unit.OnDamage(my_unit.unit_attack);
			}
		}
	}
}
