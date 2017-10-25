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

		CommandManager.Instance().battle_field = battle_field;

		CommandMove move_command = new CommandMove();
		move_command.unit_id = 1;
		move_command.start_frame = 10;
		move_command.start_grid_x = 0;
		move_command.start_grid_y = 0;
		move_command.end_grid_x = 10;
		move_command.end_grid_y = 5;

		CommandManager.Instance().DispatchCommand(move_command);
	}
	
	// Update is called once per frame
	void Update () 
	{
		HandleInput();

		TimerManager.Instance().Tick(Time.deltaTime);
		UnitManager.Instance().Tick(Time.deltaTime);

	}

	private void HandleInput()
	{
		if(Input.GetMouseButtonDown(0))
		{
			HandleTouchBegan(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
		}

		if(Input.GetMouseButtonUp(0))
		{
			HandleTouchEnded(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
		}

		if(Input.touchCount > 0)
		{
			if(Input.GetTouch(0).phase == TouchPhase.Began)
			{
				HandleTouchBegan(Input.GetTouch(0).position);
			}

			if(Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				HandleTouchMove(Input.GetTouch(0).position);
			}

			if(Input.GetTouch(0).phase == TouchPhase.Ended)
			{

				HandleTouchEnded(Input.GetTouch(0).position);
			}	
		}
	}

	private void HandleTouchBegan(Vector2 touch_position)
	{
		EventManager.Instance().PostEvent(EventConfig.EVENT_BATTLE_FIELD_CLICK, new object[]{touch_position});
	}

	private void HandleTouchMove(Vector2 touch_position)
	{
	}

	private void HandleTouchEnded(Vector2 touch_position)
	{
	}
}
