using UnityEngine;
using System.Collections;

public class AttackEffectBase 
{
	public Actor Caster { set; get; }
	public Actor Target { set; get; }

	public virtual void Init()
	{
	}

	public virtual void Destroy()
	{
		
	}

	public virtual bool Tick(float dt)
	{
		// 删除这个effect
		return true;
	}
}
