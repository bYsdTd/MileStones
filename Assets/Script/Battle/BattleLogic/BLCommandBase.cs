using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
	public class BLCommandBase
	{
		public int				cast_unit_id { set; get; }
		public int				cast_frame { set; get; }
		public BLCommandType	command_type { set; get; }

		protected BLUnitBase	cast_unit;

		public virtual void OnInit()
		{
			
		}

		public virtual void OnDestroy()
		{
			cast_unit = null;
		}

		public void DoCommand()
		{
			cast_unit = BLUnitManager.Instance().GetUnit(cast_unit_id);

			if(cast_unit != null)
			{
				cast_unit.DoCommand(this);
			}
		}

		public virtual bool Tick()
		{
			return true;
		}
	}	
}
