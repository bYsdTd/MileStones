using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

namespace BattleProto
{
	[System.Serializable]
	public class TickList
	{
		public int	frame { set; get; }
		public int	size { set; get; }

		public List<Tick> list = new List<Tick>();

//		public int Size()
//		{
//			int size = 8;
//
//			for(int i = 0; i < list.Count; ++i)
//			{
//				size += list[i].Size();
//			}
//
//			return size;
//		}
//
//		public byte[] Serialize()
//		{
//			byte[] result = new byte[Size()];
//
//			int offset = 0;
//			SerializeHelper.WriteInt(result, frame, ref offset);
//			SerializeHelper.WriteInt(result, size, ref offset);
//
//			for(int i = 0; i < list.Count; ++i)
//			{
//				// data
//				byte[] data = list[i].Serialize();
//				Array.Copy(data, 0, result, offset, data.Length);
//				offset += data.Length;
//			}
//
//			return result;
//		}

		public static TickList Deserialize(byte[] data, ref int offset)
		{
			TickList obj = new TickList();
			obj.frame = SerializeHelper.ReadInt(data, ref offset);

			obj.size = SerializeHelper.ReadInt(data, ref offset);

//			if(obj.size > 0)
//			{
//				Debug.Log("收到网络包的帧： " + obj.frame);
//				Debug.LogError("收到操作");
//			}

			for(int i = 0; i < obj.size; ++i)
			{
				Tick tick = Tick.Deserialize(data, ref offset);
				obj.list.Add(tick);
			}

			return obj;
		}
	}

}

