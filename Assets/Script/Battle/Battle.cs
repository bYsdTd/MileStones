using UnityEngine;
using System.Collections;

public class Battle: MonoBehaviour
{
	public BattleGridRenderer battle_grid_renderer;

	BattleField		battle_field;

	// Use this for initialization
	void Start () 
	{
		// 
		GDSKit.GDSMgr.Instance().InitGDSData();

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

		// 实时战斗的时候需要初始化这个
		battle_field.InitRealTimeLogic();

	}
	
	// Update is called once per frame
	void Update () 
	{
		float delta_time = Time.deltaTime;
		InputManager.Instance().Tick(delta_time);
		TimerManager.Instance().Tick(delta_time);
		UnitManager.Instance().Tick(delta_time);

		if(battle_field != null)
		{
			battle_field.Tick(delta_time);
		}
	}
}
