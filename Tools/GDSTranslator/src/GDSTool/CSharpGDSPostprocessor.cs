using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GDSTool
{
	public class CSharpGDSPostprocessor : GDSPostprocessor
	{
		public const string CSVPARSER_NAME = "CSVParser.cs";
		public const string GDS_MANAGER_NAME = "GDSMgrGenerated.cs";
		public const string NAMESPACE_STUB = "#NAMESPACE_STUB#";
		public const string QUOTE_STUB = "#QUOTE_STUB#";

		public const string FILE_NUMBER_STUB = "#FILE_NUMBER_STUB#";
		public const string FILE_NAMES_STUB = "#FILE_NAMES_STUB#";
		public const string INIT_BLOCK_STUB = "#INIT_BLOCK_STUB#";
		public const string GDS_GETTER_STUB = "#GDS_GETTER_STUB#";


		public override void Postprocess (GDSTranslator.Input input, GDSTranslator.Output result)
		{
			string csharpNamespace = "";
			for (int i = 0; i < input.dstLanguageNames.Length; ++i)
			{
				if ("csharp" == input.dstLanguageNames[i])
				{
					csharpNamespace = input.namespaceNames[i];
					break;
				}
			}
			
			result.codeFileNames["csharp"].Add(CSVPARSER_NAME);
			result.codeFileContents["csharp"].Add(DumpCSVParser(csharpNamespace));

			result.codeFileNames["csharp"].Add(GDS_MANAGER_NAME);
			result.codeFileContents["csharp"].Add(DumpGDSManager(input, csharpNamespace));

			base.Postprocess (input, result);
		}

		private string DumpCSVParser(string nameSpaceName)
		{		
			StreamReader fs = File.OpenText("../src/GDSTool/CSVParser.cs");
			string csvparserCode = fs.ReadToEnd();

			StringBuilder sb = new StringBuilder(csvparserCode);
			sb.Replace("GDSTool", nameSpaceName);
			return sb.ToString();
		}

		private string DumpGDSManager(GDSTranslator.Input input, string nameSpaceName)
		{
			StringBuilder sb = new StringBuilder(partialGDSManager);

			StringBuilder sbInitBlock = new StringBuilder();
			StringBuilder sbGetter = new StringBuilder();
			StringBuilder sbFileName = new StringBuilder();

			int len = input.csvNames.Length;
			int fileCount = 0;
			for (int i = 0; i < len; ++i)
			{
				if (input.IsIgnored("csharp", input.csvNames[i]))
				{
					continue;
				}
				++fileCount;
				AddFileName(sbFileName, input.csvNames[i]);
				CSVParser.Output typeInfo = CSVParser.Parse(input.csvContents[i], CSVParser.PARSE_TYPE_INFO);
				AddInitBlock(sbInitBlock, input.csvNames[i], fileCount - 1);
				AddGDSGetter(sbGetter, input.csvNames[i], typeInfo);
			}

			sb.Replace(INIT_BLOCK_STUB, sbInitBlock.ToString());
			sb.Replace(GDS_GETTER_STUB, sbGetter.ToString());
			sb.Replace(FILE_NUMBER_STUB, fileCount.ToString());
			sb.Replace(FILE_NAMES_STUB, sbFileName.ToString());
			sb.Replace(NAMESPACE_STUB, nameSpaceName);
			sb.Replace(QUOTE_STUB, "\"");
			return sb.ToString();
		}

		/*
			try
			{
				string building_archercamp_data = GetGDSFileData(gdsFiles[0]);
				if (!String.IsNullOrEmpty(building_archercamp_data))
				{
					building_archercamp.Initialize(CSVParser.Parse(building_archercamp_data, CSVParser.PARSE_DATA).data);
					GDSClassPostProcessor(typeof(building_archercamp));
				}
			}
			catch (Exception e)
			{
				isDataCorrupted = true;
				yield break;
			}
		 */
		private void AddInitBlock(StringBuilder sb, string clsName, int idx)
		{
			clsName = clsName.ToLower();
			//sb.Append("try\n\t\t\t");
			//sb.Append("{\n\t\t\t");
			sb.Append("string ");
			sb.Append(clsName);
			sb.Append("_data = GetGDSFileData(gdsFiles[");
			sb.Append(idx.ToString());
			sb.Append("]);\n\t\t\t");
			sb.Append("if (!String.IsNullOrEmpty(");
			sb.Append(clsName);
			sb.Append("_data))\n\t\t\t");
			sb.Append("{\n\t\t\t\t");
			sb.Append(clsName);
			sb.Append(".Initialize(CSVParser.Parse(");
			sb.Append(clsName);
			sb.Append("_data, CSVParser.PARSE_DATA).data);\n\t\t\t\t");
			sb.Append("// func.call(\"");
			sb.Append(clsName);
			sb.Append("\", ");
			sb.Append(clsName);
			sb.Append("_data);");
			sb.Append("\n\t\t\t");
			sb.Append("}\n\t\t\t");
			//sb.Append("}\n\t\t\t");
			//sb.Append("catch (Exception e)\n\t\t\t");
			//sb.Append("{\n\t\t\t\t");
			//sb.Append("LogException(e);\n\t\t\t\t");
			//sb.Append("}\n\t\t\t");
			//sb.Append("yield return null;\n\t\t\t");
			//sb.Append("\n\t\t\t");
		}

		/*
			public Building GetBuilding(int id)
			{
				return Building.GetInstance(id);
			}
		 */
		private void AddGDSGetter(StringBuilder sb, string clsName, CSVParser.Output output)
		{
			string conj = ", ";
			sb.Append("public ");
			sb.Append(clsName);
			sb.Append(" Get");
			sb.Append(clsName);
			sb.Append("(");

			for(int i = 0; i < output.fieldMetaTypes.Length; ++i)
			{
				if (String.IsNullOrEmpty(output.isKeyFlags[i]))
				{
					continue;
				}
				sb.Append(output.fieldMetaTypes[i]);
				sb.Append(" ");
				sb.Append(output.fieldNames[i]);
				sb.Append(conj);
			}
			sb.Remove(sb.Length - conj.Length, conj.Length);

			sb.Append(")\n\t\t");
			sb.Append("{\n\t\t\t");
			sb.Append("return ");
			sb.Append(clsName);
			sb.Append(".GetInstance(");

			for(int i = 0; i < output.fieldNames.Length; ++i)
			{
				if (String.IsNullOrEmpty(output.isKeyFlags[i]))
				{
					continue;
				}
				sb.Append(output.fieldNames[i]);
				sb.Append(conj);
			}
			sb.Remove(sb.Length - conj.Length, conj.Length);

			sb.Append(");\n\t\t");
			sb.Append("}\n\t\t");
		}

		private void AddFileName(StringBuilder sb, string filename)
		{
			sb.Append("\"" + filename + ".csv\",\n\t\t\t");
		}

		private static string partialGDSManager = @"
using System;
using System.Collections;

namespace #NAMESPACE_STUB#
{
	public partial class GDSMgr
	{
		private string[] gdsFiles = new string[#FILE_NUMBER_STUB#]
		{
			#FILE_NAMES_STUB#
		};

		public void InitGDSData()
		{
			// SLua.LuaFunction func = LuaGameManager.instance().GetState().getFunction(""gdsInitCallback"");

			#INIT_BLOCK_STUB#
		}
		
		#GDS_GETTER_STUB#
	}
}
";
	}
}
