using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetManager  
{
	static private NetManager instance = null;

	static public NetManager Instance()
	{
		if(instance == null)
		{
			instance = new NetManager();
		}

		return instance;
	}

	public void OnReceivePackage(BL.BLCommandBase command)
	{
		BL.BLCommandManager.Instance().AddCommand(command.cast_frame, command);

		// 这里将来应该是跟服务器同步的地方
		// 应该是收到某一帧所有的指令
		// 然后需要驱动逻辑层的播放，比如收到对应的帧，才驱动逻辑层更新
	}
}
