using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitConfig  
{
	public class UnitAttribute
	{
		public int ID { set; get; }
		public int HP { set; get; }
		public int AttackRange { set; get;}
		public int Attack { set; get; }
		// 几帧攻击一次
		public int AttackSpeed { set; get; }
		public float MoveSpeed { set; get; }

		// attack attid
		public int AttackSkillAttackID { set; get; }
	}

	public static Dictionary<int , UnitAttribute> unit_attribute_config_list = new Dictionary<int, UnitAttribute>();

	public static void Initialize()
	{
		UnitAttribute data = new UnitAttribute();
		data.ID = 1;
		data.HP = 100;
		data.AttackRange = 1;
		data.Attack = 10;
		data.AttackSpeed = 60;
		data.MoveSpeed = 2.0f;
		data.AttackSkillAttackID = 1;

		unit_attribute_config_list.Add(data.ID, data);

		data = new UnitAttribute();
		data.ID = 2;
		data.HP = 100;
		data.AttackRange = 7;
		data.Attack = 2;
		data.AttackSpeed = 6;
		data.MoveSpeed = 2.0f;
		data.AttackSkillAttackID = 2;

		unit_attribute_config_list.Add(data.ID, data);

	}

	public static UnitAttribute GetUnitAttribute(int id)
	{
		return unit_attribute_config_list[id];
	}
}
