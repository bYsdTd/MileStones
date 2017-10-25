using UnityEngine;
using System.Collections;

public class ActorAIStateAgentMove : ActorAIState 
{
	private NavMeshAgent agent;

	private Vector3 dir;

	public ActorAIStateAgentMove()
	{
		state_ = Actor.State.AgentMove;
	}

	public override void Init (Actor parent)
	{
		base.Init (parent);

		agent = actor.GetGameObject().GetComponentInChildren<NavMeshAgent>();

		CheckDirection();

	}

	public override void Destroy ()
	{
		base.Destroy ();
	}

	private void CheckDirection()
	{
		if(agent != null)
		{
			//Debug.Log("agent.destination " + agent.destination);
			//Debug.Log("actor.Position " + actor.Position);

			dir = agent.destination - actor.Position;
			dir.y = 0;

			dir = dir.normalized;

			Debug.Log("dir  " + dir);

			actor.ChangeDirection(dir);

			actor.PlaySkill("running");
		}
	}

	public override void Tick (float dt)
	{
		if(agent != null)
		{

			if (!agent.pathPending)
			{
				if (agent.remainingDistance <= agent.stoppingDistance)
				{
					if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
					{
						// Done
						actor.Idle();
					}
				}
			}
		}
	}
}
