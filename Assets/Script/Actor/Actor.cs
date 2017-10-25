using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Actor 
{
	public enum ActorType
	{
		Sprite = 1,
		Skin = 2,
	}

	public enum State
	{
		Idle,
		Move2Position,
		Move2Target,
		AgentMove,
		Attack,
		Dead,
	}

	ActorRenderer renderer;
	ActorAIState state;

	public List<AttackEffectBase> attackEffectList = new List<AttackEffectBase>();

	public int ID { set; get; }

	public void Init(string resName, ActorType type, int id)
	{
		ID = id;

		if(type == ActorType.Sprite)
		{
			renderer =  new ActorRendererSprite();	
		}
		else if(type == ActorType.Skin)
		{
			renderer = new ActorRendererSkin();
		}

		renderer.Init(resName, this);
		renderer.AfterInit();
	}

	public void Destroy()
	{
		if(renderer != null)
		{
			renderer.BeforeDestroy();

			renderer.Destroy();
			renderer = null;
		}
	}

	public void AddChild(GameObject obj)
	{
		if(obj != null && renderer != null && renderer.instance != null)
		{
			obj.transform.SetParent(renderer.instance.transform, false);
		}	
	}

	public GameObject GetGameObject()
	{
		if(renderer != null)
		{
			return renderer.instance;
		}	
		else
		{
			return null;
		}
	}

	public void Tick(float dt)
	{
		if(state != null)
		{
			state.Tick(dt);	
		}

		if(renderer != null)
		{
			renderer.Tick(dt);
		}

		// 攻击效果的tick，效果结束要删除
		for(int i = attackEffectList.Count-1;i >= 0 ; i--)
		{
			AttackEffectBase attackEffect = attackEffectList[i];

			if(attackEffect.Tick(dt))
			{
				attackEffect.Destroy();
				// delete
				attackEffectList.RemoveAt(i);
			}
		}
	}

	public void SetState(ActorAIState newState)
	{
		if(state != null)
		{
			state.Destroy();

			state = null;
		}

		state = newState;
		state.Init(this);
	}

	public void PlaySkill(string skillName, bool playOnce = false)
	{
		if(renderer != null)
		{
			renderer.PlaySkill(skillName, playOnce);
		}
	}
		
	Vector3 position;
	public Vector3 Position
	{ 
		get
		{
			return position;
		} 
		set
		{
			if(value != position)
			{
				position = value;

				if(renderer != null)
				{
					renderer.SetPosition(position);
				}
			}
		}

	}

	public void ChangeDirection(Vector3 direction)
	{
		Vector3 tmp = direction;
		tmp.y = 0;
		tmp.Normalize();

		float cosTheta = Vector3.Dot(tmp, Vector3.forward);
		float theta = Mathf.Acos(cosTheta);

		if(tmp.x <= 0)
		{
			theta = -theta;	
		}

		Rotation = theta;

		Debug.Log(" Rotation " + Mathf.Rad2Deg * Rotation);

	}

	// 绕y轴顺时针的转角
	float rotation;
	public float Rotation
	{
		get
		{
			return rotation;
		}

		set
		{
			if(value != rotation)
			{
				rotation = value;

				if(renderer != null)
				{
					renderer.SetRotation(rotation);
				}
			}
		}
	}

	// unit attribute id
	int attack_skill_attack_id_;
	public int AttackSkillAttackId
	{
		get
		{
			return attack_skill_attack_id_;
		}
		set
		{
			if(value != attack_skill_attack_id_)
			{
				attack_skill_attack_id_ = value;
			}
		}
	}

	// 设置state相关接口
	public void Move2Position(Vector3 start, Vector3 end, float speed)
	{
		ActorAIStateMove2Position state = new ActorAIStateMove2Position();
		state.startPos = start;
		state.endPos = end;
		state.moveSpeed = speed;

		SetState(state);
	}

	public void Move2Target(Actor target, float speed)
	{
		ActorAIStateMove2Target state = new ActorAIStateMove2Target();
		state.moveSpeed = speed;
		state.target = target;

		SetState(state);
	}

	public void AgentMove()
	{
		ActorAIStateAgentMove state = new ActorAIStateAgentMove();

		SetState(state);
	}

	public void Attack(Vector3 position, Actor target)
	{
		ActorCallbackData attackCallback = new ActorCallbackData();
		attackCallback.Caster = this;
		attackCallback.Target = target;
		attackCallback.Init();

		ActorAIStateAttack state = new ActorAIStateAttack();
		state.callbackData = attackCallback;
		state.Position = position;

		SetState(state);
	}

	public void Dead()
	{
		ActorAIStateDead state = new ActorAIStateDead();

		SetState(state);
	}

	public void Idle()
	{
		ActorAIStateIdle state = new ActorAIStateIdle();

		SetState(state);
	}
}
