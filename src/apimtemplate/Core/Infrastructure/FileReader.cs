using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using Apim.DevOps.Toolkit.Core.Configuration;
using Apim.DevOps.Toolkit.Extensions;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Apim.DevOps.Toolkit.Core.Variables;

namespace Apim.DevOps.Toolkit.Core.Infrastructure
{
	public class FileReader
	{
		private static readonly HttpClient _httpClient = new HttpClient();

		public async Task<VariableCollection> GetVariablesFromYaml(string variablesFilePath)
		{
			if (string.IsNullOrEmpty(variablesFilePath))
			{
				return new VariableCollection();
			}

			var content = await File.ReadAllTextAsync(variablesFilePath);
			var deserializer = new Deserializer();

			var keyValues = deserializer.Deserialize<string[]>(content);
			var variables = keyValues.Select(kv => Variable.FromString(kv));

			return new VariableCollection(variables);
		}

		public async Task<DeploymentDefinition> GetDeploymentDefinitionFromYaml(string deploymentDefinitionFilePath)
		{
			var content = await RetrieveFileContentsAsync(deploymentDefinitionFilePath);
			content = VariableReplacer.Instance.ReplaceVariablesWithValues(content);

			return GetDeploymentDefinition(content);
		}

		public async Task<string> RetrieveFileContentsAsync(string fileLocation, bool convertToBase64 = false)
		{
			var parts = fileLocation.Split(":::");
			fileLocation = parts[0];

			var isUrl = fileLocation.IsUri(out var uriResult);

			if (isUrl)
			{
				var response = await _httpClient.GetAsync(uriResult).ConfigureAwait(false);

				if (!response.IsSuccessStatusCode)
				{
					throw new Exception($"Unable to fetch remote file - {fileLocation}");
				}

				return await response.Content.ReadAsStringAsync();
			}

			var localVariables = parts.Length == 2 ? parts[1] : null;

			if (convertToBase64)
			{
				return Convert.ToBase64String(await File.ReadAllBytesAsync(fileLocation));
			}

			var content = await File.ReadAllTextAsync(fileLocation);
			var replacedContent = VariableReplacer.Instance.ReplaceVariablesWithValues(content, localVariables);

			return EvaluateExpressions(replacedContent);
		}

		private string EvaluateExpressions(string replacedContent)
		{
			var lines = replacedContent.Split(Environment.NewLine);
			var evaluatedLines = new List<string>();
			var stackIf = new Stack<bool>();
			stackIf.Push(true);

			foreach (var line in lines)
			{
				var shouldInclude = stackIf.Peek();

				if (Regex.IsMatch(line, @"^((\s)*#if(\s)+true(\s)*)$"))
				{
					stackIf.Push(true);
				}
				else if (Regex.IsMatch(line, @"^((\s)*#if(\s)+false(\s)*)$"))
				{
					stackIf.Push(false);
				}
				else if (Regex.IsMatch(line, @"^((\s)*#endif(\s)*)$"))
				{
					stackIf.Pop();
				}
				else if (shouldInclude)
				{
					var evaluatedLine = EvaluateLine(line);
					evaluatedLines.Add(evaluatedLine);
				}
			}

			var sb = new System.Text.StringBuilder();

			foreach (var line in evaluatedLines)
			{
				sb.AppendLine(line);
			}

			return sb.ToString();
		}

		private string EvaluateLine(string line)
		{
			var matchCollectionTrue = Regex.Matches(line, @"#if(\s)+true(\s)+(?<content>\w+)(\s)+#endif");

			foreach (Match match in matchCollectionTrue)
			{
				line = line.Replace(match.Groups[0].Value, match.Groups["content"].Value);
			}

			var matchCollectionFalse = Regex.Matches(line, @"#if(\s)+false(\s)+(?<content>\w+)(\s)+#endif");

			foreach (Match match in matchCollectionFalse)
			{
				line = line.Replace(match.Groups[0].Value, "");
			}

			return line;
		}

		private DeploymentDefinition GetDeploymentDefinition(string yamlContent)
		{
			var deserializer = new Deserializer();
			object deserializedYaml = deserializer.Deserialize<object>(yamlContent);
			object deserializedYaml2 = deserializer.Deserialize<DeploymentDefinition>(yamlContent);

			var jsonSerializer = new JsonSerializer();

			using (var writer = new StringWriter())
			{
				jsonSerializer.Serialize(writer, deserializedYaml);
				string jsonText = writer.ToString();
				var deploymentDefinition = JsonConvert.DeserializeObject<DeploymentDefinition>(jsonText);

				var isConfigCreatorValid = IsDeploymentDefinitionValid(deploymentDefinition);

				return deploymentDefinition;
			}
		}

		private bool IsDeploymentDefinitionValid(DeploymentDefinition deploymentDefinition)
		{
			var DeploymentDefinitionValidator = new DeploymentDefinitionValidator();

			return DeploymentDefinitionValidator.Validate(deploymentDefinition);
		}
	}
}
