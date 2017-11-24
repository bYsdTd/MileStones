using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProtoSerializer  
{
	int Length();
	void Serialize(byte[] buffer, ref int offset);
}
