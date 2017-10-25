using UnityEngine;
using System.Collections;

public class ActorAIState 
{
	public Actor.State	state_;
	protected Actor actor;

	public ActorAIState()
	{
	}

	public virtual void Init(Actor parent)
	{
		actor = parent;
	}

	public virtual void Destroy()
	{
		
	}

	public virtual void Tick(float dt)
	{


	}
}
