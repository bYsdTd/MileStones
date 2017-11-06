using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroUnit : BaseUnit 
{
	public Animator animator;

	// 调试相关辅助线
	[HideInInspector]
	public CircleRenderer attack_range_circle;

	// 英雄单位相关属性
	// 每秒0.5格
	private float _move_speed_grid = 0;
	[HideInInspector]
	public float move_speed = 0;

	[HideInInspector]
	public int unit_attack = 2;


	[HideInInspector]
	public bool is_move_attack = false;
	[HideInInspector]
	public bool is_fly = false;
	[HideInInspector]
	public bool can_attack_ground = false;
	[HideInInspector]
	public bool can_attack_fly = false;

	// 浮点数值区域
	[HideInInspector]
	public float attack_speed = 1;
	[HideInInspector]
	public float _pursue_rate = 0.5f;

	[HideInInspector]
	public float revive_cd { set; get; }

	// 射程，视野
	private float 			_attack_range = 2;
	private float 			attack_range_square = 1;

	private CommandBase		current_command = null;



	AttackAI				attack_ai = null;
	ReviveAI 				revive_ai = null;

	public void InitAI()
	{
		attack_ai = new AttackAI();
		attack_ai.my_unit = this;
		attack_ai.InitDebugGizmos();

		revive_ai = new ReviveAI();
		revive_ai.my_unit = this;

	}

	protected override void OnPositionChanged ()
	{
		base.OnPositionChanged ();

		UpdateAttackDebugGizmos();
	}

	private void UpdateAttackDebugGizmos()
	{
		if(attack_range_circle == null )
		{
			GameObject attack_range_obj = new GameObject("AttackRangeCircle");

			attack_range_circle = attack_range_obj.GetOrAddComponent<CircleRenderer>();
			attack_range_circle.Init(MaterialManager.Instance().GetMaterial("mat_line"));

			attack_range_circle.SetColor(new Color(1, 0, 0, 0.2f));
			attack_range_obj.transform.SetParent(cache_transform, false);

				
		}

		attack_range_circle.SetCircle(_position, _attack_range);
	}

	// 指令队列
	public void AddCommand(CommandBase new_command)
	{
		if(current_command != null)
		{
			// 如果当前行为没有执行完，应该直接结束，并且重新计算结束帧，比如移动停止的位置
			//current_command.end_frame = new_command.start_frame;
			current_command.OnEnd();
		}

		current_command = new_command;
		current_command.OnStart();
	}

	public override void Tick(float delta_time)
	{
		if(!IsAlive())
		{
			return;	
		}

		if(current_command != null)
		{
			if(current_command.Tick(delta_time))
			{
				current_command.OnEnd();
				// 这条指令执行完了
				current_command = null;
			}
		}

		if(attack_ai != null)
		{

			attack_ai.Tick(delta_time);

			attack_ai.DoAttackLineRender();
		}
	}



	public bool IsMoveState()
	{
		return (current_command != null && current_command._type == CommandType.Move);
	}

	public static Color[] team_color = new Color[]
	{
		Color.red,
		Color.blue,
		Color.green,
		Color.cyan,
		Color.yellow,
	};



	public void SetMoveSpeedGrid(float move_spped_grid)
	{
		if(_move_speed_grid != move_spped_grid)
		{
			_move_speed_grid = move_spped_grid;

			move_speed = BattleField.battle_field.map_data.map_step * _move_speed_grid;
		}
	}



	public void SetAttackRange(float attack_range)
	{
		_attack_range = attack_range;
		attack_range_square = _attack_range * _attack_range;

		UpdateAttackDebugGizmos();
	}

	// 距离要可以打到，并且攻击类型符合
	public bool IsCanAttack(BaseUnit enemy_unit)
	{
		if(!IsCanAttackByAttackType(enemy_unit))
		{
			return false;
		}

		float distance_square = (enemy_unit._position - _position).sqrMagnitude;

		return attack_range_square >= distance_square;
	}

	// 攻击类型决定的是否可攻击
	public bool IsCanAttackByAttackType(BaseUnit enemy_unit)
	{
		return AttackAI.IsCanAttackByAttackType(this, enemy_unit);
	}

	public void SetDirection(Vector3 dir)
	{
		Quaternion rotation = Quaternion.LookRotation(dir);
		cache_transform.rotation = rotation;
	}

	public override void OnSelectedChanged ()
	{
		base.OnSelectedChanged ();

		if(is_selected)
		{
			attack_range_circle.SetColor(new Color(1, 0, 0, 1));

		}
		else
		{
			attack_range_circle.SetColor(new Color(1, 0, 0, 0.2f));
		}
	}

	public void SetSelected(bool is_selected)
	{
		if(is_selected)
		{
			vision_range_circle.SetColor(new Color(1, 1, 0, 1));
			attack_range_circle.SetColor(new Color(1, 0, 0, 1));

			if(cache_select_effect == null)
			{
				cache_select_effect = ObjectPoolManager.Instance().GetObject("UnitSelectCircle");	
			}

			cache_select_effect.transform.localPosition = new Vector3(0, 0.1f, 0);
			cache_select_effect.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

			cache_select_effect.transform.SetParent( cache_transform, false );

		}
		else
		{
			vision_range_circle.SetColor(new Color(1, 1, 0, 0.2f));
			attack_range_circle.SetColor(new Color(1, 0, 0, 0.2f));

			ObjectPoolManager.Instance().ReturnObject("UnitSelectCircle", cache_select_effect);

			cache_select_effect = null;
		}
	}

	public void SetPursueTarget(BaseUnit pursue_target)
	{
		if(pursue_target && !IsCanAttackByAttackType(pursue_target))
		{
			return;
		}

		attack_ai.pursue_target = pursue_target;

		attack_ai.target_unit = null;

		// 这里需要播放一个效果
	}

	public void Idle()
	{
		if(current_command != null)
		{
			current_command.OnEnd();
			current_command = null;
		}

		PlayIdle();
	}

	public void Move(int grid_x, int grid_y)
	{
		if(!BattleField.battle_field.IsBlock(grid_x, grid_y) || is_fly)
		{
			int current_x;
			int current_y;

			BattleField.battle_field.WorldPositon2Grid(_position, out current_x, out current_y);

			CommandMove move_command = new CommandMove();
			move_command.unit_id = unit_id;

			move_command.start_grid_x = current_x;
			move_command.start_grid_y = current_y;

			move_command.end_grid_x = grid_x;
			move_command.end_grid_y = grid_y;

			CommandManager.Instance().DispatchCommand(move_command);
		}
	}

//	public void Move2Target(HeroUnit pursue_target)
//	{
//		CommandMove2Target move2target_command = new CommandMove2Target();
//		move2target_command.unit_id = unit_id;
//		move2target_command.pursue_target = pursue_target;
//
//		CommandManager.Instance().DispatchCommand(move2target_command);
//	}

	public void PlayMove()
	{
		animator.SetBool("Moving", true);
		animator.SetBool("Running", true);
	}

	public void PlayIdle()
	{
		animator.SetBool("Moving", false);
		animator.SetBool("Running", false);
	}

	public void PlayAttack(BaseUnit enemy_unit)
	{
		animator.SetTrigger("Attack1Trigger");

		AddEffect(fire_node, "fire_effect");

		SetDirection(enemy_unit._position - _position);

	}

	public override void OnDead ()
	{
		BuildingUnit revive_building = revive_ai.FindNeareatReviveBuilding();

		if(revive_building != null)
		{
			revive_building.AddReviveUnit(unit_name, unit_id, GetTeamID());
		}
		else
		{
			Debug.LogError("没有找到复活的建筑 , name " + unit_name + " id: " + unit_id);
		}
	}
}
