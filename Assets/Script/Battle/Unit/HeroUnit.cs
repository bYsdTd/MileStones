using UnityEngine;
using System.Collections;

public class HeroUnit : MonoBehaviour 
{
	public Animator animator;

	// 每秒0.5格
	public int unit_id = -1;
	public float move_speed = 0.5f;
	public Vector3 _position;


	private Transform cache_transform;

	// Use this for initialization
	void Start () 
	{
	}
	
	public void Init()
	{
		cache_transform = gameObject.transform;	
	}

	public void Tick(float delta_time)
	{
		
	}

	public void SetPosition(Vector3 position)
	{
		if(position != _position)
		{
			_position = position;

			cache_transform.position = position;	
		}
	}

	public void PlayMove()
	{
		animator.SetBool("Moving", true);
		animator.SetBool("Running", true);
	}

	public void PlayIdle()
	{
		animator.SetBool("Moving", false);
		animator.SetBool("Running", false);
	}

	public void PlayAttack()
	{
		animator.SetTrigger("Attack1Trigger");
	}
}
