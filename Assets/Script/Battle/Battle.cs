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

		string map_path = "";
		#if UNITY_EDITOR
		map_path = Application.dataPath + "/MapData/map_block_info";
		#endif

		battle_field.LoadMap(map_path);
		battle_field.SetBattleGridRenderer(battle_grid_renderer);
		battle_field.InitUnit();
		battle_field.InitInputHandle();
	}
	
	// Update is called once per frame
	void Update () 
	{
		TimerManager.Instance().Tick(Time.deltaTime);
		UnitManager.Instance().Tick(Time.deltaTime);

	}
}
