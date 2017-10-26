using UnityEngine;
using System.Collections;

public class Battle: MonoBehaviour
{
	public BattleGridRenderer battle_grid_renderer;

	BattleField		battle_field;

	// Use this for initialization
	void Start () 
	{
		// 初始化
		battle_field = new BattleField();
		BattleField.battle_field = battle_field;

		string map_path = "";
		#if UNITY_EDITOR
		map_path = Application.dataPath + "/MapData/map_block_info";
		#endif

		battle_field.LoadMap(map_path);
		battle_field.SetBattleGridRenderer(battle_grid_renderer);
		battle_field.InitUnit();
		battle_field.InitInputHandle();


//		CommandMove move_command = new CommandMove();
//		move_command.unit_id = 1;
//		move_command.start_frame = 10;
//		move_command.start_grid_x = 0;
//		move_command.start_grid_y = 0;
//		move_command.end_grid_x = 10;
//		move_command.end_grid_y = 5;
//
//		CommandManager.Instance().DispatchCommand(move_command);
	}
	
	// Update is called once per frame
	void Update () 
	{
		float delta_time = Time.deltaTime;
		InputManager.Instance().Tick(delta_time);
		TimerManager.Instance().Tick(delta_time);
		UnitManager.Instance().Tick(delta_time);
	}
}
