using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace BattleProto
{
	
	[System.Serializable]
	public class JoinRoomResult
	{
		public int	team_id { set; get; }

		public byte[] Serialize()
		{
			byte[] result = new byte[4];

			int offset = 0;
			SerializeHelper.WriteInt(result, team_id, ref offset);

			return result;
		}

		public static JoinRoomResult Deserialize(byte[] data, ref int offset)
		{
			JoinRoomResult obj = new JoinRoomResult();
			obj.team_id = SerializeHelper.ReadInt(data, ref offset);

			return obj;
		}

		public override string ToString ()
		{
			return string.Format ("[JoinRoom: team_id={0}]", team_id);
		}
	}

}

