using UnityEngine;
using System.Collections;

public class CommandBase  
{
	public CommandType	_type = CommandType.None;

	public int 	unit_id = -1;
//	public int 	start_frame = -1;
//	public int	end_frame = -1;

	public int	current_frame = -1;
	public float time_elapsed = 0;

	public HeroUnit hero_unit = null;

	public virtual void OnStart()
	{
		time_elapsed = 0;
	}

	// 返回true表示本条指令执行完了
	public virtual bool Tick(float delta_time)
	{
		return false;
	}

	public virtual void OnEnd()
	{
		
	}
}
