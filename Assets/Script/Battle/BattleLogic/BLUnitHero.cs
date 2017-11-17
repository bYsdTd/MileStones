using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
	public class BLUnitHero : BLUnitBase
	{
		public int 		attack_range { set; get; }
		public int		attack_power { set; get; }
		public int		attack_speed { set; get; }

		public bool		is_move_attack { set; get; }
		public bool 	can_attack_ground { set; get; }
		public bool 	can_attack_fly { set; get; }

		public int 		aoe_radius { set; get; }
		public int 		bullet_speed { set; get; }

		public int		pursue_rate { set; get; }
		public bool 	can_pursue { set; get; }

		public int 		move_speed { set; get; }
		public bool		is_fly { set; get; }

		public int		revive_cool_down { set; get; }


		public override void Tick ()
		{
			base.Tick ();
		}

	}	
}
