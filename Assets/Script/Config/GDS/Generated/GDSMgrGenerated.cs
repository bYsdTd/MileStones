﻿
using System;
using System.Collections;

namespace GDSKit
{
	public partial class GDSMgr
	{
		private string[] gdsFiles = new string[4]
		{
			"building.csv",
			"chat_expression.csv",
			"ui_config.csv",
			"unit.csv",
			
		};

		public void InitGDSData()
		{
			// SLua.LuaFunction func = LuaGameManager.instance().GetState().getFunction("gdsInitCallback");

			string building_data = GetGDSFileData(gdsFiles[0]);
			if (!String.IsNullOrEmpty(building_data))
			{
				building.Initialize(CSVParser.Parse(building_data, CSVParser.PARSE_DATA).data);
				// func.call("building", building_data);
			}
			string chat_expression_data = GetGDSFileData(gdsFiles[1]);
			if (!String.IsNullOrEmpty(chat_expression_data))
			{
				chat_expression.Initialize(CSVParser.Parse(chat_expression_data, CSVParser.PARSE_DATA).data);
				// func.call("chat_expression", chat_expression_data);
			}
			string ui_config_data = GetGDSFileData(gdsFiles[2]);
			if (!String.IsNullOrEmpty(ui_config_data))
			{
				ui_config.Initialize(CSVParser.Parse(ui_config_data, CSVParser.PARSE_DATA).data);
				// func.call("ui_config", ui_config_data);
			}
			string unit_data = GetGDSFileData(gdsFiles[3]);
			if (!String.IsNullOrEmpty(unit_data))
			{
				unit.Initialize(CSVParser.Parse(unit_data, CSVParser.PARSE_DATA).data);
				// func.call("unit", unit_data);
			}
			
		}
		
		public building Getbuilding(string building_name)
		{
			return building.GetInstance(building_name);
		}
		public chat_expression Getchat_expression(string expression_name)
		{
			return chat_expression.GetInstance(expression_name);
		}
		public ui_config Getui_config(string layout_name)
		{
			return ui_config.GetInstance(layout_name);
		}
		public unit Getunit(string unit_name)
		{
			return unit.GetInstance(unit_name);
		}
		
	}
}
