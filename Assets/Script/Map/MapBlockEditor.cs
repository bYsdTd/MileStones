#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.IO;

public class MapBlockEditor : MonoBehaviour
{
	public Material	mat_grid;

	[Header("地图配置信息")]
	public int map_width;
	public int map_height;

	public float map_step;

	[HideInInspector]
	public MeshFilter mesh_filter;
	[HideInInspector]
	public Mesh mesh;
	[HideInInspector]
	public MeshRenderer mesh_renderer;

	// 二维数组，表示哪个位置有阻挡
	public bool[,] _grid_array;

	public void SetBlock(int x, int y, bool value)
	{
		if(_grid_array == null)
		{
			Debug.LogError("地图阻挡数组没有初始化! ");
			return;
		}

		_grid_array[x,y] = value;
	}

	public bool IsBlock(int x, int y)
	{
		if(_grid_array == null)
		{
			Debug.LogError("地图阻挡数组没有初始化! ");
			return false;
		}

		return _grid_array[x, y];
	}

	public void InitBlockData()
	{
		if(_grid_array == null)
		{
			// 初始化所有的阻挡
			_grid_array = new bool[map_width, map_height];	
		}
	}

	public void ClearAllBlock()
	{
		for(int i = 0; i < map_width; ++i)
		{
			for(int j = 0; j < map_height; ++j)
			{
				SetBlock(i, j, false);
			}
		}

	}

	public void InitMeshComponent()
	{
		mesh_filter = gameObject.GetComponent<MeshFilter>();

		if(mesh_filter == null)
		{
			mesh_filter = gameObject.AddComponent<MeshFilter>();
		}

		mesh_renderer = gameObject.GetComponent<MeshRenderer>();

		if(mesh_renderer == null)
		{
			mesh_renderer = gameObject.AddComponent<MeshRenderer>();
		}

		mesh = new Mesh();
		mesh.name = gameObject.name + "Mesh";

		mesh_filter.mesh = mesh;

		mesh_renderer.sharedMaterial = mat_grid;
	}
		
	public void UpdateMesh()
	{
		if(mesh == null)
		{
			Debug.LogError(" 更新mesh 的时候，发现mesh是空的!");
			return;
		}

		Vector3[] vertices = new Vector3[map_width * map_height * 4];
		int[] indices = new int[map_width * map_height * 6];
		Vector2[] uvs = new Vector2[map_width * map_height * 4];
		Color[] colors = new Color[map_width * map_height * 4];

		for(int i = 0; i < map_width; ++i)
		{
			for(int j = 0; j < map_height; ++j)
			{
				int gridIndex = i + j * map_width;
				int vertIndex = gridIndex * 4;

				// vertices
				vertices[vertIndex] = new Vector3(i * map_step, 0.0f, j * map_step);
				vertices[vertIndex + 1] = new Vector3((i+1) * map_step, 0.0f, j * map_step);
				vertices[vertIndex + 2] = new Vector3((i+1) * map_step, 0.0f, (j+1) * map_step);
				vertices[vertIndex + 3] = new Vector3(i * map_step, 0.0f, (j+1) * map_step);

				int indicesIndex = gridIndex * 6;

				// indices
				indices[indicesIndex] = vertIndex;
				indices[indicesIndex + 1] = vertIndex + 2;
				indices[indicesIndex + 2] = vertIndex + 1;
				indices[indicesIndex + 3] = vertIndex;
				indices[indicesIndex + 4] = vertIndex + 3;
				indices[indicesIndex + 5] = vertIndex + 2;

				// uvs
				uvs[vertIndex] = new Vector2(0, 0);
				uvs[vertIndex + 1] = new Vector2(1, 0);
				uvs[vertIndex + 2] = new Vector2(1, 1);
				uvs[vertIndex + 3] = new Vector2(0, 1);

				// colors
				Color color = Color.white;

//				if(IsBlock(i, j))
//				{
//					color = Color.red;
//				}

				colors[vertIndex] = color;
				colors[vertIndex + 1] = color;
				colors[vertIndex + 2] = color;
				colors[vertIndex + 3] = color;
			}
		}

		mesh.Clear();

		mesh.vertices = vertices;
		mesh.triangles = indices;
		mesh.uv = uvs;
		mesh.colors = colors;

	}

	public void SetBlockByScreenPosition(Vector2 scene_position)
	{
		Ray camera_ray = UnityEditor.SceneView.lastActiveSceneView.camera.ScreenPointToRay(new Vector3(scene_position.x, scene_position.y, 0));

		Plane grid_plane = new Plane(Vector3.up, Vector3.zero);

		float ray_distance = 0;
		if(grid_plane.Raycast(camera_ray, out ray_distance))
		{
			Vector3 hit_point = camera_ray.GetPoint(ray_distance);

			int x = (int)(hit_point.x / map_step);
			int y = (int)(hit_point.z / map_step);

			if(x >= 0 && x < map_width && y >= 0 && y < map_height)
			{
				SetBlock(x, y, !IsBlock(x, y));
			}
		}
	}

	// 创建地图
	public void CreateNewMap(int width, int height, float step)
	{
		if(width > 0 && height > 0 && step > 0)
		{
			map_width = width;
			map_height = height;
			map_step = step;

			InitMeshComponent();
			InitBlockData();

			ClearAllBlock();

			UpdateMesh();
		}
	}

	// 保存地图
	public void SaveMapBlockData(string path)
	{
		MapSaveData map_save_data = new MapSaveData();
		map_save_data.map_width = map_width;
		map_save_data.map_height = map_height;
		map_save_data.map_step = map_step;
		map_save_data.grid_array = _grid_array;

		using (FileStream stream = File.Open(path, FileMode.Create))
		{
			var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			binaryFormatter.Serialize(stream, map_save_data);
		}
	}

	// 加载地图
	public void LoadMapBlockData(string path)
	{
		if(!File.Exists(path))
		{
			return;
		}

		using (FileStream stream = File.Open(path, FileMode.Open))
		{
			MapSaveData map_save_data = new MapSaveData();
			var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			map_save_data = (MapSaveData)binaryFormatter.Deserialize(stream);

			map_width = map_save_data.map_width;
			map_height = map_save_data.map_height;
			map_step = map_save_data.map_step;
			_grid_array = map_save_data.grid_array;

			InitMeshComponent();
			UpdateMesh();
		}
	}
}

#endif