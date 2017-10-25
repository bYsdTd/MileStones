using UnityEngine;
using System.Collections;

public class ActorRenderer 
{
	protected string actorName;
	protected Actor actor;
	protected Animator animator;
	public GameObject instance;

	virtual public void Init(string name, Actor parent)
	{
		actorName = name;
		actor = parent;
	}

	public void AfterInit()
	{
		ActorMananger.Instance().RegisterMonoBehaviour(animator.gameObject, actor);
	}

	public void BeforeDestroy()
	{
		ActorMananger.Instance().UnRgisterMonoBehaviour(animator.gameObject, actor);
	}

	virtual public void Destroy()
	{
		
	}

	virtual public void Tick(float dt)
	{
		
	}

	virtual public void SetPosition(Vector3 position)
	{
		
	}

	virtual public void PlaySkill(string skillName, bool playOnce)
	{
		
	}

	virtual public void SetRotation(float rotation)
	{
		
	}
}
