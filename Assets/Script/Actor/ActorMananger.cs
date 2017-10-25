using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActorMananger  
{
	public static float ACTOR_Y = 1f;

	static ActorMananger instance = null;

	List<Actor> actorList = new List<Actor>();

	Dictionary<GameObject, Actor> monoBehaviourMap = new Dictionary<GameObject, Actor>();

	public Actor CreateActor(int id, int x, int y)
	{
		Actor actor = new Actor();
		actor.AttackSkillAttackId = 2;
		actor.Init("Actor/Sprite/Prefab/swordman_r_h", Actor.ActorType.Sprite, id);
		actorList.Add(actor);

		actor.Position = new Vector3(x, ActorMananger.ACTOR_Y, y);

		return actor;
	}

	public static ActorMananger Instance()
	{
		if(instance == null)
		{
			instance = new ActorMananger();
		}

		return instance;
	}

	public void Tick(float dt)
	{
		List<Actor>.Enumerator enumarator = actorList.GetEnumerator();

		while(enumarator.MoveNext())
		{
			enumarator.Current.Tick(dt);
		}
	}

	// 由于动作才用统一的回调处理，所以需要一个mono和actor的映射关系
	public void RegisterMonoBehaviour(GameObject obj, Actor actor)
	{
		monoBehaviourMap[obj] = actor;
	}

	public void UnRgisterMonoBehaviour(GameObject obj, Actor actor)
	{
		monoBehaviourMap.Remove(obj);
	}

	public Actor GetActorMonoBehaviour(GameObject obj)
	{
		return monoBehaviourMap[obj];
	}
}
