using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroUnit : MonoBehaviour 
{
	public Animator animator;
	public Transform fire_node;
	public Transform hited_node;
	public Transform mesh_node;
	public Material line_render_material;

	[HideInInspector]
	public LineRenderer	line_renderer;

	[HideInInspector]
	public int unit_id = -1;
	// 每秒0.5格
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
	public string resource_key;
	private int _team_id = -1;

	[HideInInspector]
	public bool attack_only_stop_move = false;

	// 浮点数值区域
	[HideInInspector]
	public float attack_speed = 1;
	[HideInInspector]
	public float _pursue_rate = 0.5f;

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

	public void Destroy()
	{
			
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
			if(attack_only_stop_move)
			{
				if(!(current_command != null && current_command._type == CommandType.Move))
				{
					attack_ai.Tick(delta_time);	
				}
			}
			else
			{
				attack_ai.Tick(delta_time);
			}

			attack_ai.DoAttackLineRender();
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

			SkinnedMeshRenderer[] skin_mesh_renderer = GetComponentsInChildren<SkinnedMeshRenderer>();

			for(int i = 0; i < skin_mesh_renderer.Length; ++i)
			{
				skin_mesh_renderer[i].material.SetColor("_Color", team_color[_team_id-1]);
			}

			MeshRenderer[] mesh_renderer = GetComponentsInChildren<MeshRenderer>();

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

	public void SetMoveSpeedGrid(float move_spped_grid)
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

	public void SetPursueTarget(HeroUnit pursue_target)
	{
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
		if(!BattleField.battle_field.IsBlock(grid_x, grid_y))
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

	public delegate void EffectEndCallBack();

	public void AddEffect(Transform node,  string effect_name, EffectEndCallBack effect_end_call_back = null)
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

			if(effect_end_call_back != null)
			{
				effect_end_call_back.Invoke();
			}

		}, particle_system_config.effect_duration);
	}

	public void PlayAttack(HeroUnit enemy_unit)
	{
		animator.SetTrigger("Attack1Trigger");

		AddEffect(fire_node, "fire_effect");

		SetDirection(enemy_unit._position - _position);

	}

	public void OnDamage(int damage)
	{
		unit_hp -= damage;

		if(unit_hp <= 0)
		{
			EventManager.Instance().PostEvent(EventConfig.EVENT_HERO_UNIT_DEAD, new object[]{this});
			PlayDead();	
		}
		else
		{
			PlayHited();
		}

	}

	public void PlayDead()
	{
		AddEffect(hited_node, "hit_effect2", delegate() {

			gameObject.SetActive(false);
			UnitManager.Instance().DestroyUnit(resource_key, unit_id);

		});

		mesh_node.gameObject.SetActive(false);

		line_renderer.enabled = false;
	}

	// 击中效果
	public void PlayHited()
	{
		AddEffect(hited_node, "hit_effect1");
	}
}
