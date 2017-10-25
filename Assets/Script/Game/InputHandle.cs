using UnityEngine;
using System.Collections;

public class InputHandle : MonoBehaviour {

	private Plane	ground_plane;

	private GameObject	last_hit_gameobject;

	// Use this for initialization
	void Start () 
	{
		ground_plane = new Plane(Vector3.up, Vector3.zero);
	}
	
	public void Update()
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
				// 检测操作的单位


				HandleTouchBegan(Input.GetTouch(0).position);
			}

			if(Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				// 绘制目标点的连线 

				HandleTouchMove(Input.GetTouch(0).position);
			}

			if(Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				// 实际的移动目标点设定

				HandleTouchEnded(Input.GetTouch(0).position);
			}	
		}
	}

	private void HandleTouchBegan(Vector2 touch_position)
	{
		RaycastHit[] hits;
		hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(touch_position));

		if(hits.Length > 0)
		{
			last_hit_gameobject = hits[0].transform.gameObject;
		}
	}

	private void HandleTouchMove(Vector2 touch_position)
	{

	}

	private void HandleTouchEnded(Vector2 touch_position)
	{
		if(last_hit_gameobject != null)
		{

			Ray ray = Camera.main.ScreenPointToRay(touch_position);

			float rayDistance;
			if (ground_plane.Raycast(ray, out rayDistance))
			{
				Vector3 dest_position = ray.GetPoint(rayDistance);

// 原来考虑用nvamesh 自带的相关寻路，但是与原有的状态机同步是个问题， 后面考虑寻路模块单独拿出来，只为表现模块提供路点数据
//				NavMeshAgent agent = last_hit_gameobject.GetComponentInParent<NavMeshAgent>();
//
//				agent.destination = dest_position;

				Actor actor = ActorMananger.Instance().GetActorMonoBehaviour(last_hit_gameobject);

				dest_position.y = ActorMananger.ACTOR_Y;

				actor.Move2Position(actor.Position, dest_position, 2);
			}

			last_hit_gameobject = null;
		}
	}
}
