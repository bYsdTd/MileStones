using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using BattleProto;

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

	public void JoinRoom(int room_id)
	{
//		Connect("10.0.6.156", 2000);
		Connect("127.0.0.1", 2000);

		JoinRoom packet = new JoinRoom();
		packet.room_id = room_id;
		SendPacket(RequestId.JoinRoom, packet);
	}

	public void Connect(string ip, int port)
	{
		Debug.Assert(clientSocket == null);

		clientSocket = new ClientSocket();
		clientSocket.Connect(ip, port);
	}

	public void Close()
	{
		if(clientSocket != null)
		{
			clientSocket.Close();

			clientSocket = null;
		}	
	}

	public void Tick(float dt) 
	{
		if(clientSocket != null)
		{
			clientSocket.MainThreadFunc(dt);	
		}
	}

	// 这个地方应该到最后底层再去序列化，现在这样会多一次拷贝, 注意
	public void SendPacket(short packet_id, IProtoSerializer packet)
	{
		if(clientSocket != null)
		{
			clientSocket.SendPacket(packet_id, packet);	
		}
	}
}
