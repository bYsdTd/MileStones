using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using BattleProto;

public class ClientSocket 
{
	const int MAX_BUFFER_SIZE = 32768;

	Socket socket = null;
	Thread socketThread = null;

	string m_host = "";
	int m_port = 0;

	SocketBuffer receiveBuffer = new SocketBuffer(MAX_BUFFER_SIZE);
	SocketBuffer sendBuffer = new SocketBuffer(MAX_BUFFER_SIZE);

	// main thread set, socket thread get
	public bool mainThreadCloseSocket { set; get; }

	public ClientSocket()
	{
		socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

	}

	public void Connect(string host, int port)
	{
		m_host = host;
		m_port = port;

		mainThreadCloseSocket = false;

		socketThread = new Thread(ThreadFunc);
		socketThread.Start();
	}

	public void Close()
	{
		mainThreadCloseSocket = true;
	}

	void ThreadFunc()
	{
		Debug.Log("socket thread start");

		do
		{
			// connect
			try
			{
				socket.Connect(m_host, m_port);
			}
			catch
			{
				Debug.Log("socket connect failed");
				break;
			}

			// receive packet
			while(true)
			{
				if(mainThreadCloseSocket)
				{
					break;
				}

				bool ret = socket.Poll(10, SelectMode.SelectRead);
				if(ret)
				{
					if(socket.Available == 0)
					{
						Debug.Log("server disconnect the socket");
						break;
					}
					else
					{
						byte[] receive = new byte[socket.Available];
						int len = socket.Receive(receive);

						receiveBuffer.Write(receive, len);

						receiveBuffer.HandleReceive();
					}
				}

				// send packet
				if(sendBuffer.offset > 0)
				{	
					//Debug.LogError("发送网络包到缓冲区");

					byte[] buffer = new byte[sendBuffer.offset];
					
					sendBuffer.Read(ref buffer, (int)sendBuffer.offset);

					socket.Send(buffer);
				}
			}


		}while(false);

		Debug.Log("socket thread end");
	}

	public void MainThreadFunc(float delta_time)
	{
		// called every frame
		ProtoData proto_data = receiveBuffer.DequeueProtoData();
		while( proto_data != null )
		{
			//Debug.Log("收到消息包 id : " + proto_data.id);

			switch(proto_data.id)
			{
			case ResponseId.JoinRoomResult:
				{
					JoinRoomResult result = proto_data.proto as JoinRoomResult;

					// 逻辑层更新
					BL.BLTimelineController.Instance().OnJoinRoom(result.team_id);
					// 表现层更新
					BattleField.battle_field.OnJoinRoom(result.team_id, result.team_id);
				}
				break;
			case ResponseId.BattleStart:
				{
					BattleStart result = proto_data.proto as BattleStart;

					BL.BLTimelineController.Instance().Start();
					BattleFieldInputHandle.Instance().enabled = true;
				}
				break;
			case ResponseId.Tick:
				{
					TickList tick_list = proto_data.proto as TickList;
					//Debug.Log("收到同步帧 " + tick_list.frame);

					// 先把指令放入队列
					for(int i = 0; i < tick_list.list.Count; ++i)
					{
						Tick tick = tick_list.list[i];

						switch(tick.command_type)
						{
						case BL.TickCommandType.Move:
							{
								BL.BLIntVector3 dest_position;
								dest_position.x = tick.x;
								dest_position.y = 0;
								dest_position.z = tick.y;

								BL.BLCommandBase command = BL.BLCommandManager.Instance().CreateMove2PositionCommand(tick.cast_id, tick_list.frame, dest_position);

								BL.BLCommandManager.Instance().AddCommand(command.cast_frame, command);
							}
							break;
						case BL.TickCommandType.PUT_HERO:
							{
								BL.BLCommandBase command = BL.BLCommandManager.Instance().CreatePutHeroCommand(tick.team_id, tick_list.frame, tick.hero_index);

								BL.BLCommandManager.Instance().AddCommand(command.cast_frame, command);
							}
							break;
						case BL.TickCommandType.PURSUE_TARGET:
							{

							}
							break;
						};
					}

					// 时间轴控制进行一帧, 分发指令
					BL.BLTimelineController.Instance().frame_received++;
				
				}
				break;
			default:
				break;
			};

			proto_data = receiveBuffer.DequeueProtoData();
		}
	}

	// 这里处理整理协议包的内容, 目前为了兼容后端
	public void SendPacket(short packet_id, IProtoSerializer senddata)
	{
		sendBuffer.WrapSendPacket(packet_id, senddata);
	}
}
