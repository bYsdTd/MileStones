using UnityEngine;
using System.Collections;

public class ActorRendererSprite : ActorRenderer 
{
	const float PI_81 = Mathf.PI / 8;
	const float PI_83 = Mathf.PI * 3 / 8;
	const float PI_85 = Mathf.PI * 5 / 8;
	const float PI_87 = Mathf.PI * 7 / 8;

	public override void Init (string name, Actor parent)
	{
		base.Init (name, parent);

		instance = ObjectPoolManager.Instance().GetObject(actorName);
		animator = instance.GetComponentInChildren<Animator>();
	}

	public override void Destroy ()
	{
		if(instance != null)
		{
			GameObject.Destroy(instance);
			instance = null;
		}

		base.Destroy ();
	}

	public override void Tick (float dt)
	{
		base.Tick (dt);

		CalcBillboard();
	}
		
	public override void SetPosition (Vector3 position)
	{
		base.SetPosition (position);

		instance.transform.position = position;
	}

	public override void PlaySkill (string skillName, bool playOnce)
	{
		string suffix = "";

		// 根据方向，找对应的状态，老的系统有问题，只有5个状态，自己根据需要翻转
		// 新的方式应该对应8个状态,skillname 是前缀，后缀是方向
		if(actor.Rotation > -PI_81 && actor.Rotation <= PI_81 )
		{
			suffix = "_d";
		}
		else if(actor.Rotation > PI_81 && actor.Rotation <= PI_83 )
		{
			suffix = "_rd";
		}
		else if(actor.Rotation > PI_83 && actor.Rotation <= PI_85 )
		{
			suffix = "_r";
		}
		else if(actor.Rotation > PI_85 && actor.Rotation <= PI_87 )
		{
			suffix = "_rt";
		}
		else if((actor.Rotation > PI_87)||
				(actor.Rotation <= -PI_87))
		{
			suffix = "_t";
		}
		else if(actor.Rotation > -PI_87 && actor.Rotation <= -PI_85 )
		{
			suffix = "_lt";
		}
		else if(actor.Rotation > -PI_85 && actor.Rotation <= -PI_83 )
		{
			suffix = "_l";
		}
		else if(actor.Rotation > -PI_83 && actor.Rotation <= -PI_81 )
		{
			suffix = "_ld";
		}
		else
		{
			Debug.LogWarning(actor.Rotation.ToString());
			Debug.LogWarning(skillName);
		}


		Debug.Log("suffix " + suffix);

		skillName = skillName + suffix;

		animator.Play(skillName, -1, playOnce ? 0 : float.NegativeInfinity);
	}

	public override void SetRotation (float rotation)
	{
		
	}

	private void CalcBillboard()
	{
		if(instance != null)
		{
			instance.transform.rotation = Camera.main.transform.rotation;
		}
	}
		
}
