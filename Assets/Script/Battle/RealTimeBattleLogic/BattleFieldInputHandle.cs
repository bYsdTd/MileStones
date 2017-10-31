using UnityEngine;
using System.Collections;

public class BattleFieldInputHandle 
{
	public Transform cache_battle_field_camera = null;

	private	HeroUnit current_select_hero_unit = null;

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
		EventManager.Instance().UnRegisterEvent(EventConfig.EVENT_HERO_UNIT_DEAD, OnHeroUnitDead);
	}

	public void RegisterEvent()
	{
		EventManager.Instance().RegisterEvent(EventConfig.EVENT_SCENE_CLICK_DOWN, OnBattleFieldClickDown);
		EventManager.Instance().RegisterEvent(EventConfig.EVENT_SCENE_CLICK_MOVE, OnBattleFieldDragMove);
		EventManager.Instance().RegisterEvent(EventConfig.EVENT_SCENE_CLICK_UP, OnBattleFieldClickUp);
		EventManager.Instance().RegisterEvent(EventConfig.EVENT_HERO_UNIT_DEAD, OnHeroUnitDead);
	}

	public void OnHeroUnitDead(params System.Object[] all_params)
	{
		HeroUnit hero_unit_dead = (HeroUnit)all_params[0];

		if(hero_unit_dead == current_select_hero_unit)
		{
			current_select_hero_unit = null;
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

		HeroUnit hit_unit = null;

		if(is_hit_unit)
		{
			// 点中的是单位
			hit_unit = hit_info.transform.gameObject.GetComponent<HeroUnit>();

			if(BattleField.battle_field.IsMyTeam(hit_unit.GetTeamID()))
			{
				if(current_select_hero_unit != null)
				{
					current_select_hero_unit.SetSelected(false);

					if(current_select_hero_unit != hit_unit)
					{
						hit_unit.SetSelected(true);
						current_select_hero_unit = hit_unit;
					}
					else
					{
						current_select_hero_unit = null;
					}
				}
				else
				{
					hit_unit.SetSelected(true);
					current_select_hero_unit = hit_unit;
				}	
			}
			else
			{
				if(current_select_hero_unit != null)
				{
					current_select_hero_unit.SetPursueTarget(hit_unit);
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
			if(hit_map_grid && current_select_hero_unit != null)
			{
				int grid_x = 0;
				int grid_y = 0;

				current_select_hero_unit.SetPursueTarget(null);

				if(BattleField.battle_field.WorldPositon2Grid(hit_position, out grid_x, out grid_y))
				{
//					int current_x = 0;
//					int current_y = 0;
//
//					if(!BattleField.battle_field.IsBlock(grid_x, grid_y))
//					{
//						BattleField.battle_field.WorldPositon2Grid(current_select_hero_unit._position, out current_x, out current_y);
//
//						CommandMove move_command = new CommandMove();
//						move_command.unit_id = current_select_hero_unit.unit_id;
//						//move_command.start_frame = 10;
//						move_command.start_grid_x = current_x;
//						move_command.start_grid_y = current_y;
//
//						move_command.end_grid_x = grid_x;
//						move_command.end_grid_y = grid_y;
//
//						CommandManager.Instance().DispatchCommand(move_command);
//					}

					current_select_hero_unit.Move(grid_x, grid_y);
				}
			}
		}
	}

}
