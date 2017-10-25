#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections;

/// <summary>
/// Zombie manager editor.
/// 通过点击Inspector的按钮，可以方便地添加删除子节点。添加是可以，但是删除时不会更新父级可咋办？
/// 所以在删除后自动选中父节点，然后自动删除空引用的节点，就好啦~
/// </summary>
[CustomEditor(typeof(MapBlockEditor))]
public class MapEditor : Editor 
{
	MapBlockEditor map_block_editor = null;

	void OnEnable()
	{
		map_block_editor = serializedObject.targetObject as MapBlockEditor;
	}

	void OnDisable()
	{
		
	}

	void OnSceneGUI()
	{ 
		Event e = Event.current;

		if(e.type == EventType.MouseDown && e.button == 0)
		{
			Vector2 mouse_pos = Event.current.mousePosition;

			//Debug.Log("mouse pos " + mouse_pos + " size : " + SceneView.lastActiveSceneView.position);

			if(map_block_editor != null)
			{
				Vector2 screen_pos = new Vector2(mouse_pos.x, SceneView.lastActiveSceneView.position.height - mouse_pos.y);
				map_block_editor.SetBlockByScreenPosition(screen_pos);
			}
		}
			
		// 渲染阻挡信息
		if(map_block_editor._grid_array != null)
		{
			int width = map_block_editor.map_width;
			int height = map_block_editor.map_height;

			for(int x = 0; x < width; ++x)
			{
				for(int y = 0; y < height; ++y)
				{
					if(map_block_editor.IsBlock(x, y))
					{
						float x0 = x * map_block_editor.map_step;
						float x1 = x0 + map_block_editor.map_step;

						float y0 = y * map_block_editor.map_step;
						float y1 = y0 + map_block_editor.map_step;

						Vector3[] verts = new Vector3[]
						{
							new Vector3(x0, 0, y0),
							new Vector3(x0, 0, y1),
							new Vector3(x1, 0, y1),
							new Vector3(x1, 0, y0),
						};

						Handles.DrawSolidRectangleWithOutline(verts, Color.red, Color.red);
					}
				}
			}
		}

	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		GUI.color = Color.green;
		if (GUILayout.Button("创建地图"))
		{
			SerializedProperty width_property = serializedObject.FindProperty("map_width");
			SerializedProperty height_property = serializedObject.FindProperty("map_height");
			SerializedProperty step_property = serializedObject.FindProperty("map_step");

			map_block_editor.CreateNewMap(width_property.intValue, height_property.intValue, step_property.floatValue);
		}

		GUI.color = Color.yellow;
		if (GUILayout.Button("保存地图"))
		{
			string path = EditorUtility.SaveFilePanel("保存地图", "", "map_block_info", "");

			if(path.Length != 0)
			{
				map_block_editor.SaveMapBlockData(path);
			}
		}


		GUI.color = Color.yellow;
		if (GUILayout.Button("加载地图"))
		{
			string path = EditorUtility.OpenFilePanel("加载地图", "", "");

			if(path.Length != 0)
			{
				map_block_editor.LoadMapBlockData(path);
			}
		}

		serializedObject.ApplyModifiedProperties();
	}

}

#endif