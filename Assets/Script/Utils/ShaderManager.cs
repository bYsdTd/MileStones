using UnityEngine;
using System.Collections;

public class ShaderManager
{
	static private ShaderManager instance = null;

	static public ShaderManager Instance()
	{
		if(instance == null)
		{
			instance = new ShaderManager();

			instance.Init();
		}

		return instance;
	}

	public Shader transparent_colored_shader_;
	public Shader grey_color_split_shader_;
	public Shader grey_color_shader_;
	public Shader text_shader_;

	public void Init()
	{
		transparent_colored_shader_ = Resources.Load<Shader>("Unlit/Transparent Colored");
		grey_color_split_shader_ = Resources.Load<Shader>("Custom/GreyColor Split");
		grey_color_shader_ = Resources.Load<Shader>("Custom/GreyColor");
		text_shader_ = Resources.Load<Shader>("Unlit/Text");
	}

	public void Destroy()
	{

	}
}
