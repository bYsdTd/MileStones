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

		public bool				mark_delete { set; get; }

		// pre position 用作逻辑层差值
		public BLIntVector3		pre_position { set; get; }
		public BLIntVector3		position { set; get; }

		public int				hp { set; get;}
		public int				max_hp { set; get; }

		private int				vision_;
		private int				vision_sqr_;

		public int				vision 
		{
			set
			{
				vision_ = value;

				vision_sqr_ = vision * vision;
			}

			get
			{
				return vision_;
			}
		}

		protected	BLCommandBase	current_command_;

		// 表现层使用数据
		public Vector3			dir { set; get; }

		public bool IsAlive()
		{
			return hp > 0;	
		}

		// 考虑共享视野
		public bool IsCanSeeUnit(BLUnitBase enemy_unit)
		{
			HashSet<BLUnitBase> vision_enemy_units = BattleVisionControl.Instance().GetEnemys(team_id);

			return vision_enemy_units.Contains(enemy_unit);
		}

		// 只考虑自己
		public bool IsCanSeeUnitCheckOnlyMyself(BLUnitBase enemy_unit)
		{
			float distance_square = (enemy_unit.position - position).SqrMagnitude();

			return vision_sqr_ >= distance_square;
		}

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

		virtual public void OnDead()
		{
			EventManager.Instance().PostEvent(EventConfig.EVENT_L2R_PLAY_DEAD, new object[]{ unit_id });

			mark_delete = true;
		}

		public void OnDamage(int hp_changed)
		{
			//Debug.Log(" 单位收到伤害: " + unit_id + "  伤害: " + hp_changed);

			// 可能同时收到多个伤害, 但是只结算一次
			if(!IsAlive())
			{
				return;
			}

			hp -= hp_changed;

			if(hp <= 0)
			{
				OnDead();
			}
			else
			{
				EventManager.Instance().PostEvent(EventConfig.EVENT_L2R_PLAY_HIT, new object[]{ unit_id });
			}
		}
	}	
}
