using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GDSTool
{
	public class GDSTranslatorShellWrapper
	{
		private string csvFileDirectory = "";
		private string outputFileDirectory = "";
		private GDSTranslator.Input inputToTranslator = new GDSTranslator.Input();

		enum CodeFileDirStyle
		{
			ALL_IN_ONE,
			SEPARATE
		}
		private static Dictionary<string, CodeFileDirStyle> dirStyle = new Dictionary<string, CodeFileDirStyle>()
		{
			{ "csharp", CodeFileDirStyle.ALL_IN_ONE },
			{ "golang", CodeFileDirStyle.SEPARATE }
		};

		delegate void CfgHandler(string cfgValue);
		Dictionary<string, CfgHandler> cfgHandlerDict = new Dictionary<string, CfgHandler>();

		Dictionary<string, GDSPostprocessor> postprocessors = new Dictionary<string, GDSPostprocessor>()
		{
			{ "csharp", new CSharpGDSPostprocessor() }
		};

		public void DoTranslate(string[] args)
		{
			try
			{
				InitCfgHandlers ();

				if (IsNoArgs(args))
				{
					ShowTutorial();
					return;
				}

				ParseArgsToTranslatorInput(args);

				LoadIgnoreFileLists();

				GDSTranslator translator = new GDSTranslator();

				GDSTranslator.Output output = translator.Translate(inputToTranslator);

				foreach (string lng in output.codeFileNames.Keys)
				{
					if (postprocessors.ContainsKey(lng))
					{
						postprocessors[lng].Postprocess(inputToTranslator, output);
					}
				}

				inputToTranslator = null;

				WriteTranslateResult(output);
			}
			catch (FileNotFoundException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		private void InitCfgHandlers()
		{
			cfgHandlerDict["csvPath"] = SetCSVPath;
			cfgHandlerDict["dstPath"] = SetDstPath;
			cfgHandlerDict["csvFile"] = SetCSVFiles;
			cfgHandlerDict["languages"] = SetLanguages;
			cfgHandlerDict["namespaces"] = SetNamespaces;
		}

		private void SetCSVPath(string cfgValue)
		{
			csvFileDirectory = cfgValue;
		}

		private void SetDstPath(string cfgValue)
		{
			outputFileDirectory = cfgValue;
		}

		private void SetCSVFiles(string cfgValue)
		{
			inputToTranslator.csvNames = cfgValue.Split(',');
		}

		private void SetLanguages(string cfgValue)
		{
			inputToTranslator.dstLanguageNames = cfgValue.Split('|');
		}

		private void SetNamespaces(string cfgValue)
		{
			inputToTranslator.namespaceNames = cfgValue.Split('|');
		}

		private bool IsNoArgs(string[] args)
		{
			return 0 == args.Length;
		}

		private void ShowTutorial()
		{
			string tutorial = @"
GDSTranslator : Translator csv file to code in 
                varies language VO class or 
                analogue.

    --csvPath : specify the source path of csv 
                file, if NOT used with --csvFile, 
                all *.csv in this path will be used.

    --dstPath : specify the destination path of 
                the output code files. folder for 
                each lanugage will be created 
                if not exist, respectively.

    --csvFile : tell the target input files, 
                if NOT used with --csvPath, 
                only absolute paths will be 
                recognized as valid arguments. 
                Otherwise the arguments will 
                be combined with csvPath.

    --languages: the languages of output codes. 
                codes for different language will
                be put into their own folder.

    --namespaces : the namespace or package or 
                other QUOTEregionQUOTE indicator, 
                inevitable.

	example : GDSTool.exe --csvPath /Users/Yourname/test/csv
                          --dstPath /Users/Yourname/test/output 
                          --csvFile QUOTEBuilding.csv,Solider.csvQUOTE
                          --languages QUOTEcsharp|golangQUOTE
                          --namespaces QUOTETest|TestQUOTE
";
			Console.Write(tutorial.Replace("QUOTE", "\""));
		}

		private void ParseArgsToTranslatorInput(string[] args)
		{
			List<string> cfgs = new List<string>();
			List<string> values = new List<string>();

			int len = args.Length;
			string cfgPrefix = "--";
			for (int i = 0; i < len; ++i)
			{
				if (args[i].StartsWith(cfgPrefix))
				{
					cfgs.Add(args[i].Substring(cfgPrefix.Length));
					values.Add(args[i+1]);
				}
			}

			for (int i = 0; i < cfgs.Count; ++i)
			{
				string cfg = cfgs[i];
				string value = values[i];
				if (cfgHandlerDict.ContainsKey(cfg))
				{
					cfgHandlerDict[cfg](value);
				}
				else
				{
					throw new Exception("Unknown config token : " + cfg);
				}
			}

			MakeInput();
		}

		private static string IGNORE_LIST = "../src/GDSTool/ignores";
		private void LoadIgnoreFileLists()
		{
			if (File.Exists(IGNORE_LIST))
			{
				var content = File.OpenText(IGNORE_LIST).ReadToEnd();
				var cluster = CSVParser.CSVStandardParse(content);
				foreach (var lst in cluster)
				{
					var language = lst[0];
					lst.RemoveAt(0);
					inputToTranslator.ignoreFileNames.Add(language, lst);
				}
			}
		}

		private void MakeInput()
		{
			if (null == inputToTranslator.dstLanguageNames)
			{
				throw new Exception("No target languages!");
			}
			if (null == inputToTranslator.namespaceNames)
			{
				throw new Exception("No target namespaces!");
			}
			if (inputToTranslator.namespaceNames.Length != inputToTranslator.dstLanguageNames.Length)
			{
				throw new Exception("unequilong lanugages and namespaces");
			}
			if (String.IsNullOrEmpty(outputFileDirectory))
			{
				throw new Exception("No output path!");
			}
			Path.GetDirectoryName(outputFileDirectory);
			if (null == inputToTranslator.csvNames && String.IsNullOrEmpty(csvFileDirectory))
			{
				throw new Exception("No csvFile path!");
			}
			else if (String.IsNullOrEmpty(csvFileDirectory) && null != inputToTranslator.csvNames)
			{
				// Do nothing
			}
			else if (null == inputToTranslator.csvNames && !String.IsNullOrEmpty(csvFileDirectory))
			{
				inputToTranslator.csvNames = Directory.GetFiles(csvFileDirectory, "*.csv", SearchOption.AllDirectories);
			}
			else
			{
				int len = inputToTranslator.csvNames.Length;
				for (int i = 0; i < len; ++i)
				{
					inputToTranslator.csvNames[i] = Path.Combine(csvFileDirectory, inputToTranslator.csvNames[i]);
				}
			}

			int fileCount = inputToTranslator.csvNames.Length;
			inputToTranslator.csvContents = new string[fileCount];
			for (int i = 0; i < fileCount; ++i)
			{
				string absPath = inputToTranslator.csvNames[i];
				inputToTranslator.csvContents[i] = ReadCSVFile(absPath);
				inputToTranslator.csvNames[i] = Path.GetFileNameWithoutExtension(absPath);
			}
		}

		private string ReadCSVFile(string csvAbsolutePath)
		{
			if (!File.Exists(csvAbsolutePath))
			{
				throw new Exception("csv file not found : " + csvAbsolutePath);
			}
			StreamReader fs = File.OpenText(csvAbsolutePath);

			string ret = fs.ReadToEnd();

			fs.Close();

			return ret;
		}

		private void WriteTranslateResult(GDSTranslator.Output result)
		{
			foreach (KeyValuePair<string, List<string>> kvp in result.codeFileNames)
			{
				var style = CodeFileDirStyle.ALL_IN_ONE;
				if (dirStyle.ContainsKey(kvp.Key))
				{
					style = dirStyle[kvp.Key];
				}
				string dirPath = Path.Combine(outputFileDirectory, kvp.Key);
				if (Directory.Exists(dirPath))
				{
					Directory.Delete(dirPath, true);
				}
				Directory.CreateDirectory(dirPath);

				List<string> names = kvp.Value;
				List<string> contents = result.codeFileContents[kvp.Key];

				int len = names.Count;
				for (int i = 0; i < len; ++i)
				{
					switch (style)
					{
						case CodeFileDirStyle.SEPARATE:
						{
							var subDir = names[i].Split('.')[0];
							Directory.CreateDirectory(Path.Combine(dirPath, subDir));
						    WriteCodeFile(Path.Combine(dirPath, subDir, names[i]), contents[i]);
						}
						break;
						default:
						{
							WriteCodeFile(Path.Combine(dirPath, names[i]), contents[i]);
						}
						break;
					}
				}
			}
		}

		private void WriteCodeFile(string filePath, string fileContent)
		{
			FileStream fs = File.Create(filePath);
			StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
			sw.Write(fileContent);
			sw.Close();
			fs.Close();
		}
	}
}