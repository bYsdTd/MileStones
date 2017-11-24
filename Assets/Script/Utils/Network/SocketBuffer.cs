using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BattleProto;

public class SocketBuffer 
{
	// NonStateMsgMarker(1 byte) - Handler Id (2 byte) - Netpack Version(1 byte) - Data size(4 byte) - Data - CRC(4 byte) - NonStateMsgMarker
	public const byte PackageBreaker 	= 254;
	public const int WrapperLen 		= 13;

	public const int HandlerIdOfffset 	= 1;
	public const int DataSizeOffset 	= 4;
	public const int DataOffset 		= 8;
	public const int DataSize			= 4;

	// buffer array
	byte[] buffer = null;
	// current data offset
	public int offset = 0;
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

			offset = offset - len;
			return len;
		}
	}

	public void WrapSendPacket(short packet_id, IProtoSerializer packet)
	{
		lock(mutex)
		{
			SerializeHelper.WriteByte(buffer, SocketBuffer.PackageBreaker, ref offset);
			SerializeHelper.WriteShort(buffer, packet_id, ref offset);
			SerializeHelper.WriteByte(buffer, 0, ref offset);
			SerializeHelper.WriteInt(buffer, packet.Length(), ref offset);
			packet.Serialize(buffer, ref offset);
			SerializeHelper.WriteUInt32(buffer, 0, ref offset);
			SerializeHelper.WriteByte(buffer, SocketBuffer.PackageBreaker, ref offset);
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

			offset += len;
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

				// 先读取包的size
				int packet_size = ByteOrderConverter.NetworkToHostOrder(BitConverter.ToInt32(buffer, DataSizeOffset));

				int proto_total_len = packet_size + WrapperLen;

				if(offset < proto_total_len)
				{
					break;
				}

				// 读取包id
				int id = ByteOrderConverter.NetworkToHostOrder(BitConverter.ToInt16(buffer, HandlerIdOfffset));

				// 处理具体的包协议
				object proto_data = null;
				int proto_data_deserialize_offset = DataOffset;

				switch(id)
				{
				case ResponseId.JoinRoomResult:
					{
						proto_data = JoinRoomResult.Deserialize(buffer, ref proto_data_deserialize_offset);
					}
					break;
				case ResponseId.BattleStart:
					{
						proto_data = BattleStart.Deserialize(buffer, ref proto_data_deserialize_offset);
					}
					break;
				case ResponseId.Tick:
					{
						proto_data = TickList.Deserialize(buffer, ref proto_data_deserialize_offset);
					}
					break;

				default:
					break;
				};

				if(proto_data != null)
				{
					ProtoData  data = new ProtoData();
					data.id = id;
					data.proto = proto_data;

					proto_data_queue.Enqueue(data);
				}
					
				// 包体读取完毕
				Array.Copy(buffer, proto_total_len, buffer, 0, offset - proto_total_len);
				offset = offset - proto_total_len;
			}
			while(true);
		}
	}
}
