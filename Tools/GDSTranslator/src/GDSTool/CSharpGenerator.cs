using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GDSTool
{
	public class CSharpGenerator : ILiteralGenerator
	{
		public const string NAMESPACE_STUB 		= "#NAMESPACE_STUB#";
		public const string CLASSNAME_STUB 		= "#CLASSNAME_STUB#";
		public const string FIELD_STUB 			= "#FIELD_STUB#";
		public const string INITIALIZE_STUB 	= "#INITIALIZE_STUB#";
		public const string KEY_ARGS_STUB 		= "#KEY_ARGS_STUB#";
		public const string KEY_COMBINER_STUB_WITH_THIS 	= "#KEY_COMBINER_STUB_WITH_THIS#";
		public const string KEY_COMBINER_STUB_WITHOUT_THIS  = "#KEY_COMBINER_STUB_WITHOUT_THIS#";

		public const string LITERAL_KEY_ARG_SEP = ", ";
		public const string LITERAL_KEY_COMBINER_SEP = " + \u0022^\u0022 + ";

		public delegate string MetaTypeParser(string semanticInput);
		private static Dictionary<string, MetaTypeParser> metaTypeParsers = new Dictionary<string, MetaTypeParser>()
		{
			{ "int", 	delegate(string any) { return "CSVParser.GetAsInt(" + any + ")"; } },
			{ "float",  delegate(string any) { return "CSVParser.GetAsFloat(" + any + ")"; } },
			{ "bool",  delegate(string any) { return "CSVParser.GetAsBool(" + any + ")"; } },
			{ "string", delegate(string any) { return "String.Intern(CSVParser.GetAsString(" + any + "))"; } }
		};

		public string GetCodeFileName(string csvName)
		{
			return csvName + ".cs";
		}
		public string GetCodeFileContent(string clsName, string nameSpaceName, CSVParser.Output arg)
		{
			Console.WriteLine("Generating C# GDS code : " + clsName);
			StreamReader fs = File.OpenText("../src/GDSTool/csharp.code");
			string template = fs.ReadToEnd();

			StringBuilder sb = new StringBuilder(template);
			sb.Replace(NAMESPACE_STUB, nameSpaceName);
			sb.Replace(CLASSNAME_STUB, clsName);

			var contentStrings = MakeFileContentStrings(arg);
			foreach (KeyValuePair<string, StringBuilder> kvp in contentStrings)
			{
				sb.Replace(kvp.Key, kvp.Value.ToString());
			}

			return sb.ToString();
		}

		private bool isComposedType(string metaType)
		{
			return metaType.Length > 0 && metaType[0] == CSVParser.CSVStyle.ComposeStarter;
		}

		private Dictionary<string, StringBuilder> MakeFileContentStrings(CSVParser.Output arg)
		{
			var ret = new Dictionary<string, StringBuilder>()
			{
				{ FIELD_STUB, new StringBuilder() },
				{ INITIALIZE_STUB, new StringBuilder() },
				{ KEY_ARGS_STUB, new StringBuilder() },
				{ KEY_COMBINER_STUB_WITH_THIS, new StringBuilder() },
				{ KEY_COMBINER_STUB_WITHOUT_THIS, new StringBuilder() }
			};

			int len = arg.fieldMetaTypes.Length;
			for (int i = 0; i < len; ++i)
			{
				string organizeType = arg.fieldOrganizeTypes[i];
				string metaType = arg.fieldMetaTypes[i];
				string fieldName = arg.fieldNames[i].ToLower();
				bool isKey = !String.IsNullOrEmpty(arg.isKeyFlags[i]);
				if (!isComposedType(metaType) && !metaTypeParsers.ContainsKey(metaType))
				{
					throw new Exception("unknown field meta type : " + metaType + ", fieldName : " + fieldName);
				}
				if ("Array" == organizeType)
				{
					AddArrayFieldDeclare(ret[FIELD_STUB], fieldName, metaType);
					AddArrayFieldInitializer(ret[INITIALIZE_STUB], fieldName, metaType, i);
					if (isKey)
					{
						throw new Exception("Error : array type can't be key!");
					}
				}
				else if ("Meta" == organizeType)
				{
					AddMetaFieldDeclare(ret[FIELD_STUB], fieldName, metaType);
					AddMetaFieldInitializer(ret[INITIALIZE_STUB], fieldName, metaType, i);
					if (isKey)
					{
						AddKeyArg(ret[KEY_ARGS_STUB], fieldName, metaType);
						AddKeyCombiner(ret[KEY_COMBINER_STUB_WITH_THIS], fieldName, metaType);
						AddKeyCombiner2(ret[KEY_COMBINER_STUB_WITHOUT_THIS], fieldName, metaType);
					}
				}
				else
				{
					throw new Exception("Unknown field compose type : " + organizeType);
				}
			}

			if (String.IsNullOrEmpty(ret[KEY_ARGS_STUB].ToString())
			    || String.IsNullOrEmpty(ret[KEY_COMBINER_STUB_WITH_THIS].ToString())
			    || String.IsNullOrEmpty(ret[KEY_COMBINER_STUB_WITHOUT_THIS].ToString()))
			{
				throw new Exception("Invalid key config!");
			}

			Utils.TrimEnd(ret[KEY_ARGS_STUB], LITERAL_KEY_ARG_SEP.Length);
			Utils.TrimEnd(ret[KEY_COMBINER_STUB_WITH_THIS], LITERAL_KEY_COMBINER_SEP.Length);
			Utils.TrimEnd(ret[KEY_COMBINER_STUB_WITHOUT_THIS], LITERAL_KEY_COMBINER_SEP.Length);

			return ret;
		}

		//public TYPE NAME;
		private void AddMetaFieldDeclare(StringBuilder sb, string name, string type)
		{
			if (!isComposedType(type))
			{
				sb.Append("public ");
				sb.Append(type);
				sb.Append(" ");
				sb.Append(name);
				sb.Append(";\n\t\t");
			}
			else
			{
				AddStructFieldDeclare(sb, name, type, false);
			}
		}

		/*
		public class composed_tk
		{
			public string field1;
			public int field2;
		}
		public composed_tk tk = null;
		 */
		private void AddStructFieldDeclare(StringBuilder sb, string name, string type, bool isArray)
		{
			string[] names = name.Split(CSVParser.CSVStyle.ArrayElementSeperator);
			string[] types = GetMemberTypes(type);
			if (names.Length != (types.Length + 1))
			{
				throw new Exception("struct fields and names mismatch!");
			}

			sb.Append("public class composed_");
			sb.Append(names[0]);
			sb.Append("\n\t\t");
			sb.Append("{\n");
			for (int i = 0; i < types.Length; ++i)
			{
				sb.Append("\t\t\tpublic ");
				sb.Append(types[i]);
				sb.Append(" ");
				sb.Append(names[i + 1]);
				sb.Append(";\n");
			}
			sb.Append("\t\t}\n\t\t");
			sb.Append("public composed_");
			sb.Append(names[0]);
			if (isArray)
			{
				sb.Append("[]");
			}

			sb.Append(" ");
			sb.Append(names[0]);
			sb.Append(" = null;\n\t\t");
		}

		//"public List<TYPE> NAME = new List<TYPE>();"
		private void AddArrayFieldDeclare(StringBuilder sb, string name, string type)
		{
			if (!isComposedType(type))
			{
				sb.Append("public List<");
				sb.Append(type);
				sb.Append("> ");
				sb.Append(name);
				sb.Append(" = new List<");
				sb.Append(type);
				sb.Append(">();\n\t\t");
			}
			else
			{
				AddStructFieldDeclare(sb, name, type, true);
			}
		}

		//_ret_.a = int.Parse(objArr[0] as String);
		private void AddMetaFieldInitializer(StringBuilder sb, string name, string type, int index)
		{
			if (!isComposedType(type))
			{
				sb.Append("_ret_.");
				sb.Append(name);
				sb.Append(" = ");
				sb.Append(metaTypeParsers[type]("objArr[" + index + "]"));
				sb.Append(";\n\t\t\t");
				sb.Append("\n\t\t\t");
			}
			else
			{
				AddStructFieldInitializer(sb, name, type, index);
			}
		}

		private void AddStructFieldInitializer(StringBuilder sb, string name, string type, int index)
		{
			string[] names = name.Split(CSVParser.CSVStyle.ArrayElementSeperator);
			string[] types = GetMemberTypes(type);

			sb.Append("var _");
			sb.Append(names[0]);
			sb.Append("_data_ = CSVParser.GetAsStructData(objArr[" + index + "]);\n\t\t\t");
			sb.Append("_ret_.");
			sb.Append(names[0]);
			sb.Append(" = new composed_");
			sb.Append(names[0]);
			sb.Append("();\n\t\t\t");

			for (int i = 0; i < types.Length; ++i)
			{
				sb.Append("_ret_.");
				sb.Append(names[0]);
				sb.Append(".");
				sb.Append(names[i + 1]);

				sb.Append(" = ");
				sb.Append(metaTypeParsers[types[i]]("_" + names[0] + "_data_[" + i + "] as String"));
				sb.Append(";\n\t\t\t");
			}

			sb.Append("\n\t\t\t");
		}

		private void AddArrayFieldInitializer(StringBuilder sb, string name, string type, int index)
		{
			if (!isComposedType(type))
			{
				sb.Append("var ");
				sb.Append(name);
				sb.Append("_values = CSVParser.GetAsMetaArray(objArr[" + index + "]);\n\t\t\t");

				sb.Append("foreach(var s in ");
				sb.Append(name);
				sb.Append("_values)\n\t\t\t");
				sb.Append("{\n\t\t\t\t");
				sb.Append("_ret_.");
				sb.Append(name);
				sb.Append(".Add(");
				sb.Append(metaTypeParsers[type]("s"));
				sb.Append(");\n\t\t\t");
				sb.Append("}\n\t\t\t");
				sb.Append("\n\t\t\t");
			}
			else
			{
				AddStructArrayFieldInitializer(sb, name, type, index);
			}
		}

		private void AddStructArrayFieldInitializer(StringBuilder sb, string name, string type, int index)
		{
			string[] names = name.Split(CSVParser.CSVStyle.ArrayElementSeperator);
			string[] types = GetMemberTypes(type);

			string data_name = "_" + names[0] + "_data_";
			string struct_name = "composed_" + names[0];

			sb.Append("var ");
			sb.Append(data_name);
			sb.Append(" = CSVParser.GetAsStructArray(objArr[" + index + "]);\n\t\t\t");
			sb.Append("_ret_.");
			sb.Append(names[0]);
			sb.Append(" = new ");
			sb.Append(struct_name);
			sb.Append("[");
			sb.Append(data_name);
			sb.Append(".Length];\n\t\t\t");

			sb.Append("for (int i = 0; i < ");
			sb.Append(data_name);
			sb.Append(".Length; ++i)\n\t\t\t");

			sb.Append("{\n\t\t\t\t");

			sb.Append("var _tmp_ = new ");
			sb.Append(struct_name);
			sb.Append("();\n\t\t\t\t");
			sb.Append("var _data_ = CSVParser.GetAsMetaArray(");
			sb.Append(data_name);
			sb.Append("[i]);\n\t\t\t\t");

			for (int i = 0; i < types.Length; ++i)
			{
				sb.Append("_tmp_.");
				sb.Append(names[i + 1]);
				
				sb.Append(" = ");
				sb.Append(metaTypeParsers[types[i]]("_data_[" + i + "] as String"));
				sb.Append(";\n\t\t\t\t");
			}

			sb.Append("_ret_.");
			sb.Append(names[0]);
			sb.Append("[i] = _tmp_;\n\t\t\t");

			sb.Append("}\n\t\t\t");
			sb.Append("\n\t\t\t");
		}

		//int a, float b
		private void AddKeyArg(StringBuilder sb, string name, string type)
		{
			sb.Append(type);
			sb.Append(" ");
			sb.Append(name);
			sb.Append(LITERAL_KEY_ARG_SEP);
		}

		//this.a.ToString() + "^" + this.b.ToString()
		private void AddKeyCombiner(StringBuilder sb, string name, string type)
		{
			sb.Append("this.");
			sb.Append(name);
			sb.Append(".ToString()");
			sb.Append(LITERAL_KEY_COMBINER_SEP);
		}

		//a.ToString() + "^" + b.ToString()
		private void AddKeyCombiner2(StringBuilder sb, string name, string type)
		{
			sb.Append(name);
			sb.Append(".ToString()");
			sb.Append(LITERAL_KEY_COMBINER_SEP);
		}

		private string[] GetMemberTypes(string composeType)
		{
			string tmp = composeType.Substring(1, composeType.Length - 2);
			return tmp.Split(CSVParser.CSVStyle.ArrayElementSeperator);
		}
	}
}
