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
			if(!command_list.ContainsKey(logic_frame))
			{
				command_list.Add(logic_frame, new List<BLCommandBase>());
			}

			//Debug.Log("logic_frame  " + logic_frame + " add command");

			command_list[logic_frame].Add(command);
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

		public void Tick(int logic_frame)
		{
			List<BLCommandBase> current_all_command = GetAllCommands(logic_frame);

			if(current_all_command != null)
			{
				for(int i = 0; i < current_all_command.Count; ++i)
				{
					current_all_command[i].DoCommand();
				}
			}
		}

		public BLCommandMove2Position CreateMove2PositionCommand(int cast_id, int cast_frame, BLIntVector3 dest_position)
		{
			BLCommandMove2Position move_command = new BLCommandMove2Position();
			move_command.cast_frame = cast_frame;
			move_command.cast_unit_id = cast_id;
			move_command.dest_position = dest_position;

			return move_command;
		}
	}	
}
