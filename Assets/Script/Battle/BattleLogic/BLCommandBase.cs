using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
	public class BLCommandBase
	{
		public int				cast_frame { set; get; }
		public int				command_type { set; get; }

		public virtual void OnInit()
		{
			
		}

		public virtual void OnDestroy()
		{
		}

		public virtual void DoCommand()
		{
		}

		public virtual bool Tick()
		{
			return true;
		}
	}	
}
