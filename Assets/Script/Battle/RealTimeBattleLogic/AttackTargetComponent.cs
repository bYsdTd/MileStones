using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTargetComponent  
{
	public HeroUnit my_unit;
	public BaseUnit target_unit;

	private float attack_cool_down = 0;
	// debug gizmos
	private LineRenderer	line_renderer;

	public void DoAttackLineRender()
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

	public void AttackImplement()
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

	public void InitDebugGizmos()
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

	public void DoAttack(BaseUnit attack_target_unit)
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
