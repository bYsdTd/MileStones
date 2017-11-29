using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGameObject
{
	public int 			bullet_id { set; get; }
	public GameObject 	bullet_obj { set; get; }

	public void			Tick(float delta_time)
	{
		BL.BulletComponent bullet_com = BL.BLBulletManager.Instance().GetBullet(bullet_id);

		if(bullet_com != null)
		{
			float current_time_span = (BL.BLTimelineController.Instance().current_logic_frame_time_stamp - BL.BLTimelineController.Instance().pre_logic_frame_time_stamp);

			if(current_time_span == 0)
			{
				return;
			}

			float current_elapsed = BL.BLTimelineController.Instance().time_elapsed_from_pre_frame;

			Vector3 pre_position = bullet_com.pre_position.Vector3Value();
			Vector3 next_position = bullet_com.now_position.Vector3Value();

			Vector3 now_position = Vector3.Lerp(pre_position, next_position, current_elapsed / current_time_span);

			bullet_obj.transform.position = now_position;
		}
	}
}