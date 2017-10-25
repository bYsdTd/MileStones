using UnityEngine;
using System.Collections;

public class AttackEffectHit : AttackEffectBase 
{
	public string EffectPrefabName { set; get; }

	GameObject _instance = null;
	Animator animator = null;

	float _time = 0;

	public override void Init ()
	{
		_instance = ObjectPoolManager.Instance().GetObject("Effect/" + EffectPrefabName);

		if(Target != null)
		{
			Target.AddChild(_instance);
		}

		animator = _instance.GetComponentInChildren<Animator>();

		animator.Play("hit");

		AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);

		_time = clipInfo[0].clip.length;
	}

	public override void Destroy ()
	{
		if(_instance != null)
		{
			GameObject.Destroy(_instance);
			_instance = null;
		}
	}

	public override bool Tick (float dt)
	{
		if(_time > 0 )
		{
			_time -= dt;

			return false;
		}
		else
		{
			return true;
		}
	}
}
