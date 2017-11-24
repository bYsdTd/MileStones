using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class SerializeHelper 
{
	public static byte ReadByte(byte[] buf, ref int offset)
	{
		return buf[offset++];
	}

	public static int ReadInt(byte[] buf, ref int offset)
	{
		int val = ByteOrderConverter.NetworkToHostOrder(BitConverter.ToInt32(buf, offset));
		offset += 4;
		return val;
	}

	public static short ReadShort(byte[] buf, ref int offset)
	{
		short val = ByteOrderConverter.NetworkToHostOrder(BitConverter.ToInt16(buf, offset));
		offset += 2;
		return val;
	}

	public static UInt32 ReadUInt32(byte[] buf, ref int offset)
	{
		UInt32 val = ByteOrderConverter.NetworkToHostOrder(BitConverter.ToUInt32(buf, offset));
		offset += 4;
		return val;
	}

	public static int WriteByte(byte[] buf, byte val, ref int offset)
	{
		buf[offset++] = val;

		return 1;
	}

	public static int WriteShort(byte[] buf, short val, ref int offset)
	{
		byte[] bytes = BitConverter.GetBytes(ByteOrderConverter.HostToNetworkOrder(val));
		Array.Copy(bytes, 0, buf, offset, bytes.Length);
		offset += bytes.Length;

		return bytes.Length;
	}

	public static int WriteInt(byte[] buf, int val, ref int offset)
	{
		byte[] bytes = BitConverter.GetBytes(ByteOrderConverter.HostToNetworkOrder(val));
		Array.Copy(bytes, 0, buf, offset, bytes.Length);
		offset += bytes.Length;

		return bytes.Length;
	}

	public static int WriteUInt32(byte[] buf, UInt32 val, ref int offset)
	{
		byte[] bytes = BitConverter.GetBytes(ByteOrderConverter.HostToNetworkOrder(val));
		Array.Copy(bytes, 0, buf, offset, bytes.Length);
		offset += bytes.Length;

		return bytes.Length;
	}
}
