//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//
//public class RobotBaseAI  
//{
//	private Vector3 	enemy_base_position { set; get; }
//	private HeroUnit	hero_unit;
//
//	public RobotBaseAI(HeroUnit unit,  Vector3 base_position )
//	{
//		enemy_base_position = base_position;
//		hero_unit = unit;
//
//		//OnIdle();
//	}
//
//	public void Tick(float delta_time)
//	{
//		if(hero_unit != null && !hero_unit.IsHaveAttackTarget() && !hero_unit.IsHavePursueTarget())
//		{
//			BaseUnit unit = AttackAI.FindCanAttackTarget(hero_unit);
//
//			hero_unit.SetPursueTarget(unit);
//		}
//	}
//
//	public void OnIdle()
//	{
//		if(hero_unit != null && !hero_unit.IsMoveState() && !hero_unit.IsHaveAttackTarget() && !hero_unit.IsHavePursueTarget())
//		{
//			int grid_x;
//			int grid_y;
//
//			BattleField.battle_field.WorldPositon2Grid(enemy_base_position, out grid_x, out grid_y);
//
//			grid_x += 10;
//			grid_y += 3;
//
//			hero_unit.Move(grid_x, grid_y);
//
//			return;
//		}
//
//	}
//
//	public void OnHited(HeroUnit attacker)
//	{
//		if(hero_unit.IsCanSeeUnit(attacker))
//		{
//			hero_unit.SetPursueTarget(attacker);
//		}
//	}
//}
