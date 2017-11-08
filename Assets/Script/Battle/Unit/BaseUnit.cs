using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseUnit : MonoBehaviour {

	private FoW.FogOfWarUnit		fow_unit;

	// prefab 上相关的信息
	public Transform fire_node;
	public Transform hited_node;
	public Transform mesh_node;
	public Transform blood_hud_node;

	// 调试相关辅助线
	private CircleRenderer _vision_range_circle;
	[HideInInspector]
	public CircleRenderer vision_range_circle 
	{
		get
		{
			if(_vision_range_circle == null)
			{
				GameObject vision_range_obj = new GameObject("VisisonRangeCircle");
				_vision_range_circle = vision_range_obj.GetOrAddComponent<CircleRenderer>();
				_vision_range_circle.Init(MaterialManager.Instance().GetMaterial("mat_line"));

				_vision_range_circle.SetColor(new Color(1, 1, 0, 0.2f));
				vision_range_obj.transform.SetParent(cache_transform, false);
			}

			return _vision_range_circle;
		}
	}

	// 血条渲染
	[HideInInspector]
	private UIProgressBar	blood_progress_;
	[HideInInspector]
	private GameObject		blood_hud_obj;

	// 单位属性
	[HideInInspector]
	public int 				unit_id = -1;
	[HideInInspector]
	public Vector3 			position
	{
		get
		{
			return position_;
		}

		set
		{
			position_ = value;

			cache_transform.position = position_;

			UpdateVisionDebugGizmos();

			OnPositionChanged();
		}
	}

	private Vector3			position_;

	[HideInInspector]
	public string 			resource_key;
	protected int 			team_id_ = -1;
	[HideInInspector]
	public int 				unit_hp = 100;
	[HideInInspector]
	public int 				max_hp { set; get; }
	[HideInInspector]
	public UnitType			unit_type = UnitType.None;
	[HideInInspector]
	public string			unit_name = "";

	// 视野
	private float		 	_attack_vision = -1;
	public float			attack_vision 
	{
		set
		{
			if(_attack_vision != value)
			{
				_attack_vision = value;

				attack_vision_square = _attack_vision * _attack_vision;

				if(fow_unit != null)
				{
					fow_unit.radius = _attack_vision;
				}

				UpdateVisionDebugGizmos();
			}
		}
		get
		{
			return _attack_vision;
		}
	}

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

	[HideInInspector]
	public bool show_blood_hud { set; get; }

	virtual public void OnInit()
	{
		show_blood_hud = false;

		mesh_node.gameObject.SetActive(true);
		gameObject.SetActive(true);
		vision_range_circle.gameObject.SetActive(false);

		if(BattleField.battle_field.IsMyTeam(GetTeamID()))
		{
			fow_unit = gameObject.GetOrAddComponent<FoW.FogOfWarUnit>();

			fow_unit.team = GetTeamID();
			fow_unit.radius = attack_vision;
		}

		if(blood_hud_obj != null)
		{
			Debug.LogError("初始化的时候已经有hud了，不正常! " + unit_name + " id: " + unit_id);
		}

		blood_hud_obj = ObjectPoolManager.Instance().GetObject("BloodHud");
		blood_hud_obj.transform.SetParent(GUIManager.Instance().cache_root, false);
		blood_hud_obj.transform.localScale = Vector3.one;

		blood_progress_ = blood_hud_obj.GetComponentInChildren<UIProgressBar>();
	}

	virtual public void OnClear()
	{
		blood_progress_ = null;

		ObjectPoolManager.Instance().ReturnObject("BloodHud", blood_hud_obj);
		blood_hud_obj = null;

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
		if(team_id_ != team_id)
		{
			team_id_ = team_id;

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

		if(fow_unit != null)
		{
			fow_unit.team = team_id;
		}
	}

	public int GetTeamID()
	{
		return team_id_;	
	}

	public void SetAttackVision(float attack_vision)
	{
		_attack_vision = attack_vision;

		attack_vision_square = _attack_vision * _attack_vision;

		if(fow_unit != null)
		{
			fow_unit.radius = attack_vision;
		}

		UpdateVisionDebugGizmos();
	}

	private void UpdateVisionDebugGizmos()
	{
		vision_range_circle.SetCircle(position, _attack_vision);			
	}

	virtual protected void OnPositionChanged()
	{
		
	}

	private void UpdateBloodHud()
	{
		if(show_blood_hud)
		{
			blood_hud_obj.SetActive(true);

			Vector3 screen_position = Camera.main.WorldToScreenPoint(blood_hud_node.position);
			screen_position.z = 0;

			blood_hud_obj.transform.localPosition = GUIManager.Instance().ScreenPosToUIPos(screen_position);

			float hp_percent = unit_hp * 1.0f / max_hp;

			blood_progress_.value = hp_percent;

			if(hp_percent < 0.3f)
			{
				blood_progress_.foregroundWidget.color = Color.red;
			}
			else if(hp_percent < 0.5f)
			{
				blood_progress_.foregroundWidget.color = Color.yellow;
			}
			else
			{
				blood_progress_.foregroundWidget.color = Color.green;
			}	
		}
		else
		{
			blood_hud_obj.SetActive(false);
		}
	}

	virtual public void Tick(float delta_time)
	{
		UpdateBloodHud();
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
		// 可能同时收到多个伤害, 但是只结算一次
		if(!IsAlive())
		{
			return;
		}

		show_blood_hud = true;

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
			vision_range_circle.gameObject.SetActive(true);

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
			vision_range_circle.gameObject.SetActive(false);

			vision_range_circle.SetColor(new Color(1, 1, 0, 0.2f));

			ObjectPoolManager.Instance().ReturnObject("UnitSelectCircle", cache_select_effect);

			cache_select_effect = null;
		}
	}

	// 只考虑自己
	public bool IsCanSeeUnitCheckOnlyMyself(BaseUnit enemy_unit)
	{
		float distance_square = (enemy_unit.position - position).sqrMagnitude;

		return attack_vision_square >= distance_square;
	}

	// 考虑共享视野
	public bool IsCanSeeUnit(BaseUnit enemy_unit)
	{
		HashSet<BaseUnit> vision_enemy_units = BattleField.battle_field.real_time_battle_logic.battle_vision_control.vision_enemy_units[GetTeamID()];

		return vision_enemy_units.Contains(enemy_unit);
	}

	public delegate void EffectEndCallBack();

	private GameObject AddEffectImp(string effect_name, EffectEndCallBack effect_end_call_back = null)
	{
		GameObject effect_object = ObjectPoolManager.Instance().GetObject(effect_name);

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

		return effect_object;
	}

	public void AddEffect(Vector3 world_position, string effect_name, EffectEndCallBack effect_end_call_back = null)
	{
		GameObject effect_object = AddEffectImp(effect_name, effect_end_call_back);

		effect_object.transform.SetParent(UnitManager.Instance().cache_root_effect_node, false);
		effect_object.transform.position = world_position;

	}

	public void AddEffect(Transform node,  string effect_name, EffectEndCallBack effect_end_call_back = null)
	{
		GameObject effect_object = AddEffectImp(effect_name, effect_end_call_back);

		effect_object.transform.SetParent(node, false);
	}
}
