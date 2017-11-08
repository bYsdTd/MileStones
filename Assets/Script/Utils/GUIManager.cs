using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager  
{
	static private GUIManager instance = null;

	static public GUIManager Instance()
	{
		if(instance == null)
		{
			instance = new GUIManager();
		}

		return instance;
	}

	private Transform _cache_root;

	public Transform cache_root
	{
		get
		{
			return _cache_root;
		}
	}

	private Transform 	ngui_root_;
	private UIRoot		ui_root_componet_;

	public void Init()
	{
		ngui_root_ = GameObject.Find("UI Root").transform;
		GameObject.DontDestroyOnLoad(ngui_root_);

		GameObject temp = NGUITools.AddChild(ngui_root_.gameObject);
		temp.name = "UIRootNode";
		_cache_root = temp.transform;

		ui_root_componet_ = ngui_root_.gameObject.GetComponent<UIRoot>();

		cache_root.rotation = Quaternion.AngleAxis(180, Vector3.right);
		cache_root.localPosition = new Vector3(-ui_root_componet_.manualWidth * 0.5f, ui_root_componet_.activeHeight * 0.5f);
	}

	public Vector3 ScreenPosToUIPos(Vector3 screen_pos)
	{
		screen_pos.y = Camera.main.pixelHeight - screen_pos.y;

		Vector3 ui_position = Vector3.zero;

		ui_position.x = ui_root_componet_.manualWidth * screen_pos.x / Camera.main.pixelWidth;
		ui_position.y = ui_root_componet_.activeHeight * screen_pos.y / Camera.main.pixelHeight;
	
		return ui_position;
	}
}
