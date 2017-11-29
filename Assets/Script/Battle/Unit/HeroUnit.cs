using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroUnit : BaseUnit 
{
	public Animator 			animator;

	// 调试相关辅助线
	public CircleRenderer _attack_range_circle;
	[HideInInspector]
	public CircleRenderer attack_range_circle
	{
		get
		{
			if(_attack_range_circle == null )
			{
				GameObject attack_range_obj = new GameObject("AttackRangeCircle");

				_attack_range_circle = attack_range_obj.GetOrAddComponent<CircleRenderer>();
				_attack_range_circle.Init(MaterialManager.Instance().GetMaterial("mat_line"));

				_attack_range_circle.SetColor(new Color(1, 0, 0, 0.2f));
				attack_range_obj.transform.SetParent(cache_transform, false);
			}

			return _attack_range_circle;
		}
	}

	// 射程，视野
	private float 			attack_range_ = 2;
	public float			attack_range 
	{
		set
		{
			attack_range_ = value;
			UpdateAttackDebugGizmos();
		}

		get
		{
			return attack_range_;
		}
	}

	public BL.BLUnitHero	bl_unit_info;
	public bool				is_move { set; get; }

	public override void OnInit ()
	{
		bl_unit_info = BL.BLUnitManager.Instance().GetUnit(unit_id) as BL.BLUnitHero;

		base.OnInit ();

		attack_range_circle.gameObject.SetActive(false);
	}

	public override void OnClear ()
	{
		base.OnClear ();

		bl_unit_info = null;
	}

	protected override void OnPositionChanged ()
	{
		base.OnPositionChanged ();

		UpdateAttackDebugGizmos();
	}

	private void UpdateAttackDebugGizmos()
	{
		attack_range_circle.SetCircle(position, attack_range_);
	}

	public override void Tick (float delta_time)
	{
		base.Tick (delta_time);

		if(is_move && bl_unit_info != null)
		{
			float current_time_span = (BL.BLTimelineController.Instance().current_logic_frame_time_stamp - BL.BLTimelineController.Instance().pre_logic_frame_time_stamp);

			if(current_time_span == 0)
			{
				return;
			}

			//float current_elapsed = (BL.BLTimelineController.Instance().current_time_stamp - BL.BLTimelineController.Instance().pre_logic_frame_time_stamp);
			float current_elapsed = BL.BLTimelineController.Instance().time_elapsed_from_pre_frame;


			Vector3 pre_position = bl_unit_info.pre_position.Vector3Value();
			Vector3 next_position = bl_unit_info.position.Vector3Value();

			// Debug.Log("current elapsed " + current_elapsed + " current time span " + current_time_span);

			Vector3 now_position = Vector3.Lerp(pre_position, next_position, current_elapsed / current_time_span);

			position = now_position;

			if(bl_unit_info.dir != Vector3.zero)
			{
				SetDirection(bl_unit_info.dir);	
			}
		}
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
			attack_range_circle.gameObject.SetActive(true);
			attack_range_circle.SetColor(new Color(1, 0, 0, 1));

		}
		else
		{
			attack_range_circle.gameObject.SetActive(false);
			attack_range_circle.SetColor(new Color(1, 0, 0, 0.2f));
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

	public void PlayAttack(BaseUnit enemy_unit)
	{
		animator.SetTrigger("Attack1Trigger");

		AddEffect(fire_node, "fire_effect");

		SetDirection(enemy_unit.position - position);
	}
}
