using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

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


	ClientSocket clientSocket = null;

	public void Init()
	{
		Connect("10.0.6.156", 2000);

		JoinRoom packet = new JoinRoom();
		packet.room_id = 147;
		SendPacket(1, packet.Serialize());
	}

	public void Connect(string ip, int port)
	{
		Debug.Assert(clientSocket == null);

		clientSocket = new ClientSocket();
		clientSocket.Connect(ip, port);
	}

	public void Tick(float dt) 
	{
		// check network mannager
		clientSocket.MainThreadFunc();
	}

	public void SendPacket(short packet_id, byte[] packet)
	{
		clientSocket.SendPacket(packet_id, packet);
	}

	public void OnReceivePackage(BL.BLCommandBase command)
	{

		BL.BLCommandManager.Instance().AddCommand(command.cast_frame, command);

		// 这里将来应该是跟服务器同步的地方
		// 应该是收到某一帧所有的指令
		// 然后需要驱动逻辑层的播放，比如收到对应的帧，才驱动逻辑层更新
	}
}
