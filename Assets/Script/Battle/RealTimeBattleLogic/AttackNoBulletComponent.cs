using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackNoBulletComponent  : AttackComponentBase
{
	// debug gizmos
	private LineRenderer	line_renderer;

	public override void RenderGizmos()
	{
		if(target_unit != null && my_unit.IsAlive())
		{
			line_renderer.enabled = true;

			Vector3 dir = target_unit._position - my_unit._position;
			dir.Normalize();

			Vector3 right_dir = Vector3.Cross(Vector3.up, dir);
			line_renderer.SetPosition(0, my_unit._position + right_dir * 0.1f);
			line_renderer.SetPosition(1, target_unit._position + right_dir * 0.1f);

		}
		else
		{
			line_renderer.enabled = false;
		}
	}

	public override void InitDebugGizmos()
	{
		line_renderer = my_unit.gameObject.GetOrAddComponent<LineRenderer>();
		line_renderer.material = MaterialManager.Instance().GetMaterial("mat_line");

		Color line_color = HeroUnit.team_color[my_unit.GetTeamID() - 1];
		line_renderer.startColor = line_color;
		line_renderer.endColor = line_color;
		line_renderer.positionCount = 2;
		line_renderer.startWidth = 0.05f;
		line_renderer.endWidth = 0.05f;
	}

	public override void DoAttack(BaseUnit attack_target_unit)
	{
		if(attack_target_unit != target_unit)
		{
			target_unit = attack_target_unit;	
		}

		if(attack_cool_down <= 0)
		{
			my_unit.PlayAttack(attack_target_unit);

			attack_target_unit.OnDamage(my_unit.unit_attack);

			attack_cool_down = my_unit.attack_speed;
		}
	}
}
