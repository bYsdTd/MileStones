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
		Vector3 position = (Vector3)all_params[0];
		Debug.Log("OnBattleFieldClick " + position);
	}
}
