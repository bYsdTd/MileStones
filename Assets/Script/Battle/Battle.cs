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


		// test


		TimerManager.Instance().RepeatCallFunc(delegate(float dt) {

			HeroUnit unit1 = UnitManager.Instance().GetHeroUnit(1);
			HeroUnit unit2 = UnitManager.Instance().GetHeroUnit(2);

			unit1.PlayAttack();

			unit1.SetDirection(unit2._position - unit1._position);

			unit2.PlayHited();

		}, 1);
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
