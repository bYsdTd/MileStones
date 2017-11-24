using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace BattleProto
{
	[System.Serializable]
	public class BattleStart
	{
		public int	battle_start_time { set; get; }

		public byte[] Serialize()
		{
			byte[] result = new byte[4];

			int offset = 0;
			SerializeHelper.WriteInt(result, battle_start_time, ref offset);

			return result;
		}

		public static BattleStart Deserialize(byte[] data, ref int offset)
		{
			BattleStart obj = new BattleStart();
			obj.battle_start_time = SerializeHelper.ReadInt(data, ref offset);

			return obj;
		}

		public override string ToString ()
		{
			return string.Format ("[BattleStart: battle_start_time = {0}]", battle_start_time);
		}
	}	
}


