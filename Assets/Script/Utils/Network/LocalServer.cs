using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalServer 
{
	static private LocalServer instance = null;

	static public LocalServer Instance()
	{
		if(instance == null)
		{
			instance = new LocalServer();
		}

		return instance;
	}

	public void SendPackeage(BL.BLCommandBase command)
	{
		command.cast_frame = BL.BLTimelineController.Instance().current_frame;

		NetManager.Instance().OnReceivePackage(command);
	}

}
