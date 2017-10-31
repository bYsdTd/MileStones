//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//using Pathfinding.RVO;
//
//public class CommandMove2Target : CommandBase 
//{
//	public HeroUnit	pursue_target;
//
//	private float pursue_cool_down = 0;
//
//	public override void OnStart ()
//	{
//		base.OnStart ();
//
//		hero_unit.PlayMove();
//	}
//
//	public override void OnEnd ()
//	{
//		base.OnEnd ();
//
//		hero_unit.PlayIdle();
//	}
//
//	public override bool Tick (float delta_time)
//	{
//		base.Tick (delta_time);
//
//		if(pursue_cool_down <= 0)
//		{
//			Vector3 dir = pursue_target._position - hero_unit._position;
//			dir.Normalize();
//
//			Vector3 new_position = hero_unit._position + dir * hero_unit.move_speed * delta_time;
//
//			int grid_x;
//			int grid_y;
//
//			if(BattleField.battle_field.WorldPositon2Grid(pursue_target._position, out grid_x, out grid_y))
//			{
//				if(BattleField.battle_field.IsBlock(grid_x, grid_y))
//				{
//					hero_unit.SetPursueTarget(null);
//					return true;
//				}
//			}
//			else
//			{
//				hero_unit.SetPursueTarget(null);
//				return true;	
//			}	
//
//			hero_unit.SetPosition(new_position);
//			hero_unit.SetDirection(dir);
//
//			if(hero_unit.IsCanAttack(pursue_target) || !hero_unit.IsCanSeeUnit(pursue_target))
//			{
//				return true;
//			}
//			else
//			{
//				return false;
//			}
//		}
//		else
//		{
//			pursue_cool_down -= delta_time;
//		}
//	}
//}
