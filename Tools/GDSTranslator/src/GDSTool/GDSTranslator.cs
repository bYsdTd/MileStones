using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GDSTool
{
	public class GDSTranslator
	{
		public class Input
		{
			public string[] dstLanguageNames;
			public string[] namespaceNames;
			public string[] csvNames;
			public string[] csvContents;
			public Dictionary<string, List<string>> ignoreFileNames = new Dictionary<string, List<string>>();
			public bool IsIgnored(string lng, string fileName)
			{
				return ignoreFileNames.ContainsKey(lng) && ignoreFileNames[lng].Contains(fileName);
			}
		}

		public class Output
		{
			public Dictionary<string, List<string>> codeFileNames;
			public Dictionary<string, List<string>> codeFileContents;
		}

		private Dictionary<string, ILiteralGenerator> codeGenerators = new Dictionary<string, ILiteralGenerator>()
		{
			{ "csharp", new CSharpGenerator() },
			{ "golang", new GolangGenerator() }
		};

		public Output Translate(Input input)
		{
			if (input.dstLanguageNames.Length != input.namespaceNames.Length)
			{
				throw new Exception("language and namespace length mismatch!");
			}
			if (input.csvNames.Length != input.csvContents.Length)
			{
				throw new Exception("csv file names and contents length mismatch!");
			}
			// precheck
			foreach (string lng in input.dstLanguageNames)
			{
				if (!codeGenerators.ContainsKey(lng))
				{
					throw new Exception("unknown language : " + lng);
				}
			}

			Output ret = new Output();
			ret.codeFileNames = new Dictionary<string, List<string>>();
			ret.codeFileContents = new Dictionary<string, List<string>>();

			for(int i = 0; i < input.dstLanguageNames.Length; ++i)
			{
				string lng = input.dstLanguageNames[i];
				string namespaceName = input.namespaceNames[i];
				ILiteralGenerator generator = codeGenerators[lng];

				List<string> fileNames = new List<string>();
				List<string> fileContents = new List<string>();

				for (int csvIdx = 0; csvIdx < input.csvNames.Length; ++csvIdx)
				{
					string csvFileName = input.csvNames[csvIdx];
					if (input.IsIgnored(lng, csvFileName))
					{
						continue;
					}
					string csvFileContent = input.csvContents[csvIdx];
					fileNames.Add(generator.GetCodeFileName(csvFileName));

					CSVParser.Output csvResult = CSVParser.Parse(csvFileContent, CSVParser.PARSE_TYPE_INFO);

					fileContents.Add(generator.GetCodeFileContent(csvFileName, namespaceName, csvResult));
				}

				ret.codeFileNames.Add(lng, fileNames);
				ret.codeFileContents.Add(lng, fileContents);
			}

			return ret;
		}
	}
}