using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
	public class BLCommandManager
	{
		static private BLCommandManager instance = null;

		static public BLCommandManager Instance()
		{
			if(instance == null)
			{
				instance = new BLCommandManager();
			}

			return instance;
		}

		// 帧，指令队列
		private Dictionary<int, List<BLCommandBase>> command_list = new Dictionary<int, List<BLCommandBase>>();

		public void AddCommand(int logic_frame, BLCommandBase command)
		{
			
		}

		public List<BLCommandBase> GetAllCommands(int logic_frame)
		{
			if(command_list.ContainsKey(logic_frame))
			{
				return command_list[logic_frame];
			}
			else
			{
				return null;
			}
		}
	}	
}
