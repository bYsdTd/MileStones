using UnityEngine;
using System.Collections;

public class BattleFieldInputHandle 
{
	public Transform cache_battle_field_camera = null;

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
