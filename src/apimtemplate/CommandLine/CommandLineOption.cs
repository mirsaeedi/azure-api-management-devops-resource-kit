using CommandLine;

namespace Apim.DevOps.Toolkit.CommandLine
{
	public class CommandLineOption
	{
		[Option('c', "yamlConfig", Required = true, HelpText = "Config Yaml file location (Local | Url)")]
		public string YamlConfigPath { get; set; }

		[Option('f', "variableFile", Required = false, HelpText = "Variable definition file location (Local | Url).")]
		public string VariableFilePath { get; set; }

		[Option('s', "variableString", Required = false, HelpText = "Variable definition string.")]
		public string VariableString { get; set; }

		[Option('p', "armPrefix", Required = false, HelpText = "Prefix of generated arm files.")]
		public string FileNamePrefix { get; set; } = "";

		[Option('m', "master", Required = false, HelpText = "Name of the master template file.")]
		public string MasterFileName { get; set; }

		[Option('v', "printVariables", Required = false, HelpText = "Print loaded variables before generating arm templates.")]
		public bool PrintVariables { get; set; }

		[Option('o', "output", Required = true, HelpText = "Output file location.")]
		public string OutputPath { get; set; }
	}
}
