using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletComponent 
{
	public int 		bullet_id;

	public Vector3 start_postion { set; get; }
	public Vector3 end_position { set; get; }

	public float	bullet_speed { set; get; }

	private float	time_stamp { set; get; }
	private Vector3	 bullet_dir { set; get; }

	// 目前是直线的组件，将来如果有别的需求，就扩展bullet 组件
	public delegate void HitEndCallback(BulletComponent bullet_component);

	public HitEndCallback end_call_back = null;

	private GameObject bullet_game_object;

	public void Start(Vector3 start_pos, Vector3 end_pos, HitEndCallback call_back)
	{
		start_postion = start_pos;
		end_position = end_pos;

		bullet_dir = (end_position - start_postion).normalized;
		time_stamp = (end_position - start_postion).magnitude / bullet_speed;
		end_call_back = call_back;

		bullet_game_object = ObjectPoolManager.Instance().GetObject("Missil_01");

		bullet_game_object.transform.SetParent(UnitManager.Instance().cache_root_effect_node, false);
		bullet_game_object.transform.position = start_postion;

		bullet_game_object.transform.rotation = Quaternion.LookRotation(bullet_dir);
	}

	public bool Tick(float delta_time)
	{
		if(time_stamp <= 0)
		{
			if(bullet_game_object != null)
			{
				ObjectPoolManager.Instance().ReturnObject("Missil_01", bullet_game_object);
				bullet_game_object = null;
			}

			if(end_call_back != null)
			{
				end_call_back.Invoke(this);
			}

			return true;
		}
		else
		{
			if(bullet_game_object != null)
			{
				bullet_game_object.transform.position += bullet_dir * delta_time * bullet_speed;
			}

			time_stamp -= delta_time;
		}

		return false;
	}


}
