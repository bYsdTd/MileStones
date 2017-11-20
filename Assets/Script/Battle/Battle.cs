using UnityEngine;
using System.Collections;

public class Battle: MonoBehaviour
{
	public BattleGridRenderer battle_grid_renderer;

	BattleField		battle_field;

	// Use this for initialization
	void Start () 
	{
		Application.targetFrameRate = 60;
		QualitySettings.vSyncCount = 0;

		GUIManager.Instance().Init();

		// 
		GDSKit.GDSMgr.Instance().InitGDSData();

		// 初始化
		battle_field = new BattleField();
		BattleField.battle_field = battle_field;

		string map_path = "";

		map_path = FileManager.Instance().GetReadOnlyPath() + "MapData/map_block_info";

		battle_field.LoadMap(map_path);
		// 实时战斗的时候需要初始化这个
		battle_field.InitRealTimeLogic();
		battle_field.SetBattleGridRenderer(battle_grid_renderer);

		UnitManager.Instance().OnInit();
		// 这里逻辑层和渲染层同时初始化了
		BL.BLTimelineController.Instance().Start();
		BL.BLUnitManager.Instance().InitUnit();

	}
	
	// Update is called once per frame
	void Update () 
	{
		float delta_time = Time.deltaTime;

		// 逻辑层
		BL.BLTimelineController.Instance().Tick(delta_time);

		InputManager.Instance().Tick(delta_time);
		TimerManager.Instance().Tick(delta_time);

		// 渲染层
		UnitManager.Instance().Tick(delta_time);

		if(battle_field != null)
		{
			battle_field.Tick(delta_time);
		}


	}
}
