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

	private GameObject		blood_hud_obj_;
	private GameObject		blood_hud_obj 
	{
		get
		{
			if(blood_hud_obj_ == null)
			{
				blood_hud_obj_ = ObjectPoolManager.Instance().GetObject("BloodHud");
				blood_hud_obj_.transform.SetParent(GUIManager.Instance().cache_root, false);
				blood_hud_obj_.transform.localScale = Vector3.one;
				blood_hud_obj_.SetActive(true);
			}

			return blood_hud_obj_;
		}

		set
		{
			blood_hud_obj_ = value;
		}
	}

	private GameObject		enemy_symbol_;
	private GameObject		enemy_symbol
	{
		get
		{
			if(enemy_symbol_ == null)
			{
				enemy_symbol_ = blood_hud_obj.transform.Find("EnemySymbol").gameObject;
			}
			return enemy_symbol_;
		}

		set
		{
			enemy_symbol_ = value;
		}
	}

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

	[HideInInspector]
	public int				team_id 
	{
		set
		{
			team_id_ = value;

			UpdateTeam();
		}
		get
		{
			return team_id_;
		}
	}
	protected int 			team_id_ = -1;

	[HideInInspector]
	public int 				unit_hp 
	{
		get
		{
			if(bl_unit_info_base != null)
			{
				return bl_unit_info_base.hp;
			}

			return 0;
		}
	}

	[HideInInspector]
	public int 				max_hp 
	{
		get
		{
			if(bl_unit_info_base != null)
			{
				return bl_unit_info_base.max_hp;
			}

			return 100;
		}
	}

	[HideInInspector]
	public UnitType			unit_type = UnitType.None;
	[HideInInspector]
	public string			unit_name = "";

	// 视野
	private float		 	_attack_vision = -1;
	[HideInInspector]
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
	[HideInInspector]
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
		
	private bool show_blood_hud_;
	[HideInInspector]
	public bool show_blood_hud 
	{ 
		set
		{
			show_blood_hud_ = value;
			show_blood_time = 5;
		}
		get
		{
			return show_blood_hud_;
		}
	}
		
	private BL.BLUnitBase	bl_unit_info_base { set; get; }

	private float show_blood_time = 5;

	public void UpdateTeam()
	{
		if(BattleField.battle_field.IsMyTeam(team_id))
		{
			fow_unit = gameObject.GetOrAddComponent<FoW.FogOfWarUnit>();

			fow_unit.team = team_id;

			fow_unit.radius = attack_vision;
		}	

		if(enemy_symbol != null)
		{
			enemy_symbol.SetActive(!BattleField.battle_field.IsMyTeam(team_id));	
		}
	}

	virtual public void OnInit()
	{
		show_blood_hud = false;

		mesh_node.gameObject.SetActive(true);
		gameObject.SetActive(true);
		vision_range_circle.gameObject.SetActive(false);
			
		blood_progress_ = blood_hud_obj.GetComponentInChildren<UIProgressBar>();

		bl_unit_info_base = BL.BLUnitManager.Instance().GetUnit(unit_id);

		UpdateTeam();
	}

	virtual public void OnClear()
	{
		enemy_symbol = null;
		blood_progress_ = null;
		bl_unit_info_base = null;

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

	private void UpdateBloodHud(float delta_time)
	{
		if(blood_progress_ == null)
		{
			return;
		}

		Vector3 screen_position = Camera.main.WorldToScreenPoint(blood_hud_node.position);
		screen_position.z = 0;

		blood_hud_obj.transform.localPosition = GUIManager.Instance().ScreenPosToUIPos(screen_position);

		if(show_blood_hud)
		{
			show_blood_time -= delta_time;

			if(show_blood_time <= 0)
			{
				blood_progress_.gameObject.SetActive(false);
				show_blood_hud = false;
			}
			else
			{
				blood_progress_.gameObject.SetActive(true);

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

		}
		else
		{
			if(blood_progress_ == null)
			{
				Debug.Break();
			}

			blood_progress_.gameObject.SetActive(false);
		}
	}

	virtual public void Tick(float delta_time)
	{
		if(!IsAlive())
		{
			return;	
		}

		UpdateBloodHud(delta_time);
	}

	public bool IsAlive()
	{
		return unit_hp > 0;	
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
		show_blood_hud = true;

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
