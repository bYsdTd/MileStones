using UnityEngine;
using System.Collections;

public class BattleFieldInputHandle 
{
	public Transform cache_battle_field_camera = null;

	private	BaseUnit current_select_unit = null;

	int unit_layer_mask;

	private bool is_draging_battle_field = false;

	private Vector2 current_click_down_position;

	private	Plane map_grid_plane = new Plane(Vector3.up, Vector3.zero);

	public void Init()
	{
		cache_battle_field_camera = Camera.main.gameObject.transform;

		RegisterEvent();

		unit_layer_mask = 1 << LayerMask.NameToLayer("Unit");
	}

	public void Destroy()
	{
		EventManager.Instance().UnRegisterEvent(EventConfig.EVENT_SCENE_CLICK_DOWN, OnBattleFieldClickDown);
		EventManager.Instance().UnRegisterEvent(EventConfig.EVENT_SCENE_CLICK_MOVE, OnBattleFieldDragMove);
		EventManager.Instance().UnRegisterEvent(EventConfig.EVENT_SCENE_CLICK_UP, OnBattleFieldClickUp);
		EventManager.Instance().UnRegisterEvent(EventConfig.EVENT_UNIT_DEAD, OnUnitDead);
	}

	public void RegisterEvent()
	{
		EventManager.Instance().RegisterEvent(EventConfig.EVENT_SCENE_CLICK_DOWN, OnBattleFieldClickDown);
		EventManager.Instance().RegisterEvent(EventConfig.EVENT_SCENE_CLICK_MOVE, OnBattleFieldDragMove);
		EventManager.Instance().RegisterEvent(EventConfig.EVENT_SCENE_CLICK_UP, OnBattleFieldClickUp);
		EventManager.Instance().RegisterEvent(EventConfig.EVENT_UNIT_DEAD, OnUnitDead);
	}

	public void OnUnitDead(params System.Object[] all_params)
	{
		BaseUnit unit_dead = (BaseUnit)all_params[0];

		if(unit_dead == current_select_unit)
		{
			current_select_unit.is_selected = false;
			current_select_unit = null;
		}
	}

	public void OnBattleFieldClickDown(params System.Object[] all_params)
	{
		current_click_down_position = (Vector2)all_params[0];
	}

	public void OnBattleFieldDragMove(params System.Object[] all_params)
	{
		Vector2 delta_position = (Vector2)all_params[0];
		Vector2 touch_position = (Vector2)all_params[1];

		if(!is_draging_battle_field && (touch_position - current_click_down_position).sqrMagnitude >= 25)
		{
			is_draging_battle_field = true;
		}

		if(is_draging_battle_field && cache_battle_field_camera != null)
		{
			Vector3 new_position = cache_battle_field_camera.position + (-Vector3.right * delta_position.x - Vector3.forward * delta_position.y) * 0.01f; 
			cache_battle_field_camera.position = new_position;
		}
	}

	public void OnBattleFieldClickUp(params System.Object[] all_params)
	{
		if(is_draging_battle_field)
		{
			is_draging_battle_field = false;
			return;
		}

		Vector2 touch_position = (Vector2)all_params[0];
		Ray camera_ray = Camera.main.ScreenPointToRay(new Vector3(touch_position.x, touch_position.y, 0));

		float distance = 0;

		bool hit_map_grid = map_grid_plane.Raycast(camera_ray, out distance);
		Vector3 hit_position = Vector3.zero;

		if(hit_map_grid)
		{
			hit_position = camera_ray.GetPoint(distance);
		}

		RaycastHit hit_info;
		bool is_hit_unit = Physics.Raycast(camera_ray, out hit_info, Mathf.Infinity, unit_layer_mask);

		BaseUnit hit_unit = null;

		if(is_hit_unit)
		{
			// 点中的是单位
			hit_unit = hit_info.transform.gameObject.GetComponent<BaseUnit>();

			if(BattleField.battle_field.IsMyTeam(hit_unit.GetTeamID()))
			{
				if(current_select_unit != null)
				{
					current_select_unit.is_selected = false;

					if(current_select_unit != hit_unit)
					{
						hit_unit.is_selected = true;
						current_select_unit = hit_unit;
					}
					else
					{
						current_select_unit = null;
					}
				}
				else
				{
					hit_unit.is_selected = true;
					current_select_unit = hit_unit;
				}	
			}
			else
			{
				if(current_select_unit != null && current_select_unit.unit_type == UnitType.Hero)
				{
					HeroUnit hero_unit = current_select_unit as HeroUnit;
					hero_unit.SetPursueTarget(hit_unit);
				}
				else
				{
					// 点中了敌方的单位，还不知道怎么处理，显示敌方信息?
				}
			}
		}
		else
		{
			// 点中的是空地
			if(hit_map_grid && current_select_unit != null && current_select_unit.unit_type == UnitType.Hero)
			{
				int grid_x = 0;
				int grid_y = 0;

				HeroUnit hero_unit = current_select_unit as HeroUnit;

				hero_unit.SetPursueTarget(null);

				if(BattleField.battle_field.WorldPositon2Grid(hit_position, out grid_x, out grid_y))
				{
					hero_unit.Move(grid_x, grid_y);
				}
			}
		}
	}

}
