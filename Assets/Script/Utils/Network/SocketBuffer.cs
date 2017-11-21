using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SocketBuffer 
{
	// NonStateMsgMarker(1 byte) - Handler Id (2 byte) - Netpack Version(1 byte) - Data size(4 byte) - Data - CRC(4 byte) - NonStateMsgMarker
	public const byte PackageBreaker 	= 254;
	public const int WrapperLen 		= 13;

	public const int HandlerIdOfffset 	= 1;
	public const int DataSizeOffset 	= 4;
	public const int DataSize			= 4;

	// buffer array
	byte[] buffer = null;
	// current data offset
	public uint offset = 0;
	int totalSize = 0;

	object mutex = new object();

	Queue<ProtoData>	proto_data_queue = new Queue<ProtoData>();

	public SocketBuffer(int size)
	{
		totalSize = size;
		buffer = new byte[size];
		offset = 0;
	}

	public int Read(ref byte[] dest, int len)
	{
		lock(mutex)
		{
			if(len > dest.Length)
			{
				len = dest.Length;
			}

			if(len > offset)
			{
				Debug.Log("socket buffer read error");
				return 0;
			}

			Array.Copy(buffer, 0, dest, 0, len);
			Array.Copy(buffer, len, buffer, 0, offset - len);

			offset = offset - (uint)len;
			return len;
		}
	}

	public void Write(byte[] src, int len)
	{
		lock(mutex)
		{
			if(len + offset > totalSize)
			{
				Debug.Log("socket buffer write error");

				return;
			}

			Array.Copy(src, 0, buffer, offset, len);

			offset += (uint)len;
		}
	}

	public ProtoData DequeueProtoData()
	{
		lock(mutex)
		{
			if(proto_data_queue.Count > 0)
			{
				return proto_data_queue.Dequeue();
			}
			else
			{
				return null;
			}
		}
	}

	public void HandleReceive()
	{
		lock(mutex)
		{
			do
			{
				if(offset < DataSizeOffset + DataSize)
				{
					break;
				}

				int packet_size = BitConverter.ToInt32(buffer, DataSizeOffset);

				int len = packet_size + WrapperLen;

				if(offset < len)
				{
					break;
				}

				int id = BitConverter.ToInt16(buffer, HandlerIdOfffset);

				// 包头 读取完毕
				Array.Copy(buffer, WrapperLen, buffer, 0, offset - WrapperLen);
				offset = offset - (uint)WrapperLen;

				object proto_data = null;


				if(id == 2)
				{
					proto_data = JoinRoomResult.Deserialize(buffer);
				}
				else 
				{
					
				}

				if(proto_data != null)
				{
					ProtoData  data = new ProtoData();
					data.id = id;
					data.proto = proto_data;

					proto_data_queue.Enqueue(data);
				}

				// 包体读取完毕
				Array.Copy(buffer, packet_size, buffer, 0, offset - packet_size);
				offset = offset - (uint)packet_size;
			}
			while(false);
		}
	}
}
