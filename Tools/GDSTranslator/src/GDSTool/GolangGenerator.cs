using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GDSTool
{
	public class GolangGenerator : ILiteralGenerator
	{
		public const string NAMESPACE_STUB 		= "#NAMESPACE_STUB#";
		public const string CLASSNAME_STUB 		= "#CLASSNAME_STUB#";
		public const string COMPOSE_DECLARE_STUB = "#COMPOSE_DECLARE_STUB#";
		public const string FIELD_STUB 			= "#FIELD_STUB#";
		public const string INITIALIZE_STUB 	= "#INITIALIZE_STUB#";
		public const string KEY_ARGS_STUB 		= "#KEY_ARGS_STUB#";
		public const string KEY_COMBINER_STUB_WITH_THIS 	= "#KEY_COMBINER_STUB_WITH_THIS#";
		public const string KEY_COMBINER_STUB_WITHOUT_THIS  = "#KEY_COMBINER_STUB_WITHOUT_THIS#";

		public const string LITERAL_KEY_ARG_SEP = ", ";
		public const string LITERAL_KEY_COMBINER_SEP = " + \u0022^\u0022 + ";

		public const string TABLE_IN_4_SPACE = "    ";
		public const int DECLARE_INDENT = 16;

		public delegate string MetaTypeParser(string semanticInput);
		private static Dictionary<string, MetaTypeParser> metaTypeParsers = new Dictionary<string, MetaTypeParser>()
		{
			{ "int", 	delegate(string any) { return "strconv.Atoi(" + any + ")"; } },
			{ "float",  delegate(string any) { return "strconv.ParseFloat(" + any + ", 64)"; } },
			{ "bool",  delegate(string any) { return "strconv.ParseBool(" + any + ")"; } },
			{ "string", delegate(string any) { return any; } }
		};
		private static Dictionary<string, MetaTypeParser> metaType2Key = new Dictionary<string, MetaTypeParser>()
		{
			{ "int", 	delegate(string any) { return "strconv.Itoa(" + any + ")"; } },
			{ "string", delegate(string any) { return any; } }
		};
		private static Dictionary<string, string> metaTypeSugers = new Dictionary<string, string>()
		{
			{ "int", 	", _" },
			{ "float",  ", _" },
			{ "bool",  ", _" },
			{ "string", "" }
		};

		public string GetCodeFileName(string csvName)
		{
			return csvName + ".go";
		}
		public string GetCodeFileContent(string clsName, string nameSpaceName, CSVParser.Output arg)
		{
			Console.WriteLine("Generating Golang GDS code : " + clsName);
			StreamReader fs = File.OpenText("../src/GDSTool/go.code");
			string template = fs.ReadToEnd();

			StringBuilder sb = new StringBuilder(template);
			sb.Replace(NAMESPACE_STUB, clsName);
			sb.Replace(CLASSNAME_STUB, Utils.underline2Pascal(clsName));
			
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
				{ COMPOSE_DECLARE_STUB, new StringBuilder() },
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
				string fieldName = arg.fieldNames[i];

				fieldName = Utils.underline2Pascal(fieldName);

				bool isKey = !String.IsNullOrEmpty(arg.isKeyFlags[i]);
				if (!isComposedType(metaType) && !metaTypeParsers.ContainsKey(metaType))
				{
					throw new Exception("unknown field meta type : " + metaType);
				}

				if (isComposedType(metaType))
				{
					AddStructDeclare(ret[COMPOSE_DECLARE_STUB], fieldName, metaType);
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
						AddKeyCombiner(ret[KEY_COMBINER_STUB_WITH_THIS], fieldName, metaType, true);
						AddKeyCombiner(ret[KEY_COMBINER_STUB_WITHOUT_THIS], fieldName, metaType, false);
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
			ret[KEY_COMBINER_STUB_WITH_THIS] = new StringBuilder(WrapWithSprint(ret[KEY_COMBINER_STUB_WITH_THIS].ToString()));

			Utils.TrimEnd(ret[KEY_COMBINER_STUB_WITHOUT_THIS], LITERAL_KEY_COMBINER_SEP.Length);
			ret[KEY_COMBINER_STUB_WITHOUT_THIS] = new StringBuilder(WrapWithSprint(ret[KEY_COMBINER_STUB_WITHOUT_THIS].ToString()));
			
			return ret;
		}

		private string WrapWithSprint(string str)
		{
			return "fmt.Sprint(" + str + ")";
		}

		private void AddStructDeclare(StringBuilder sb, string name, string type)
		{
			string[] names = name.Split(CSVParser.CSVStyle.ArrayElementSeperator);
			string[] types = GetMemberTypes(type);
			sb.Append("type Composed");
			sb.Append(Utils.underline2Pascal(names[0]));
			sb.Append(" struct {\n");

			for (int i = 0; i < types.Length; ++i)
			{
				string n = Utils.underline2Pascal(names[i + 1]);
				sb.Append(TABLE_IN_4_SPACE);
				sb.Append(n);

				AppendNeatIndent(sb, n.Length);

				sb.Append(MetaTypeConverter(types[i]));
				sb.Append('\n');
			}

			sb.Append("}\n\n");
		}

		private void AddArrayFieldDeclare(StringBuilder sb, string name, string type)
		{
			AddFieldDeclare(sb, name, type, true);
		}

		private void AddMetaFieldDeclare(StringBuilder sb, string name, string type)
		{
			AddFieldDeclare(sb, name, type, false);
		}

		private void AddFieldDeclare(StringBuilder sb, string name, string type, bool isArray)
		{
			if (!isComposedType(type))
			{
				sb.Append(TABLE_IN_4_SPACE);
				sb.Append(name);

				AppendNeatIndent(sb, name.Length);

				if (isArray) sb.Append("[]");
				sb.Append(MetaTypeConverter(type));
				sb.Append("\n");
			}
			else
			{
				AddStructFieldDeclare(sb, name, type, isArray);
			}
		}

		private void AddStructFieldDeclare(StringBuilder sb, string name, string type, bool isArray)
		{
			string[] names = name.Split(CSVParser.CSVStyle.ArrayElementSeperator);

			sb.Append(TABLE_IN_4_SPACE);
			sb.Append(names[0]);
			AppendNeatIndent(sb, names[0].Length);
			if (isArray) sb.Append("[]");
			sb.Append("Composed");
			sb.Append(Utils.underline2Pascal(names[0]));
			sb.Append("\n\n");
		}

		private void AddMetaFieldInitializer(StringBuilder sb, string name, string type, int index)
		{
			if (!isComposedType(type))
			{
				sb.Append(TABLE_IN_4_SPACE);
				sb.Append("_ret_.");
				sb.Append(name);
				sb.Append(metaTypeSugers[type]);
				sb.Append(" = ");
				sb.Append(metaTypeParsers[type]("strings.TrimSpace(_data_[" + index + "].(string))"));
				sb.Append("\n\n");
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

			sb.Append(TABLE_IN_4_SPACE);
			sb.Append("_data_");
			sb.Append(names[0]);
			sb.Append(" := _data_[" + index + "].([]interface{})\n");

			for (int i = 0; i < types.Length; ++i)
			{
				sb.Append(TABLE_IN_4_SPACE);
				sb.Append("_ret_.");
				sb.Append(names[0]);
				sb.Append('.');
				sb.Append(Utils.underline2Pascal(names[i + 1]));
				sb.Append(metaTypeSugers[types[i]]);
				sb.Append(" = ");
				sb.Append(metaTypeParsers[types[i]]("_data_" + names[0] + "[" + i + "].(string)"));
				sb.Append('\n');
			}

			sb.Append("\n");
		}

		private void AddArrayFieldInitializer(StringBuilder sb, string name, string type, int index)
		{
			if (!isComposedType(type))
			{
				sb.Append(TABLE_IN_4_SPACE);
				sb.Append("_data_" + name);
				sb.Append(" := _data_[" + index + "].([]interface{})\n");
				sb.Append(TABLE_IN_4_SPACE);

				sb.Append("_ret_.");
				sb.Append(name);
				sb.Append(" = make([]");
				sb.Append(type);
				sb.Append(", len(_data_");
				sb.Append(name);
				sb.Append("))\n");

				sb.Append(TABLE_IN_4_SPACE);
				sb.Append("for _i_, _v_ := range _data_");
				sb.Append(name);
				sb.Append(" {\n");

				sb.Append(TABLE_IN_4_SPACE);
				sb.Append(TABLE_IN_4_SPACE);
				sb.Append("_ret_.");
				sb.Append(name);
				sb.Append("[_i_]");
				sb.Append(metaTypeSugers[type]);
				sb.Append(" = ");
				sb.Append(metaTypeParsers[type]("_v_.(string)"));
				sb.Append('\n');
				sb.Append(TABLE_IN_4_SPACE);
				sb.Append("}\n\n");
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
			
			string data_name = "_data_" + names[0];
			string struct_name = "Composed" + Utils.underline2Pascal(names[0]);

			sb.Append(TABLE_IN_4_SPACE);
			sb.Append(data_name);
			sb.Append(" := _data_[");
			sb.Append(index);
			sb.Append("].([]interface{})\n");

			sb.Append(TABLE_IN_4_SPACE);
			sb.Append("_ret_.");
			sb.Append(names[0]);
			sb.Append(" = make([]");
			sb.Append(struct_name);
			sb.Append(", len(" + data_name + "))\n");

			sb.Append(TABLE_IN_4_SPACE);
			sb.Append("for _i_, __v_ := range " + data_name + " {\n");

			sb.Append(TABLE_IN_4_SPACE);
			sb.Append(TABLE_IN_4_SPACE);
			sb.Append("_v_ := __v_.([]interface{})\n");

			sb.Append(TABLE_IN_4_SPACE);
			sb.Append(TABLE_IN_4_SPACE);
			sb.Append("if len(_v_) == 0 {\n");

			sb.Append(TABLE_IN_4_SPACE);
			sb.Append(TABLE_IN_4_SPACE);
			sb.Append(TABLE_IN_4_SPACE);
			sb.Append("continue\n");

			sb.Append(TABLE_IN_4_SPACE);
			sb.Append(TABLE_IN_4_SPACE);
			sb.Append("}\n");

			for (int i = 0; i < types.Length; ++i)
			{
				sb.Append(TABLE_IN_4_SPACE);
				sb.Append(TABLE_IN_4_SPACE);
				sb.Append("_ret_." + names[0] + "[_i_].");
				sb.Append(Utils.underline2Pascal(names[i + 1]));
				sb.Append(metaTypeSugers[types[i]]);
				sb.Append(" = ");
				sb.Append(metaTypeParsers[types[i]]("_v_[" + i + "].(string)"));
				sb.Append('\n');
			}

			sb.Append(TABLE_IN_4_SPACE);
			sb.Append("}\n\n");
		}

		private void AddKeyArg(StringBuilder sb, string name, string type)
		{
			sb.Append(name);
			sb.Append(" ");
			sb.Append(type);
			sb.Append(LITERAL_KEY_ARG_SEP);
		}

		private void AddKeyCombiner(StringBuilder sb, string name, string type, bool withSelf)
		{
			string str = String.Empty;
			if (withSelf)
			{
				str = "_self_." + name;
			}
			else
			{
				str = name;
			}
			sb.Append(metaType2Key[type](str));
			sb.Append(LITERAL_KEY_COMBINER_SEP);
		}

		private string MetaTypeConverter(string t)
		{
			if ("float" == t)
			{
				return "float64";
			}
			return t;
		}

		private string[] GetMemberTypes(string composeType)
		{
			string tmp = composeType.Substring(1, composeType.Length - 2);
			return tmp.Split(CSVParser.CSVStyle.ArrayElementSeperator);
		}

		private void AppendNeatIndent(StringBuilder sb, int usedLen)
		{
			int len = Math.Max(1, DECLARE_INDENT - usedLen);
			for (int i = 0; i < len; ++i)
			{
				sb.Append(' ');
			}
		}
	}
}