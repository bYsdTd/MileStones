using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GDSKit
{
	public partial class GDSMgr
	{
		static private GDSMgr instance = null;

		static public GDSMgr Instance()
		{
			if(instance == null)
			{
				instance = new GDSMgr();
			}

			return instance;
		}

		private string GetGDSFileData(string filename)
		{
			return FileManager.Instance().ReadAllText("GDS/" + filename);
		}
			

	}
}

