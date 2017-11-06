using UnityEngine;
using System.Collections;

public class Random 
{
	static System.Random random = new System.Random();

	public static int Rand(int min, int max)
	{
		return random.Next(min, max);
	}

	public static float Rand()
	{
		return (float)random.NextDouble();
	}

}
