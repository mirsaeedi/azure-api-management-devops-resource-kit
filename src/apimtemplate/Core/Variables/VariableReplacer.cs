using System.Threading.Tasks;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Apim.DevOps.Toolkit.Core.Infrastructure;

namespace Apim.DevOps.Toolkit.Core.Variables
{
	internal class VariableReplacer
	{
		private static string _variableKeyRegexPattern = @"\$\([a-zA-Z][a-zA-Z0-9]+\)";
		private static Lazy<VariableReplacer> _instance = new Lazy<VariableReplacer>(() => new VariableReplacer());

		private readonly FileReader _fileReader = new FileReader();

		private VariableCollection _variableCollection = new VariableCollection();

		public static VariableReplacer Instance => _instance.Value;
		public IReadOnlyCollection<Variable> Variables => this._variableCollection.Variables;

		private VariableReplacer() { }

		public async Task LoadFromFile(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
				return;

			var variableCollection = await _fileReader.GetVariablesFromYaml(filePath);
			_variableCollection = _variableCollection.Merge(variableCollection);
		}

		/// <summary>
		/// variables separated by ;. example: a=1;b=2;
		/// </summary>
		/// <param name="variables"></param>
		/// <returns></returns>
		public void LoadFromString(string variables)
		{
			var variableCollection = GetFromString(variables);
			_variableCollection = this._variableCollection.Merge(variableCollection);
		}

		/// <summary>
		/// variables separated by ;. example: a=1;b=2;
		/// </summary>
		/// <param name="variables"></param>
		/// <returns></returns>
		public VariableCollection GetFromString(string variables)
		{
			if (string.IsNullOrEmpty(variables))
				return new VariableCollection();

			if(variables[0] == '{' && variables[variables.Length - 1] == '}')
			{
				// Use System.Range when drop support for .NET Core 2.1.
				variables = variables.Substring(1, variables.Length - 2);
			}

			return new VariableCollection(variables.Split(";").Select(Variable.FromString));
		}

		public string ReplaceVariablesWithValues(string content, string localVariables = null)
		{
			var localVariableCollection = GetFromString(localVariables);

			var overridenVariableCollection = this._variableCollection.Merge(localVariableCollection);

			Validate(content, overridenVariableCollection);

			foreach (var variable in overridenVariableCollection.Variables)
			{
				content = content.Replace(variable.Key, variable.Value);
			}

			return content;
		}

		private void Validate(string content, VariableCollection overridenVariableCollection)
		{
			var variableKeyMatches = Regex.Matches(content, _variableKeyRegexPattern);

			var validationFailed = false;
			foreach (Match variableKeyMatch in variableKeyMatches)
			{
				var variableKey = variableKeyMatch.Value;
				if (!overridenVariableCollection.ContainsKey(variableKey))
				{
					validationFailed = true;
					Console.Error.WriteLine($"There is no value defined for the variable {variableKey}");
				}
			}

			if (validationFailed)
			{
				throw new InvalidOperationException("There should be a value assigned to all variables");
			}
		}
	}
}
