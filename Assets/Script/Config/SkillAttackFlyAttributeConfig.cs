using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillAttackFlyAttributeConfig 
{
	public class SkillAttackFlyAttribute
	{
		public int ID { set; get; }

		public bool IsFly { set; get; }

		// 0, 无飞行
		// 1, 直线
		public int FlyType { set; get; }

		public float Speed { set; get; }

		// 前摇时间
		public int ForwardFrame { set; get; }

		public string HitEffectName { set; get; }

		public string FlyEffectName { set; get; }
	}

	private static Dictionary<int , SkillAttackFlyAttribute> attack_effect_config_list_ = new Dictionary<int, SkillAttackFlyAttribute>();

	public static void Initialize()
	{
		SkillAttackFlyAttribute data = new SkillAttackFlyAttribute();
		data.ID = 1;
		data.IsFly = false;
		data.Speed = 0;
		data.ForwardFrame = 14;
		data.HitEffectName = "beiji_ren_ani";
		data.FlyEffectName = "";
		data.FlyType = 0;

		attack_effect_config_list_.Add(data.ID, data);


		data = new SkillAttackFlyAttribute();
		data.ID = 2;
		data.IsFly = true;
		data.Speed = 50;
		data.ForwardFrame = 1;
		data.HitEffectName = "beiji_ren_ani";
		data.FlyEffectName = "arrow";
		data.FlyType = 1;

		attack_effect_config_list_.Add(data.ID, data);


	}

	public static SkillAttackFlyAttribute GetSkillAttackFlyAttribute(int id)
	{
		return attack_effect_config_list_[id];
	}
}
