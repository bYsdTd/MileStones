using UnityEngine;
using System.Collections;
using System.IO;

public class FileManager
{
	static private FileManager instance = null;

	static public FileManager Instance()
	{
		if(instance == null)
		{
			instance = new FileManager();
		}

		return instance;
	}

	public string GetWriteablePath()
	{
		#if UNITY_EDITOR

		return Application.streamingAssetsPath + "/";

		#endif
	}
		
	public string ReadAllText(string path)
	{
		string path_prefix = GetWriteablePath();

		path = path_prefix + path;

		return File.ReadAllText(path);
	}
}
