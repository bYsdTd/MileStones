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
	public CircleRenderer attack_range_circle;
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
}
