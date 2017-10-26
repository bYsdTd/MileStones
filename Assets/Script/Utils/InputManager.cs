using UnityEngine;
using System.Collections;

public class InputManager  
{

	static private InputManager instance = null;

	static public InputManager Instance()
	{
		if(instance == null)
		{
			instance = new InputManager();
		}

		return instance;
	}

	private Vector2 last_touch_down_position = Vector2.zero;
	private Vector2 last_touch_move_position = Vector2.zero;

	public void Tick(float delta_time)
	{
		#if UNITY_EDITOR

		if(Input.GetMouseButton(0))
		{
			Vector2 mouse_pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

			if(Input.GetMouseButtonDown(0))
			{
				last_touch_down_position = mouse_pos;
				last_touch_move_position = last_touch_down_position;
				HandleTouchBegan(last_touch_down_position);
			}
			else
			{
				Vector2 delta_position = mouse_pos - last_touch_move_position;
				last_touch_move_position = mouse_pos;

				HandleTouchMove(mouse_pos, delta_position);
			}

		}
		else if(Input.GetMouseButtonUp(0))
		{
			Vector2 mouse_pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			HandleTouchEnded(mouse_pos);
		}

		#else
		if(Input.touchCount > 0)
		{
			Touch input_touch = Input.GetTouch(0);

			if(input_touch.phase == TouchPhase.Began)
			{
				HandleTouchBegan(input_touch.position);
			}

			if(input_touch.phase == TouchPhase.Moved)
			{
				HandleTouchMove(input_touch.position);
			}

			if(input_touch.phase == TouchPhase.Ended)
			{

				HandleTouchEnded(input_touch.position);
			}	
		}
		#endif
	}

	private void HandleTouchBegan(Vector2 touch_position)
	{
		//Debug.Log("began " + touch_position);
//		 EventManager.Instance().PostEvent(EventConfig.EVENT_SCENE_CLICK, new object[]{touch_position});
	}

	private void HandleTouchMove(Vector2 touch_position, Vector2 delta_position)
	{
		//Debug.Log("HandleTouchMove " + touch_position + " delta_position " + delta_position);

		EventManager.Instance().PostEvent(EventConfig.EVENT_SCENE_DRAG_MOVE, new object[]{delta_position, touch_position});
	}

	private void HandleTouchEnded(Vector2 touch_position)
	{
		//Debug.Log("HandleTouchEnded " + touch_position);
	}
}
