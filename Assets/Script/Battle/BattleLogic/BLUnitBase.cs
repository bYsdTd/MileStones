using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
	public class BLUnitBase  
	{
		public UnitType			unit_type { set; get; }
		public int 				unit_id { set; get; }
		public string			gds_name { set; get; }
		public int				team_id { set; get; }

		public BLIntVector3		pre_position { set; get; }
		public BLIntVector3		position { set; get; }

		public int				hp { set; get;}
		public int				max_hp { set; get; }

		public int				vision { set; get; }

		private	BLCommandBase	current_command_;


		virtual public void OnInit()
		{
			
		}

		virtual public void OnDestroy()
		{
			
		}
			
		virtual public void	Tick()
		{
			if(current_command_ != null && current_command_.Tick())
			{
				current_command_.OnDestroy();
				current_command_ = null;
			}
		}

		public void DoCommand(BLCommandBase	command)
		{
			if(current_command_ != null)
			{
				current_command_.OnDestroy();

				current_command_ = null;
			}

			current_command_ = command;

			current_command_.OnInit();
		}
	}	
}
