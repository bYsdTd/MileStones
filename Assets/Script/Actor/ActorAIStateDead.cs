using UnityEngine;
using System.Collections;

public class ActorAIStateDead : ActorAIState 
{
	public ActorAIStateDead()
	{
		state_ = Actor.State.Dead;
	}

	public override void Init (Actor parent)
	{
		base.Init (parent);

		actor.PlaySkill("dead", true);
	}

	public override void Destroy ()
	{
		base.Destroy ();
	}

	public override void Tick (float dt)
	{
		
	}
}
