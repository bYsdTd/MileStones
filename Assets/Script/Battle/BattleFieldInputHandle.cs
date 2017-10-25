using UnityEngine;
using System.Collections;

public class BattleFieldInputHandle 
{
	public void RegisterEvent()
	{
		EventManager.Instance().RegisterEvent(EventConfig.EVENT_BATTLE_FIELD_CLICK, OnBattleFieldClick);
	}

	public void OnBattleFieldClick(params System.Object[] all_params)
	{
		//Vector2 screen_position = (Vector2)all_params[0];


	}
}
