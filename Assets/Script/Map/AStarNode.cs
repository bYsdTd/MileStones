using UnityEngine;
using System.Collections;

public class AStarNode  
{
	public int _x;
	public int _y;

	public AStarNode _parent;

	public int _g;
	public int _h;
	public int _f;

	public AStarNode()
	{
		_g = 0;
		_h = 0;
		_f = 0;
		_x = 0;
		_y = 0;

		_parent = null;
	}
}
