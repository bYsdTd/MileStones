using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseUnit : MonoBehaviour {

	// prefab 上相关的信息
	public Transform fire_node;
	public Transform hited_node;
	public Transform mesh_node;

	// 调试相关辅助线
	[HideInInspector]
	public CircleRenderer vision_range_circle;

	// 单位属性
	[HideInInspector]
	public int 				unit_id = -1;
	[HideInInspector]
	public Vector3 			_position;
	[HideInInspector]
	public string 			resource_key;
	protected int 			_team_id = -1;
	[HideInInspector]
	public int 				unit_hp = 100;
	[HideInInspector]
	public UnitType			unit_type = UnitType.None;
	[HideInInspector]
	public string			unit_name = "";

	// 视野
	private float		 	_attack_vision = 3;
	protected float 		attack_vision_square = 1;

	private	bool			_is_selected = false;
	public bool 			is_selected
	{
		set
		{
			if(_is_selected != value)
			{
				_is_selected = value;

				OnSelectedChanged();
			}	
		}

		get
		{
			return _is_selected;
		}
	}

	virtual public void OnInit()
	{
		mesh_node.gameObject.SetActive(true);
		gameObject.SetActive(true);
	}

	virtual public void OnClear()
	{
		gameObject.SetActive(false);

		if(cache_select_effect != null)
		{
			ObjectPoolManager.Instance().ReturnObject("UnitSelectCircle", cache_select_effect);

			cache_select_effect = null;	
		}
			
	}

	// 缓存transform组件
	public Transform 		cache_transform 
	{
		get
		{
			if(_cache_transform == null)
			{
				_cache_transform = gameObject.transform;	
			}

			return _cache_transform;
		}
	}
	private Transform		_cache_transform;

	// 选中特效
	protected GameObject	cache_select_effect;

	public void SetTeamID(int team_id)
	{
		if(_team_id != team_id)
		{
			_team_id = team_id;

			//			SkinnedMeshRenderer[] skin_mesh_renderer = GetComponentsInChildren<SkinnedMeshRenderer>();
			//
			//			for(int i = 0; i < skin_mesh_renderer.Length; ++i)
			//			{
			//				skin_mesh_renderer[i].material.SetColor("_Color", team_color[_team_id-1]);
			//			}
			//
			//			MeshRenderer[] mesh_renderer = GetComponentsInChildren<MeshRenderer>();
			//
			//			for(int i = 0; i < mesh_renderer.Length; ++i)
			//			{
			//				mesh_renderer[i].material.SetColor("_Color", team_color[_team_id-1]);
			//			}
		}
	}

	public int GetTeamID()
	{
		return _team_id;	
	}

	public void SetAttackVision(float attack_vision)
	{
		_attack_vision = attack_vision;

		attack_vision_square = _attack_vision * _attack_vision;

		UpdateVisionDebugGizmos();
	}

	private void UpdateVisionDebugGizmos()
	{
		if(vision_range_circle == null)
		{
			GameObject vision_range_obj = new GameObject("VisisonRangeCircle");
			vision_range_circle = vision_range_obj.GetOrAddComponent<CircleRenderer>();
			vision_range_circle.Init(MaterialManager.Instance().GetMaterial("mat_line"));

			vision_range_circle.SetColor(new Color(1, 1, 0, 0.2f));
			vision_range_obj.transform.SetParent(cache_transform, false);
		}

		vision_range_circle.SetCircle(_position, _attack_vision);			
	}

	public void SetPosition(Vector3 position)
	{
		if(position != _position)
		{
			_position = position;

			cache_transform.position = position;

			UpdateVisionDebugGizmos();

			OnPositionChanged();
		}
	}

	virtual protected void OnPositionChanged()
	{
		
	}
	virtual public void Tick(float delta_time)
	{
		
	}

	public bool IsAlive()
	{
		return unit_hp > 0;	
	}

	virtual public void OnDead()
	{
		
	}

	public void OnDamage(int damage)
	{
		unit_hp -= damage;

		if(unit_hp <= 0)
		{
			OnDead();

			EventManager.Instance().PostEvent(EventConfig.EVENT_UNIT_DEAD, new object[]{this});
			PlayDead();	
		}
		else
		{
			PlayHited();
		}

	}

	public void PlayDead()
	{
		string dead_effect_name = "hit_effect2";

		if(unit_type == UnitType.Hero)
		{
			dead_effect_name = "hit_effect2";
		}
		else if(unit_type == UnitType.Building)
		{
			dead_effect_name = "hit_effect3";
		}

		AddEffect(hited_node, dead_effect_name, delegate() {

			UnitManager.Instance().DestroyUnit(resource_key, unit_id);

		});

		mesh_node.gameObject.SetActive(false);
	}

	// 击中效果
	public void PlayHited()
	{
		AddEffect(hited_node, "hit_effect1");
	}

	virtual public void OnSelectedChanged()
	{
		if(is_selected)
		{
			vision_range_circle.SetColor(new Color(1, 1, 0, 1));


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

			ObjectPoolManager.Instance().ReturnObject("UnitSelectCircle", cache_select_effect);

			cache_select_effect = null;
		}
	}

	// 只考虑自己
	public bool IsCanSeeUnitCheckOnlyMyself(BaseUnit enemy_unit)
	{
		float distance_square = (enemy_unit._position - _position).sqrMagnitude;

		return attack_vision_square >= distance_square;
	}

	// 考虑共享视野
	public bool IsCanSeeUnit(BaseUnit enemy_unit)
	{
		HashSet<BaseUnit> vision_enemy_units = BattleField.battle_field.real_time_battle_logic.battle_vision_control.vision_enemy_units[GetTeamID()];

		return vision_enemy_units.Contains(enemy_unit);
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
}
