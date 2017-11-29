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
		BulletManager.Instance().Init();

		GDSKit.GDSMgr.Instance().InitGDSData();

		// 初始化
		battle_field = new BattleField();
		BattleField.battle_field = battle_field;

		string map_path = "";

		map_path = FileManager.Instance().GetReadOnlyPath() + "MapData/map_block_info";

		battle_field.LoadMap(map_path);

		battle_field.SetBattleGridRenderer(battle_grid_renderer);
		BattleFieldInputHandle.Instance().Init();
		UnitManager.Instance().Init();
	}

	// 加入房间
	void JoinRoom(int room_id)
	{
		// 这里逻辑层和渲染层同时初始化了
		BL.BLUnitManager.Instance().InitBase();


		// 加载完成，加入房间1
		NetManager.Instance().JoinRoom(room_id);
	}

	// Update is called once per frame
	void Update () 
	{

		float delta_time = Time.deltaTime;

		// 网络层
		NetManager.Instance().Tick(delta_time);

		// 逻辑层
		BL.BLTimelineController.Instance().Tick(delta_time);

		// 用户输入层
		InputManager.Instance().Tick(delta_time);
		TimerManager.Instance().Tick(delta_time);

		// 渲染层
		UnitManager.Instance().Tick(delta_time);
		BulletManager.Instance().Tick(delta_time);


		// 目前没有战场的逻辑, 这个tick是空的，逻辑都在timeline controller里
		if(battle_field != null)
		{
			battle_field.Tick(delta_time);
		}
	}

	void OnApplicationQuit()
	{
		BattleFieldInputHandle.Instance().Destroy();

		GUIManager.Instance().Destroy();
		BulletManager.Instance().Destroy();

		UnitManager.Instance().Destroy();

		NetManager.Instance().Close();
	}

	string str_room_id = "";

	void OnGUI() 
	{
		if(battle_field != null)
		{
			GUIStyle style = new GUIStyle();
			style.fontSize = 20;
			style.normal.textColor = Color.white;

			GUI.Label(new Rect(Screen.width - 200, 10, 100, 30), battle_field.GetStateText(), style);
		}

		if(BL.BLTimelineController.Instance().is_join_room == false)
		{
			str_room_id = GUI.TextField(new Rect(Screen.width/2 - 100, Screen.height/2 - 20, 200, 20), str_room_id, 10);	

			if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 + 20, 100, 50), "连入房间"))
			{
				int room_id = int.Parse(str_room_id);

				JoinRoom(room_id);
			}
		}
	}
}
