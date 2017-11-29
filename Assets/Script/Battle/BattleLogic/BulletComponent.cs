using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
	public class BulletComponent 
	{
		public int 		bullet_id;

		public BLIntVector3 start_postion { set; get; }
		public BLIntVector3 end_position { set; get; }

		public BLIntVector3 pre_position { set; get; }
		public BLIntVector3 now_position { set; get; }

		public int	bullet_speed { set; get; }

		private int	remain_frames { set; get; }
		private Vector3	 bullet_dir { set; get; }

		private int total_frames { set; get; }

		// 目前是直线的组件，将来如果有别的需求，就扩展bullet 组件
		public delegate void HitEndCallback(BulletComponent bullet_component);

		public HitEndCallback end_call_back = null;

		public void Start(BLIntVector3 start_pos, BLIntVector3 end_pos, HitEndCallback call_back)
		{
			start_postion = start_pos;
			end_position = end_pos;

			remain_frames = (int)((end_position - start_postion).Magnitude() / bullet_speed / BLTimelineController.MS_PER_FRAME);
			end_call_back = call_back;
			total_frames = remain_frames;

			pre_position = start_pos;
			now_position = start_pos;

			// 发送创建表现层子弹的事件
			EventManager.Instance().PostEvent(EventConfig.EVENT_L2R_BULLET_START, new object[]{ bullet_id });
		}

		public bool Tick()
		{
			if(remain_frames <= 0)
			{
				EventManager.Instance().PostEvent(EventConfig.EVENT_L2R_BULLET_END, new object[]{ bullet_id });

				// 发送销毁子弹粒子的事件
				if(end_call_back != null)
				{
					end_call_back.Invoke(this);
				}

				return true;
			}
			else
			{
				pre_position = now_position;
				now_position = BLIntVector3.Lerp(start_postion, end_position, total_frames-remain_frames, total_frames);

				remain_frames--;
			}

			return false;
		}


	}

}

