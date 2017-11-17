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

			}

			return instance;
		}

		private int	battle_start_time { set; get; }

	 	public bool	is_start { set; get; }

		public int current_frame { set; get; }

		public static int FRAME_RATE = 16;
		public static int MS_PER_FRAME = 1000 / 16;

		public void Start()
		{
			current_frame = 0;
			is_start = true;
			// 临时设置成本地的时间，之后替换成服务器发下来的开始时间戳
			battle_start_time = System.Environment.TickCount;
		}

		public void Tick()
		{
			if(!is_start)
			{
				return;
			}

			// 目前由于没有服务器同步，所以是按照客户端的时间在走逻辑层运算
			// 加入服务器同步以后，要按照服务器的同步帧，去执行

			int total_elapsed = System.Environment.TickCount - battle_start_time;

			int new_frame = total_elapsed / MS_PER_FRAME;

			int elapsed_frame = new_frame - current_frame;

			DoMainLogic(elapsed_frame);
		}

		public void DoMainLogic(int elapsed_frame)
		{
			while(elapsed_frame > 0)
			{
				--elapsed_frame;

				// 有指令的话，分发指令
				BLCommandManager.Instance().Tick(current_frame);

				BLUnitManager.Instance().Tick();

				++current_frame;
			}
		}
	}
}
