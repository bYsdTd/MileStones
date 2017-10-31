using System;

namespace GDSTool
{
	interface ILiteralGenerator
	{
		string GetCodeFileName(string csvName);
		string GetCodeFileContent(string clsName, string nameSpaceName, CSVParser.Output arg);
	}
}