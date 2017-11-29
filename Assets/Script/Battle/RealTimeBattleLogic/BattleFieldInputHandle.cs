using UnityEngine;
using System.Collections;

public class BattleFieldInputHandle 
{
	static private BattleFieldInputHandle instance = null;

	static public BattleFieldInputHandle Instance()
	{
		if(instance == null)
		{
			instance = new BattleFieldInputHandle();
		}

		return instance;
	}

	public bool enabled { set; get; }

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

		enabled = false;
	}

	public void Destroy()
	{
		EventManager.Instance().UnRegisterEvent(EventConfig.EVENT_SCENE_CLICK_DOWN, OnBattleFieldClickDown);
		EventManager.Instance().UnRegisterEvent(EventConfig.EVENT_SCENE_CLICK_MOVE, OnBattleFieldDragMove);
		EventManager.Instance().UnRegisterEvent(EventConfig.EVENT_SCENE_CLICK_UP, OnBattleFieldClickUp);
		EventManager.Instance().UnRegisterEvent(EventConfig.EVENT_L2R_PLAY_DEAD, OnUnitDead);
	}

	public void RegisterEvent()
	{
		EventManager.Instance().RegisterEvent(EventConfig.EVENT_SCENE_CLICK_DOWN, OnBattleFieldClickDown);
		EventManager.Instance().RegisterEvent(EventConfig.EVENT_SCENE_CLICK_MOVE, OnBattleFieldDragMove);
		EventManager.Instance().RegisterEvent(EventConfig.EVENT_SCENE_CLICK_UP, OnBattleFieldClickUp);
		EventManager.Instance().RegisterEvent(EventConfig.EVENT_L2R_PLAY_DEAD, OnUnitDead);
	}

	public void OnUnitDead(params System.Object[] all_params)
	{
		int unit_id = (int)all_params[0];
		BaseUnit unit_dead = UnitManager.Instance().GetUnit(unit_id);

		if(current_select_unit != null && unit_dead == current_select_unit)
		{
			current_select_unit.is_selected = false;
			current_select_unit = null;
		}
	}

	public void OnBattleFieldClickDown(params System.Object[] all_params)
	{
		if(!enabled)
		{
			return;
		}

		current_click_down_position = (Vector2)all_params[0];
	}

	public void OnBattleFieldDragMove(params System.Object[] all_params)
	{
		if(!enabled)
		{
			return;
		}

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

//	public void OnBattleFieldClickUp(params System.Object[] all_params)
//	{
//		if(is_draging_battle_field)
//		{
//			is_draging_battle_field = false;
//			return;
//		}
//
//		Vector2 touch_position = (Vector2)all_params[0];
//		Ray camera_ray = Camera.main.ScreenPointToRay(new Vector3(touch_position.x, touch_position.y, 0));
//
//		float distance = 0;
//
//		bool hit_map_grid = map_grid_plane.Raycast(camera_ray, out distance);
//		Vector3 hit_position = Vector3.zero;
//
//		if(hit_map_grid)
//		{
//			hit_position = camera_ray.GetPoint(distance);
//		}
//
//		RaycastHit hit_info;
//		bool is_hit_unit = Physics.Raycast(camera_ray, out hit_info, Mathf.Infinity, unit_layer_mask);
//
//		BaseUnit hit_unit = null;
//
//		if(is_hit_unit)
//		{
//			// 点中的是单位
//			hit_unit = hit_info.transform.gameObject.GetComponent<BaseUnit>();
//
//			if(BattleField.battle_field.IsMyTeam(hit_unit.GetTeamID()))
//			{
//				if(current_select_unit != null)
//				{
//					current_select_unit.is_selected = false;
//
//					if(current_select_unit != hit_unit)
//					{
//						hit_unit.is_selected = true;
//						current_select_unit = hit_unit;
//					}
//					else
//					{
//						current_select_unit = null;
//					}
//				}
//				else
//				{
//					hit_unit.is_selected = true;
//					current_select_unit = hit_unit;
//				}	
//			}
//			else
//			{
//				if(current_select_unit != null && current_select_unit.unit_type == UnitType.Hero)
//				{
//					HeroUnit hero_unit = current_select_unit as HeroUnit;
//					hero_unit.SetPursueTarget(hit_unit);
//				}
//				else
//				{
//					// 点中了敌方的单位，还不知道怎么处理，显示敌方信息?
//				}
//			}
//		}
//		else
//		{
//			// 点中的是空地
//			if(hit_map_grid && current_select_unit != null && current_select_unit.unit_type == UnitType.Hero)
//			{
//				int grid_x = 0;
//				int grid_y = 0;
//
//				HeroUnit hero_unit = current_select_unit as HeroUnit;
//
//				hero_unit.SetPursueTarget(null);
//
//				if(BattleField.battle_field.WorldPositon2Grid(hit_position, out grid_x, out grid_y))
//				{
//					hero_unit.Move(grid_x, grid_y);
//				}
//			}
//		}
//	}

	public Vector3 GetCurrentCameraCenter()
	{
		Ray camera_ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));

		float distance = 0;

		bool hit_map_grid = map_grid_plane.Raycast(camera_ray, out distance);
		Vector3 hit_position = Vector3.zero;

		if(hit_map_grid)
		{
			hit_position = camera_ray.GetPoint(distance);
		}

		hit_position.y = 0;

		return hit_position;
	}

	public void OnBattleFieldClickUp(params System.Object[] all_params)
	{
		if(!enabled)
		{
			return;
		}

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

			if(BattleField.battle_field.IsMyTeam(hit_unit.team_id))
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
				// 这个地方要做成追击指令

//				if(current_select_unit != null && current_select_unit.unit_type == UnitType.Hero)
//				{
//					HeroUnit hero_unit = current_select_unit as HeroUnit;
//					hero_unit.SetPursueTarget(hit_unit);
//				}
//				else
//				{
//					// 点中了敌方的单位，还不知道怎么处理，显示敌方信息?
//				}
			}
		}
		else
		{
			// 点中的是空地
			if(hit_map_grid && current_select_unit != null && current_select_unit.unit_type == UnitType.Hero)
			{
				
//				HeroUnit hero_unit = current_select_unit as HeroUnit;
//
//				hero_unit.SetPursueTarget(null);
//
//				if(BattleField.battle_field.WorldPositon2Grid(hit_position, out grid_x, out grid_y))
//				{
//					hero_unit.Move(grid_x, grid_y);
//				}

				// 这里是临时的处理
				// 正常应该是按照协议的标准去发包到服务器
				// 在收到包的时候才创建command

//				BL.BLIntVector3 dest_position;
//				dest_position.x = (int)(hit_position.x * 1000);
//				dest_position.y = 0;
//				dest_position.z = (int)(hit_position.z * 1000);
//
//				BL.BLCommandBase command = BL.BLCommandManager.Instance().CreateMove2PositionCommand(current_select_unit.unit_id, 0, dest_position);
//				LocalServer.Instance().SendPackeage(command);

				BattleProto.Tick command = new BattleProto.Tick();
				command.team_id = BattleField.battle_field.my_team_id;
				command.command_type = BL.TickCommandType.Move;
				command.cast_id = current_select_unit.unit_id;
				command.x = (int)(hit_position.x * 1000);
				command.y = (int)(hit_position.z * 1000);

				NetManager.Instance().SendPacket(RequestId.Tick, command);
			}
		}
	}
}
