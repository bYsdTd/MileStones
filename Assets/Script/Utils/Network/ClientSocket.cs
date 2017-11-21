using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class ClientSocket 
{
	const int MAX_BUFFER_SIZE = 32768;

	Socket socket = null;
	Thread socketThread = null;

	string m_host = "";
	int m_port = 0;

	SocketBuffer receiveBuffer = new SocketBuffer(MAX_BUFFER_SIZE);
	SocketBuffer sendBuffer = new SocketBuffer(MAX_BUFFER_SIZE);

	public ClientSocket()
	{
		socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

	}

	public void Connect(string host, int port)
	{
		m_host = host;
		m_port = port;

		socketThread = new Thread(ThreadFunc);
		socketThread.Start();
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
					byte[] buffer = new byte[sendBuffer.offset];
					
					sendBuffer.Read(ref buffer, (int)sendBuffer.offset);

					socket.Send(buffer);
				}
			}


		}while(false);

		Debug.Log("socket thread end");
	}

	public void MainThreadFunc()
	{
		// called every frame
		ProtoData proto_data = receiveBuffer.DequeueProtoData();
		if( proto_data != null )
		{
			Debug.Log("收到消息包 id : " + proto_data.id);
		}
	}

	// 这里处理整理协议包的内容, 目前为了兼容后端
	public void SendPacket(short packet_id, byte[] senddata)
	{
		sendBuffer.WriteByte(SocketBuffer.PackageBreaker);
		sendBuffer.WriteShort(packet_id);
		sendBuffer.WriteByte(0);
		sendBuffer.WriteInt(senddata.Length);
		sendBuffer.Write(senddata, senddata.Length);
		sendBuffer.WriteUInt32(0);
		sendBuffer.WriteByte(SocketBuffer.PackageBreaker);
	}
}
