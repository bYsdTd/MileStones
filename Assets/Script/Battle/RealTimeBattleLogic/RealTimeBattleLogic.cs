using UnityEngine;
using System.Collections;

public class RealTimeBattleLogic 
{
	public int my_team_id;

	BattleFieldInputHandle battle_field_input_handle;
	public BattleVisionControl	battle_vision_control;

	public void Init(int team_id)
	{
		my_team_id = team_id;

		battle_field_input_handle = new BattleFieldInputHandle();

		battle_field_input_handle.Init();

		battle_vision_control = new BattleVisionControl();

		battle_vision_control.my_real_time_logic = this;
	}

	public void Tick(float delta_time)
	{
		if(battle_vision_control != null)
		{
			battle_vision_control.Tick(delta_time);
		}
	}
}
