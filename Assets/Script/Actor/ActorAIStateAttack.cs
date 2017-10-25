using UnityEngine;
using System.Collections;

public class ActorAIStateAttack : ActorAIState 
{
	public Vector3 Position { set; get;}

	public ActorCallbackData callbackData = null;

	private float forward_time = 0;

	private int attack_interval = 8;

	public ActorAIStateAttack()
	{
		state_ = Actor.State.Attack;
	}

	public override void Init (Actor parent)
	{
		//Debug.Log("Renderer Attack at frame: " + Launch.battleplayer._battle.Frame + " time : " + System.Environment.TickCount );

		base.Init (parent);

		var data = SkillAttackFlyAttributeConfig.GetSkillAttackFlyAttribute(actor.AttackSkillAttackId);
		forward_time = data.ForwardFrame * GlobalConfig.TIME_PER_FRAME;

		actor.Position = Position;

		actor.ChangeDirection(callbackData.Target.Position - callbackData.Caster.Position);


		TimerManager.Instance().RepeatCallFunc(delegate(float dt) {

			actor.PlaySkill("attack", true);

			TimerManager.Instance().DelayCallFunc(delegate(float delta_time) {

				_onHitCallBack();

			}, forward_time);

		}, attack_interval * GlobalConfig.TIME_PER_FRAME);

	}

	public override void Destroy ()
	{
		// 有的时候状态被打断，不会触发回调，这里强制处理一下
		_onHitCallBack();

		base.Destroy ();
	}

	private void _onHitCallBack()
	{
		// callback data 保证只触发一次, 要么是在回调的时候，要么是在被打断的时候在destory
		if(callbackData != null && callbackData.Caster != null && callbackData.Target != null)
		{
			//Debug.Log("Renderer Forward at frame: " + Launch.battleplayer._battle.Frame + " time : " + System.Environment.TickCount );

			var data = SkillAttackFlyAttributeConfig.GetSkillAttackFlyAttribute(callbackData.Caster.AttackSkillAttackId);


			if(!data.IsFly)
			{
				// 只有击中
				AttackEffectHit hitEffect = new AttackEffectHit();

				hitEffect.EffectPrefabName = data.HitEffectName;
				hitEffect.Caster = callbackData.Caster;
				hitEffect.Target = callbackData.Target;

				hitEffect.Init();

				callbackData.Target.attackEffectList.Add(hitEffect);
			}
			else
			{
				// 飞行击中
				AttackEffectStraightLine attackEffect = new AttackEffectStraightLine();
				attackEffect.HitEffectPrefabName = data.HitEffectName;
				attackEffect.FlyEffectPrefabName = data.FlyEffectName;

				attackEffect.Caster = callbackData.Caster;
				attackEffect.Target = callbackData.Target;

				attackEffect.Speed = data.Speed;

				attackEffect.Init();
				callbackData.Target.attackEffectList.Add(attackEffect);
			}
		}
	}

	public override void Tick (float dt)
	{
//		if(forward_time > 0)
//		{
//			forward_time -= dt;
//		}
//		else
//		{
//			_onHitCallBack();
//		}
	}
}
