
using System;
using System.Collections;

namespace GDSKit
{
	public partial class GDSMgr
	{
		private string[] gdsFiles = new string[1]
		{
			"unit.csv",
			
		};

		public void InitGDSData()
		{
			// SLua.LuaFunction func = LuaGameManager.instance().GetState().getFunction("gdsInitCallback");

			string unit_data = GetGDSFileData(gdsFiles[0]);
			if (!String.IsNullOrEmpty(unit_data))
			{
				unit.Initialize(CSVParser.Parse(unit_data, CSVParser.PARSE_DATA).data);
				// func.call("unit", unit_data);
			}
			
		}
		
		public unit Getunit(string unit_name)
		{
			return unit.GetInstance(unit_name);
		}
		
	}
}
