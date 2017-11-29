using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace BattleProto
{
	[System.Serializable]
	public class Tick : IProtoSerializer
	{
		public int		team_id { set; get; }
		public int		command_type { set; get; }

		// 下面的字段都是可选的项，根据上面的操作类型来定义
		// 这个是move的字段
		public int		cast_id { set; get; }
		public int		x { set; get; }
		public int 		y { set; get; }
		// 放置将军字段
		public int		hero_index { set; get; }
		// 追击目标(用上面的cast id)
		public int		target_id { set; get; }

		public int	Length()
		{
			return 20;
		}

		public void Serialize(byte[] buffer, ref int offset)
		{
			SerializeHelper.WriteInt(buffer, team_id, ref offset);
			SerializeHelper.WriteInt(buffer, command_type, ref offset);

			switch(command_type)
			{
			case BL.TickCommandType.Move:
				{
					SerializeHelper.WriteInt(buffer, cast_id, ref offset);
					SerializeHelper.WriteInt(buffer, x, ref offset);
					SerializeHelper.WriteInt(buffer, y, ref offset);
				}
				break;
			case BL.TickCommandType.PUT_HERO:
				{
					// 暂时都填充满20个字节
					SerializeHelper.WriteInt(buffer, hero_index, ref offset);
					SerializeHelper.WriteInt(buffer, hero_index, ref offset);
					SerializeHelper.WriteInt(buffer, hero_index, ref offset);
				}
				break;
			case BL.TickCommandType.PURSUE_TARGET:
				{
					// 暂时都填充满20个字节
					SerializeHelper.WriteInt(buffer, cast_id, ref offset);
					SerializeHelper.WriteInt(buffer, target_id, ref offset);
					SerializeHelper.WriteInt(buffer, target_id, ref offset);
				}
				break;
			};

		}

		public static Tick Deserialize(byte[] data, ref int offset)
		{
			Tick obj = new Tick();

			obj.team_id = SerializeHelper.ReadInt(data, ref offset);
			obj.command_type = SerializeHelper.ReadInt(data, ref offset);

			switch(obj.command_type)
			{
			case BL.TickCommandType.Move:
				{
					obj.cast_id = SerializeHelper.ReadInt(data, ref offset);
					obj.x = SerializeHelper.ReadInt(data, ref offset);
					obj.y = SerializeHelper.ReadInt(data, ref offset);
				}
				break;
			case BL.TickCommandType.PUT_HERO:
				{
					obj.hero_index = SerializeHelper.ReadInt(data, ref offset);
					SerializeHelper.ReadInt(data, ref offset);
					SerializeHelper.ReadInt(data, ref offset);
				}
				break;
			case BL.TickCommandType.PURSUE_TARGET:
				{
					obj.cast_id = SerializeHelper.ReadInt(data, ref offset);
					obj.target_id = SerializeHelper.ReadInt(data, ref offset);
					SerializeHelper.ReadInt(data, ref offset);
				}
				break;
			};

			return obj;
		}
	}
}

