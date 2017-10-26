using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroUnit : MonoBehaviour 
{
	public Animator animator;

	[HideInInspector]
	public int unit_id = -1;
	// 每秒0.5格
	[HideInInspector]
	public float move_speed_grid = 2f;
	[HideInInspector]
	public float move_speed = 0;
	[HideInInspector]
	public Vector3 _position;


	private Transform cache_transform;

	private CommandBase	current_command = null;

	private GameObject	cache_select_effect;

	// Use this for initialization
	void Start () 
	{
	}
	
	public void Init()
	{
		cache_transform = gameObject.transform;	
		move_speed = BattleField.battle_field.map_data.map_step * move_speed_grid;
	}

	// 指令队列
	public void AddCommand(CommandBase new_command)
	{
		if(current_command != null)
		{
			// 如果当前行为没有执行完，应该直接结束，并且重新计算结束帧，比如移动停止的位置
			current_command.end_frame = new_command.start_frame;
			current_command.OnEnd();
		}

		current_command = new_command;
		current_command.OnStart();
	}

	public void Tick(float delta_time)
	{
		if(current_command != null)
		{
			if(current_command.Tick(delta_time))
			{
				// 这条指令执行完了
				current_command = null;
			}
		}
	}

	public void SetPosition(Vector3 position)
	{
		if(position != _position)
		{
			_position = position;

			cache_transform.position = position;	
		}
	}

	public void SetDirection(Vector3 dir)
	{
		Quaternion rotation = Quaternion.LookRotation(dir);
		cache_transform.rotation = rotation;
	}

	public void SetSelected(bool is_selected)
	{
		if(is_selected)
		{
			if(cache_select_effect == null)
			{
				cache_select_effect = ObjectPoolManager.Instance().GetObject("UnitSelectCircle");	
			}

			cache_select_effect.transform.localPosition = new Vector3(0, 0.1f, 0);
			cache_select_effect.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

			cache_select_effect.transform.SetParent( cache_transform, false );

		}
		else
		{
			ObjectPoolManager.Instance().ReturnObject("UnitSelectCircle", cache_select_effect);
		}
	}

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

	public void PlayAttack()
	{
		animator.SetTrigger("Attack1Trigger");
	}
}
