using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackAI 
{
	public HeroUnit my_unit;
	public HeroUnit target_unit;

	// 追击的目标
	public HeroUnit pursue_target;

	public float attack_cool_down = 0;
	public float pursue_cool_down = 0;

	// debug gizmos
	private LineRenderer	line_renderer;

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

	public void InitDebugGizmos()
	{
		line_renderer = my_unit.gameObject.AddComponent<LineRenderer>();
		line_renderer.material = MaterialManager.Instance().GetMaterial("mat_line");

		Color line_color = HeroUnit.team_color[my_unit.GetTeamID() - 1];
		line_renderer.SetColors(line_color, line_color);
		line_renderer.SetVertexCount(2);
		line_renderer.SetWidth(0.05f, 0.05f);	
	}

	public void Tick(float delta_time)
	{
		if(pursue_target != null)
		{
			if(pursue_target.IsAlive())
			{
				if(my_unit.IsCanAttack(pursue_target))
				{
					target_unit = pursue_target;

					my_unit.Idle();

					DoAttack(delta_time);
				}
				else
				{
					target_unit = null;

					if(my_unit.IsCanSeeUnit(pursue_target))
					{
						DoPursueTarget(delta_time);
					}
					else
					{
						my_unit.SetPursueTarget(null);
					}
				}
			}
			else
			{
				my_unit.SetPursueTarget(null);
				target_unit = null;
			}
		}
		else
		{
			if(my_unit.is_move_attack || (!my_unit.IsMoveState()))
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
					if(target_unit.IsAlive())
					{
						if(my_unit.IsCanAttack(target_unit))
						{
							DoAttack(delta_time);
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
	}

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

	private void DoAttack(float delta_time)
	{
		if(attack_cool_down <= 0)
		{
			my_unit.PlayAttack(target_unit);

			target_unit.OnDamage(my_unit.unit_attack);

			attack_cool_down = my_unit.attack_speed;
		}
		else
		{
			attack_cool_down -= delta_time;
		}
	}
		
	private void DoPursueTarget(float delta_time)
	{
		if(pursue_cool_down <= 0)
		{
			int grid_x;
			int grid_y;

			if(BattleField.battle_field.WorldPositon2Grid(pursue_target._position, out grid_x, out grid_y))
			{
				my_unit.Move(grid_x, grid_y);
			}

			pursue_cool_down = my_unit._pursue_rate;
		}
		else
		{
			pursue_cool_down -= delta_time;
		}
	}
}
