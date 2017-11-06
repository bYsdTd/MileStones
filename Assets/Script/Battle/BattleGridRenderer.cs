using UnityEngine;
using System.Collections;

public class BattleGridRenderer : MonoBehaviour
{
	public Material	mat_grid;

	[HideInInspector]
	public int map_width;
	[HideInInspector]
	public int map_height;
	[HideInInspector]
	public float map_step;
	[HideInInspector]
	public MeshFilter mesh_filter;
	[HideInInspector]
	public Mesh mesh;
	[HideInInspector]
	public MeshRenderer mesh_renderer;
	[HideInInspector]
	public BattleField battle_field;

	void Start ()
	{

	}

	public void Init(int width, int height, float step)
	{
		mesh_filter = gameObject.GetComponent<MeshFilter>();

		if(mesh_filter == null)
		{
			mesh_filter = gameObject.GetOrAddComponent<MeshFilter>();
		}

		mesh_renderer = gameObject.GetComponent<MeshRenderer>();

		if(mesh_renderer == null)
		{
			mesh_renderer = gameObject.GetOrAddComponent<MeshRenderer>();
		}

		mesh = new Mesh();
		mesh.name = gameObject.name + "Mesh";

		mesh_filter.mesh = mesh;

		mesh_renderer.sharedMaterial = mat_grid;	

		map_width = width;
		map_height = height;
		map_step = step;

		UpdateMesh();
	}

	private void UpdateMesh()
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

		for(int x = 0; x < map_width; ++x)
		{
			for(int y = 0; y < map_height; ++y)
			{
				int gridIndex = x + y * map_width;
				int vertIndex = gridIndex * 4;

				// vertices
				vertices[vertIndex] = new Vector3(x * map_step, 0.0f, y * map_step);
				vertices[vertIndex + 1] = new Vector3((x+1) * map_step, 0.0f, y * map_step);
				vertices[vertIndex + 2] = new Vector3((x+1) * map_step, 0.0f, (y+1) * map_step);
				vertices[vertIndex + 3] = new Vector3(x * map_step, 0.0f, (y+1) * map_step);

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
				Color grid_color = Color.grey;

//				if(battle_field.IsBlock(x, y))
//				{
//					grid_color = Color.red;
//				}

				colors[vertIndex] = grid_color;
				colors[vertIndex + 1] = grid_color;
				colors[vertIndex + 2] = grid_color;
				colors[vertIndex + 3] = grid_color;
			}
		}

		mesh.Clear();

		mesh.vertices = vertices;
		mesh.triangles = indices;
		mesh.uv = uvs;
		mesh.colors = colors;

	}
}
