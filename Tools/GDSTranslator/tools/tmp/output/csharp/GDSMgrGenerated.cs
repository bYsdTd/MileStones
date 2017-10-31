
using System;
using System.Collections;

namespace GDSKit
{
	public partial class GDSMgr
	{
		private string[] gdsFiles = new string[2]
		{
			"parameter.csv",
			"shop.csv",
			
		};

		public void InitGDSData()
		{
			// SLua.LuaFunction func = LuaGameManager.instance().GetState().getFunction("gdsInitCallback");

			string parameter_data = GetGDSFileData(gdsFiles[0]);
			if (!String.IsNullOrEmpty(parameter_data))
			{
				parameter.Initialize(CSVParser.Parse(parameter_data, CSVParser.PARSE_DATA).data);
				// func.call("parameter", parameter_data);
			}
			string shop_data = GetGDSFileData(gdsFiles[1]);
			if (!String.IsNullOrEmpty(shop_data))
			{
				shop.Initialize(CSVParser.Parse(shop_data, CSVParser.PARSE_DATA).data);
				// func.call("shop", shop_data);
			}
			
		}
		
		public parameter Getparameter(string parameter_name)
		{
			return parameter.GetInstance(parameter_name);
		}
		public shop Getshop(string item_name)
		{
			return shop.GetInstance(item_name);
		}
		
	}
}
