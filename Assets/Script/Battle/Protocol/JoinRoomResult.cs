using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class JoinRoomResult
{
	public int	team_id { set; get; }

	public byte[] Serialize()
	{
		MemoryStream stream = new MemoryStream();
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(stream , this);

		byte[] data = stream.ToArray();
		stream.Close( );

		return data;
	}

	public static JoinRoom Deserialize(byte[] data)
	{
		MemoryStream stream = new MemoryStream();
		stream.Write(data , 0 , data.Length);
		stream.Position = 0;

		BinaryFormatter bf = new BinaryFormatter();
		JoinRoom obj = bf.Deserialize(stream) as JoinRoom;
		stream.Close();

		return obj;
	}

	public override string ToString ()
	{
		return string.Format ("[JoinRoom: room_id={0}]", team_id);
	}
}
