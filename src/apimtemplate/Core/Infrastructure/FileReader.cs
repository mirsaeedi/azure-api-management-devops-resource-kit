using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create;
using Apim.DevOps.Toolkit.Extensions;
using System.Collections.Generic;
using Apim.DevOps.Toolkit;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class FileReader
    {
        private HttpClient _httpClient = new HttpClient();

        public async Task<string[]> GetReplacementVariablesFromYaml(string replacementVariablesFilePath)
        {
            if (string.IsNullOrEmpty(replacementVariablesFilePath))
            {
                return new string[0];
            }

            var content = await RetrieveFileContentsAsync(replacementVariablesFilePath);
            var deserializer = new Deserializer();
            var replacementVariables = deserializer.Deserialize<string[]>(content);

            return replacementVariables;
        }

        public async Task<DeploymentDefinition> GetCreatorConfigFromYaml(string configFilePath)
        {
            var content = await RetrieveFileContentsAsync(configFilePath);

			content = VariableReplacer.Instance.ReplaceVariablesWithValues(content);

            return GetCreatorConfig(content);
        }

		public async Task<string> RetrieveFileContentsAsync(string fileLocation, bool convertToBase64=false)
		{
			string result = null;

			var parts = fileLocation.Split(":::");
			fileLocation = parts[0];

			var isUrl = fileLocation.IsUri(out var uriResult);

			if (!isUrl)
			{
				var localVariables = parts.Length == 2 ? parts[1] : null; 
				var content = await File.ReadAllTextAsync(fileLocation);
				var replacedContent = VariableReplacer.Instance.ReplaceVariablesWithValues(content,localVariables);
				var interpretedContent = EvaluateExpressions(replacedContent);

				result =  interpretedContent;
			}
			else
			{
				var response = await _httpClient.GetAsync(uriResult).ConfigureAwait(false);

				if (!response.IsSuccessStatusCode)
				{
					throw new Exception($"Unable to fetch remote file - {fileLocation}");
				}

				result = await response.Content.ReadAsStringAsync();
			}

			if (convertToBase64)
			{
				result = GetBase64(result);
			}

			return result;
		}

		private string GetBase64(string content)
		{
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(content);
			return Convert.ToBase64String(plainTextBytes);
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
				else if(shouldInclude)
				{
					var evaluatedLine = EvaluateLine(line);
					evaluatedLines.Add(evaluatedLine);
				}
			}

			var sb = new System.Text.StringBuilder();

			foreach(var line in evaluatedLines)
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

		private DeploymentDefinition GetCreatorConfig(string yamlContent)
        {
            var deserializer = new Deserializer();
            object deserializedYaml = deserializer.Deserialize<object>(yamlContent);
           
            var jsonSerializer = new JsonSerializer();

            using (var writer = new StringWriter())
            {
                jsonSerializer.Serialize(writer, deserializedYaml);
                string jsonText = writer.ToString();
                var creatorConfig = JsonConvert.DeserializeObject<DeploymentDefinition>(jsonText);

				var isConfigCreatorValid = IsCreatorConfigValid(creatorConfig);

				return creatorConfig;
            }
        }

		private bool IsCreatorConfigValid(DeploymentDefinition creatorConfig)
		{
			var creatorConfigurationValidator = new ConfigurationValidator();
			bool isValidCreatorConfig = creatorConfigurationValidator.Validate(creatorConfig);
			return isValidCreatorConfig;
		}
	}
}
