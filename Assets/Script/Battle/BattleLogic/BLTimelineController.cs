using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
	public class BLTimelineController  
	{
		static private BLTimelineController instance = null;

		static public BLTimelineController Instance()
		{
			if(instance == null)
			{
				instance = new BLTimelineController();

				instance.is_start = false;
				instance.is_join_room = false;

			}

			return instance;
		}

	 	public bool	is_start { set; get; }

		public bool is_join_room { set; get; }

		public int current_logic_frame { set; get; }

		// 这三个变量，用作表现层做插值用
		// 分别是当前前一帧的时间戳，当前后一帧的时间戳，和距离前一帧的经历的时间
		public float pre_logic_frame_time_stamp { set; get; }
		public float current_logic_frame_time_stamp { set; get; }
		public float time_elapsed_from_pre_frame { set; get; }

		public int frame_received { set; get; }

		public static int FRAME_RATE = 16;
		public static float MS_PER_FRAME = 1000.0f / FRAME_RATE;
		public static float SECOND_PER_FRAME = 1.0f / FRAME_RATE;


		private float 	time_pre_frame = 0;
		private int		now_local_logic_frame = 0;

		private float	total_time_elapsed = 0;

		public void Start()
		{
			current_logic_frame = 0;
			is_start = true;

			time_pre_frame = Time.realtimeSinceStartup;
			pre_logic_frame_time_stamp = Time.realtimeSinceStartup;
			current_logic_frame_time_stamp = Time.realtimeSinceStartup;
			time_elapsed_from_pre_frame = 0;

			now_local_logic_frame = 0;

			total_time_elapsed = 0;

			frame_received = 0;
		}

		int pre_time = 0;

		public void Tick(float dt)
		{
			if(!is_start)
			{
				return;
			}

			// 切入后台的时候dt 不是真实的dt，所以要用下面的自己计算的
			float delta_time = (Time.realtimeSinceStartup - time_pre_frame);
			total_time_elapsed += delta_time;

			time_pre_frame = Time.realtimeSinceStartup;

			now_local_logic_frame = (int)(total_time_elapsed / SECOND_PER_FRAME);

			int need_move_frame = now_local_logic_frame - current_logic_frame;

			int real_move_frame = 0;
			if(frame_received >= need_move_frame)
			{
				real_move_frame = need_move_frame;
			}
			else
			{
				real_move_frame = frame_received;
			}

			//Debug.Log("frame_received " + frame_received + " current_logic_frame " + current_logic_frame + " now_local_logic_frame " + now_local_logic_frame + " real_move_frame " + real_move_frame);

			if( real_move_frame > 0 )
			{
				time_elapsed_from_pre_frame = 0;
			}
			else
			{
				time_elapsed_from_pre_frame += delta_time;
			}
				
			DoMainLogic(real_move_frame);

			frame_received -= real_move_frame;

		}

		public void DoMainLogic(int elapsed_frame)
		{
			while(elapsed_frame > 0)
			{
				--elapsed_frame;

				//pre_logic_frame_time_stamp = current_logic_frame * MS_PER_FRAME * 0.001f;
				pre_logic_frame_time_stamp = current_logic_frame_time_stamp;
				current_logic_frame_time_stamp = total_time_elapsed;

				// 有指令的话，分发指令
				BLCommandManager.Instance().Tick(current_logic_frame);

				BLUnitManager.Instance().Tick();

				++current_logic_frame;

				//current_logic_frame_time_stamp = current_logic_frame * MS_PER_FRAME * 0.001f;
			}
		}
	}
}
