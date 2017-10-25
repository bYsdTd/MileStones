using UnityEngine;
using System.Collections;

public class ActorAIStateMove2Position : ActorAIState 
{
	public Vector3 startPos { set; get; }
	public Vector3 endPos { set; get; }
	public float moveSpeed { set; get; }

	private float timeStamp = 0;
	private float totalTime = 0;
	private Vector3 dir = Vector3.zero;

	public ActorAIStateMove2Position()
	{
		state_ = Actor.State.Move2Position;
	}

	public override void Init (Actor parent)
	{
		base.Init (parent);

		timeStamp = 0;

		dir = (endPos - startPos).normalized;
		totalTime = (endPos - startPos).magnitude / moveSpeed;

		actor.ChangeDirection(dir);

		actor.PlaySkill("running");
	}

	public override void Destroy ()
	{
		base.Destroy ();
	}

	public override void Tick (float dt)
	{
		if(timeStamp >= totalTime)
		{
			actor.Position = endPos;

			actor.Idle();

			return;
		}
		else
		{
			timeStamp += dt;

			actor.Position = startPos + dir * moveSpeed * timeStamp;
		}
	}
}
