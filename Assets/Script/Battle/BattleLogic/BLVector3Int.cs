using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
	public struct BLIntVector3
	{
		public int x;
		public int y;
		public int z;

		public BLIntVector3(int x_, int y_, int z_)
		{
			x = x_;
			y = y_;
			z = z_;
		}

		public Vector3 Vector3Value()
		{
			return new Vector3(x * 0.001f, y * 0.001f, z * 0.001f);
		}

		public Vector3 Normalize()
		{
			return new Vector3(x * 1.0f /Magnitude(), y * 1.0f /Magnitude(), z * 1.0f /Magnitude());
		}

		public int Magnitude()
		{
			return (int)Mathf.Sqrt(x*x + y*y + z*z);
		}

		public static int DistanceSqr (BLIntVector3 a, BLIntVector3 b)
		{
			BLIntVector3 tmp = a - b;

			return tmp.x * tmp.x + tmp.y * tmp.y + tmp.z * tmp.z;
		}

		public static bool operator == (BLIntVector3 lhs, BLIntVector3 rhs)
		{
			return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
		}

		public static bool operator != (BLIntVector3 lhs, BLIntVector3 rhs)
		{
			return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z;
		}

		public static BLIntVector3 operator - (BLIntVector3 a, BLIntVector3 b)
		{
			return new BLIntVector3(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public static BLIntVector3 operator + (BLIntVector3 a, BLIntVector3 b)
		{
			return new BLIntVector3(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		public static BLIntVector3 operator * (int d, BLIntVector3 a)
		{
			return new BLIntVector3(a.x * d, a.y * d, a.z * d);
		}

		public static Vector3 operator * (float d, BLIntVector3 a)
		{
			return new Vector3(a.x * d, a.y * d, a.z * d);
		}
	}	
}
