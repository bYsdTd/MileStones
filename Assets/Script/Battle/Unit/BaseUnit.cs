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

	// 视野
	private float		 	_attack_vision = 3;
	protected float 			attack_vision_square = 1;

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
			vision_range_circle = vision_range_obj.AddComponent<CircleRenderer>();
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

	virtual public void OnDamage(int damage)
	{
		
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
