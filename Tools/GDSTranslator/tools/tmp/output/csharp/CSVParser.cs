using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GDSKit
{
	public class CSVParser
	{
		public const int PARSE_TYPE_INFO 	= 0x1;
		public const int PARSE_DATA			= 0x2;
		public const int PARSE_ALL			= PARSE_TYPE_INFO & PARSE_DATA;

		public class CSVStyle
		{
			public static readonly string[] lineSeperators = { "\r\n", "\r", "\n" };
			public const char ColumnSeperator = ',';
			public const string CommentHead = "#";
			public const string ArrayBrackets = "[]";
			public const char ComposeStarter = '[';
			public const char ComposeTerminator = ']';
			public const char ArrayElementSeperator = '|';
			public const char ComposeSeperator = '|';
			public static readonly char[] NeedTrim = { ' ', '\t' };
		}

		public class Output
		{
			public string[] fieldNames;
			public string[] fieldMetaTypes;
			public string[] fieldOrganizeTypes;
			public string[] isKeyFlags;
			public List<string[]> data;
		}

		public static Output Parse(string input, int parseFlag)
		{
			Output ret = new Output();
			List<List<string>> csvFields = CSVStandardParse(input);

			int current = 0;
			if ((parseFlag & PARSE_TYPE_INFO) > 0)
			{
				ret.fieldNames = csvFields[current].ToArray();
			}
			++current;

			if ((parseFlag & PARSE_TYPE_INFO) > 0)
			{
				ret.fieldMetaTypes = csvFields[current].ToArray();
			}
			++current;

			if ((parseFlag & PARSE_TYPE_INFO) > 0)
			{
				int len = ret.fieldMetaTypes.Length;
				ret.fieldOrganizeTypes = new string[len];
				for (int i = 0; i < len; ++i)
				{
					string t = ret.fieldMetaTypes[i];
					if (t.EndsWith(CSVStyle.ArrayBrackets))
					{
						ret.fieldMetaTypes[i] = t.Substring(0, t.Length - CSVStyle.ArrayBrackets.Length);
						ret.fieldOrganizeTypes[i] = "Array";
					}
					else
					{
						ret.fieldOrganizeTypes[i] = "Meta";
					}
				}
			}

			++current; //skip the comments

			if ((parseFlag & PARSE_TYPE_INFO) > 0)
			{
				ret.isKeyFlags = csvFields[current].ToArray();
			}
			++current;

			if ((parseFlag & PARSE_DATA) > 0)
			{
				ret.data = new List<string[]>();
				int length = csvFields.Count;

				while (current < length)
				{
					string head = csvFields[current][0];
					if (head.Contains("0"))
					{
						++current;
						break;
					}
					else if (current == (length - 1))
					{
						throw new Exception("No content seperator found!");
					}
					++current;
				}

				while (current < length)
				{
					ret.data.Add(csvFields[current].ConvertAll((str)=>{
						return str.Trim(CSVStyle.NeedTrim);
					}).ToArray());
					++current;
				}
			}

			return ret;
		}

		enum ParseState
		{
			FieldStart,
			NonQuoteField,
			QuoteField,
			Error
		}
		
		private delegate void EndLine();
		private delegate void EndField();
		private delegate char NextCharGetter(int idx);
		
		public static List<List<string>> CSVStandardParse(string content)
		{
			List<List<string>> ret = new List<List<string>>();
			ParseState state = ParseState.FieldStart;
			
			const char comma = ',';
			const char quote = '"';
			const char newline1 = '\r';
			const char newline2 = '\n';

			bool appendNewlineAtEnd = true;
			if (content.Length > 0)
			{
				var last = content[content.Length - 1];
				if (last == newline1 || last == newline2)
					appendNewlineAtEnd = false;
			}

			int len = content.Length;
			if (appendNewlineAtEnd)
			{
				++len;
			}
			
			List<string> currentLine = new List<string>();
			StringBuilder sb = new StringBuilder();
			
			EndLine end_line = delegate() {
				currentLine.Add(sb.ToString());
				sb.Length = 0;
				ret.Add(currentLine);
				currentLine = new List<string>();
			};
			
			EndField end_field = delegate() {
				currentLine.Add(sb.ToString());
				sb.Length = 0;
			};

			NextCharGetter next_char = delegate(int idx) {
				if (appendNewlineAtEnd && (idx == (len - 1)))
				{
					return newline1;
				}
				else
				{
					return content[idx];
				}
			};



			for (int i = 0; i < len; ++i)
			{
				char c = next_char(i);
				
				switch (state)
				{
				case ParseState.FieldStart:
				{
					switch (c)
					{
					case comma:
					{
						end_field();
					}
						break;
					case quote:
					{
						state = ParseState.QuoteField;
					}
						break;
					case newline1:
					{
						if (((i + 1) < len) && newline2 == content[i + 1])
						{
							++i;
						}
						end_line();
					}
						break;
					case newline2:
					{
						end_line();
					}
						break;
					default:
					{
						sb.Append(c);
						state = ParseState.NonQuoteField;
					}
						break;
					}
				}
					break;
				case ParseState.NonQuoteField:
				{
					switch (c)
					{
					case comma:
					{
						end_field();
						state = ParseState.FieldStart;
					}
						break;
					case quote:
					{
						state = ParseState.Error;
					}
						break;
					case newline1:
					{
						if (((i + 1) < len) && newline2 == content[i + 1])
						{
							++i;
						}
						end_line();
						state = ParseState.FieldStart;
					}
						break;
					case newline2:
					{
						end_line();
						state = ParseState.FieldStart;
					}
						break;
					default:
					{
						sb.Append(c);
					}
						break;
					}
				}
					break;
				case ParseState.QuoteField:
				{
					switch (c)
					{
					case quote:
					{
						if ((i + 1) >= len)
						{
							end_line();
						}
						else
						{
							if (quote == content[i + 1])
							{
								sb.Append(quote);
								++i;
							}
							else if (comma == content[i + 1])
							{
								end_field();
								++i;
								state = ParseState.FieldStart;
							}
							else if (newline1 == content[i + 1])
							{
								if ((i + 2) >= len)
								{
									state = ParseState.Error;
								}
								else
								{
									if (newline2 == content[i + 2])
									{
										end_line();
										++i;
										++i;
										state = ParseState.FieldStart;
									}
									else
									{
										end_line();
										++i;
										state = ParseState.FieldStart;
									}
								}
							}
							else if (newline2 == content[i + 1])
							{
								end_line();
								++i;
								state = ParseState.FieldStart;
							}
							else
							{
								state = ParseState.Error;
							}
						}
					}
						break;
					default:
					{
						sb.Append(c);
					}
						break;
					}
				}
					break;
				case ParseState.Error:
				{
					throw new Exception("invalid csv format!");
				}
				default:
					break;
				}
			}
			
			if (ParseState.Error == state)
			{
				throw new Exception("invalid csv format!");
			}
			
			return ret;
		}

		public static int GetAsInt(string str)
		{   
			if (String.IsNullOrEmpty(str))
				return default(int);
			return int.Parse(str);
		}   
		
		public static float GetAsFloat(string str)
		{   
			if (String.IsNullOrEmpty(str))
				return default(float);
			return float.Parse(str);
		}   
		
		public static string GetAsString(string str)
		{   
			if (String.IsNullOrEmpty(str))
				return String.Empty;
			return str;
		}

		public static bool GetAsBool(string str)
		{   
			if (String.IsNullOrEmpty(str))
				return false;
			return bool.Parse(str);
		}
		
		public static string[] GetAsMetaArray(string str)
		{   
			if (String.IsNullOrEmpty(str))
				return new string[0];
			if (str[0] == CSVStyle.ComposeStarter)
				throw new NotSupportedException();
			return str.Split(CSVStyle.ArrayElementSeperator);
		}  

		public static string[] GetAsStructData(string str)
		{
			if (String.IsNullOrEmpty(str))
				throw new NotSupportedException();
			if (str[0] != CSVStyle.ComposeStarter || str[str.Length - 1] != CSVStyle.ComposeTerminator)
				throw new NotSupportedException();
			return str.Substring(1, str.Length - 2).Split(CSVStyle.ArrayElementSeperator);
		}

		private static string[] StructArraySpliter = new string[1] { "]|[" };
		public static string[] GetAsStructArray(string str)
		{
			if (String.IsNullOrEmpty(str))
				return new string[0];
			if (str[0] != CSVStyle.ComposeStarter || str[str.Length - 1] != CSVStyle.ComposeTerminator)
				throw new NotSupportedException();
			return str.Substring(1, str.Length - 2).Split(StructArraySpliter, StringSplitOptions.None);
		}
	}
}