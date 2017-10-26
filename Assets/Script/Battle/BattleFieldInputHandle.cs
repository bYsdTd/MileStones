using UnityEngine;
using System.Collections;

public class BattleFieldInputHandle 
{
	public Transform cache_battle_field_camera = null;

	private	HeroUnit last_select_hero_unit = null;

	public void Init()
	{
		cache_battle_field_camera = Camera.main.gameObject.transform;

		RegisterEvent();
	}

	public void Destroy()
	{
		
	}

	public void RegisterEvent()
	{
		EventManager.Instance().RegisterEvent(EventConfig.EVENT_SCENE_CLICK, OnBattleFieldClick);
		EventManager.Instance().RegisterEvent(EventConfig.EVENT_SCENE_DRAG_MOVE, OnBattleFieldDragMove);
	}

	public void OnBattleFieldClick(params System.Object[] all_params)
	{
		//Vector2 screen_position = (Vector2)all_params[0];
		Ray	camera_ray = (Ray)all_params[1];
		//bool hit_map_grid = (bool)all_params[2];
		//Vector3 hit_position = (Vector3)all_params[3];

		int layer_mask = 1 << LayerMask.NameToLayer("Unit");
		//Debug.Log("layer_mask " + layer_mask);

		RaycastHit hit_info;
		bool is_hit_unit = Physics.Raycast(camera_ray, out hit_info, Mathf.Infinity, layer_mask);

		HeroUnit hit_unit = null;

		if(is_hit_unit)
		{
			hit_unit = hit_info.transform.gameObject.GetComponent<HeroUnit>();
		
			// 点中的是单位

			if(last_select_hero_unit != null)
			{
				if(last_select_hero_unit != hit_unit)
				{
					last_select_hero_unit.SetSelected(false);

				}
			}

			hit_unit.SetSelected(true);
			last_select_hero_unit = hit_unit;

		}
		else
		{
			// 点中的是空地


		}
			
	}

	public void OnBattleFieldDragMove(params System.Object[] all_params)
	{
		Vector2 delta_position = (Vector2)all_params[0];

		if(cache_battle_field_camera != null)
		{
			Vector3 new_position = cache_battle_field_camera.position + (-Vector3.right * delta_position.x - Vector3.forward * delta_position.y) * 0.01f; 
			cache_battle_field_camera.position = new_position;
		}
	}
}
