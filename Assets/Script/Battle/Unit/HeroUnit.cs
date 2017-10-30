﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroUnit : MonoBehaviour 
{
	public Animator animator;
	public Transform fire_node;
	public Transform hited_node;
	public Material line_render_material;

	[HideInInspector]
	public LineRenderer	line_renderer;

	[HideInInspector]
	public int unit_id = -1;
	// 每秒0.5格
	[HideInInspector]
	private float _move_speed_grid = 0;
	[HideInInspector]
	public float move_speed = 0;
	[HideInInspector]
	public Vector3 _position;
	[HideInInspector]
	public int unit_hp = 100;
	[HideInInspector]
	public int unit_attack = 2;
	[HideInInspector]
	private int _team_id = -1;
	[HideInInspector]
	// 是否追逐状态，玩家操作A敌人的时候，设置成true
	public bool is_pursue_state = false;

	// 浮点数值区域
	[HideInInspector]
	public float attack_speed = 1;

	private float _attack_range = 2;
	private float _attack_vision = 3;

	private Transform cache_transform;

	private CommandBase	current_command = null;

	private GameObject	cache_select_effect;

	private float attack_vision_square = 1;
	private float attack_range_square = 1;

	AttackAI		attack_ai = null;


	// Use this for initialization
	void Start () 
	{
	}
	
	public void InitAfterAttribute()
	{
		cache_transform = gameObject.transform;	

		attack_ai = new AttackAI();
		attack_ai.my_unit = this;

		line_renderer = gameObject.AddComponent<LineRenderer>();
		line_renderer.material = line_render_material;

		Color line_color = HeroUnit.team_color[GetTeamID() - 1];
		line_renderer.SetColors(line_color, line_color);
		line_renderer.SetVertexCount(2);
		line_renderer.SetWidth(0.05f, 0.05f);
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

		if(attack_ai != null)
		{
			attack_ai.Tick(delta_time);
		}
	}

	public static Color[] team_color = new Color[]
	{
		Color.red,
		Color.blue,
		Color.green,
		Color.cyan,
		Color.yellow,
	};

	public void SetTeamID(int team_id)
	{
		if(_team_id != team_id)
		{
			_team_id = team_id;

			SkinnedMeshRenderer[] mesh_renderer = GetComponentsInChildren<SkinnedMeshRenderer>();

			for(int i = 0; i < mesh_renderer.Length; ++i)
			{
				mesh_renderer[i].material.SetColor("_Color", team_color[_team_id-1]);
			}
		}
	}

	public int GetTeamID()
	{
		return _team_id;	
	}

	public void SetMoveSpeedGrid(int move_spped_grid)
	{
		if(_move_speed_grid != move_spped_grid)
		{
			_move_speed_grid = move_spped_grid;

			move_speed = BattleField.battle_field.map_data.map_step * _move_speed_grid;
		}
	}

	public void SetAttackVision(float attack_vision)
	{
		_attack_vision = attack_vision;

		attack_vision_square = _attack_vision * _attack_vision;
	}

	public void SetAttackRange(float attack_range)
	{
		_attack_range = attack_range;
		attack_range_square = _attack_range * _attack_range;
	}

	public bool IsCanSeeUnit(HeroUnit enemy_unit)
	{
		float distance_square = (enemy_unit._position - _position).sqrMagnitude;

		return attack_vision_square >= distance_square;
	}

	public bool IsCanAttack(HeroUnit enemy_unit)
	{
		float distance_square = (enemy_unit._position - _position).sqrMagnitude;

		return attack_range_square >= distance_square;
	}

	public bool IsAlive()
	{
		return unit_hp > 0;	
	}

	public void SetPosition(Vector3 position)
	{
		if(position != _position)
		{
			_position = position;

			if(cache_transform == null)
			{
				cache_transform = gameObject.transform;
			}

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

			cache_select_effect = null;
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

	public void AddEffect(Transform node,  string effect_name)
	{
		GameObject effect_object = ObjectPoolManager.Instance().GetObject(effect_name);

		effect_object.transform.SetParent(node, false);


		ParticleSystem[] all_particles = effect_object.GetComponentsInChildren<ParticleSystem>();
		for(int i = 0; i < all_particles.Length; ++i)
		{
			all_particles[i].Play();
		}

		ParticleEffectConfig particle_system_config = effect_object.GetComponent<ParticleEffectConfig>();

		TimerManager.Instance().DelayCallFunc(delegate(float dt) {

			ParticleSystem[] all_particles_stop = effect_object.GetComponentsInChildren<ParticleSystem>();
			for(int i = 0; i < all_particles.Length; ++i)
			{
				all_particles_stop[i].Stop();
			}

			ObjectPoolManager.Instance().ReturnObject(effect_name, effect_object);

		}, particle_system_config.effect_duration);
	}

	public void PlayAttack(HeroUnit enemy_unit)
	{
		//animator.SetTrigger("Attack1Trigger");

		AddEffect(fire_node, "fire_effect");

		SetDirection(enemy_unit._position - _position);

	}

	// 击中效果
	public void PlayHited()
	{
		AddEffect(hited_node, "hit_effect1");
	}
}
