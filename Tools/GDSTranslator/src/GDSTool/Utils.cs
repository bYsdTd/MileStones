using System;
using System.Text;

namespace GDSTool
{
	public class Utils
	{
		public static void TrimEnd(StringBuilder sb, int count)
		{
			sb.Remove(sb.Length - count, count);
		}

		public static string underline2Camel(string str)
		{
			return underlineHandler(str, 1);
		}

		public static string underline2Pascal(string str)
		{
			return underlineHandler(str, 0);
		}

		private static string underlineHandler(string str, int offset)
		{
			StringBuilder sb = new StringBuilder();
			if (!str.Contains("_"))
			{
				sb.Append(str.Substring(0, 1).ToUpper());
				sb.Append(str.Substring(1));
				return sb.ToString();
			}
			string[] parts = str.Split('_');
			int len = parts.Length;
			for (int i = 0; i < len; ++i)
			{
				if (String.IsNullOrEmpty(parts[i]))
				{
					sb.Append('_');
				}
				else
				{
					string s = parts[i].ToLower();
					if (--offset < 0)
					{
						if (1 == s.Length)
						{
							sb.Append(s.ToUpper());
						}
						else
						{
							sb.Append(s.Substring(0, 1).ToUpper());
							sb.Append(s.Substring(1));
						}
					}
					else
					{
						sb.Append(s);
					}
				}
			}
			return sb.ToString();
		}
	}
}

