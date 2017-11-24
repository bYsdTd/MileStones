using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace BattleProto
{
	public class JoinRoom : IProtoSerializer
	{
		public int	room_id { set; get; }
		 
		public int Length()
		{
			return 4;
		}

		public void Serialize(byte[] buffer, ref int offset)
		{
			SerializeHelper.WriteInt(buffer, room_id, ref offset);
		}

		public override string ToString ()
		{
			return string.Format ("[JoinRoom: room_id={0}]", room_id);
		}
	}	
}
