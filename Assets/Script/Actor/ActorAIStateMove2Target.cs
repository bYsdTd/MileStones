using UnityEngine;
using System.Collections;

public class ActorAIStateMove2Target : ActorAIState 
{
	public float moveSpeed { set; get; }
	public Actor target { set; get; }

	private Vector3 _dir = Vector3.zero;

	// 从开始这个状态经过的总时间
	private float _timeStamp = 0;
	// 前一个逻辑帧
	private int _lastFrame = 0;
	// 上次更新完，距离前一个逻辑帧经过的时间
	private float _elapsedTimeSinceLastFrame = 0;

	public ActorAIStateMove2Target()
	{
		state_ = Actor.State.Move2Target;
	}

	public override void Init (Actor parent)
	{
		base.Init (parent);

		_timeStamp = 0;
		_lastFrame = 0;

		_checkDirection();
	}

	public override void Destroy ()
	{
		base.Destroy ();
	}

	// 逻辑帧跟渲染帧不同，方向的变化是在逻辑帧矫正一次向目标的方向
	private void _checkDirection()
	{
		_dir = (target.Position - actor.Position).normalized;

		actor.ChangeDirection(_dir);

		actor.PlaySkill("running");
	}

	public override void Tick (float dt)
	{
		_timeStamp += dt;
		int nowFrame = (int)(_timeStamp / GlobalConfig.TIME_PER_FRAME);
		int frameElapsed = nowFrame - _lastFrame;

		if(frameElapsed == 0)
		{
			actor.Position = actor.Position + _dir * moveSpeed * dt;
			_elapsedTimeSinceLastFrame += dt;
		}
		else if(frameElapsed > 0)
		{
			float lastFrameRemainTime = GlobalConfig.TIME_PER_FRAME - _elapsedTimeSinceLastFrame;
			actor.Position = actor.Position + _dir * moveSpeed * lastFrameRemainTime;

			--frameElapsed;

			_elapsedTimeSinceLastFrame = dt - lastFrameRemainTime - frameElapsed * GlobalConfig.TIME_PER_FRAME;

			while(frameElapsed > 0)
			{
				_checkDirection();

				actor.Position = actor.Position + _dir * moveSpeed * GlobalConfig.TIME_PER_FRAME;

				--frameElapsed;
			}

			_checkDirection();
			actor.Position = actor.Position + _dir * moveSpeed * _elapsedTimeSinceLastFrame;

		}

		_lastFrame = nowFrame;
	}
}
