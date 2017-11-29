using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
	public class BLCommandUnitBase : BLCommandBase
	{
		public int				cast_unit_id { set; get; }
		protected BLUnitBase	cast_unit;

		public override void OnInit ()
		{
			base.OnInit ();
		}

		public override void OnDestroy()
		{
			base.OnDestroy();

			cast_unit = null;
		}

		public override void DoCommand()
		{
			cast_unit = BLUnitManager.Instance().GetUnit(cast_unit_id);

			if(cast_unit != null)
			{
				cast_unit.DoCommand(this);
			}	
		}

		public override bool Tick()
		{
			return true;
		}
	}	
}
