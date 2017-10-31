using System;
using System.IO;

namespace GDSTool
{
	class MainClass
	{
		public static void Main (string[] args)
		{
//			if (false)
//			{
//				args = new string[8];
//				args[0] = "--csvPath";
//				args[1] = "../test/csv";
//				args[2] = "--dstPath";
//				args[3] = "../test/output";
//				args[4] = "--languages";
//				args[5] = "csharp|golang";
//				args[6] = "--namespaces";
//				args[7] = "GDSKit|main";
//			}
			GDSTranslatorShellWrapper wrapper = new GDSTranslatorShellWrapper();
			wrapper.DoTranslate(args);
		}
	}
}
