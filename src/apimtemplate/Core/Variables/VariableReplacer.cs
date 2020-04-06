using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using Apim.DevOps.Toolkit.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Apim.DevOps.Toolkit
{
	internal class VariableReplacer
	{
		private static Lazy<VariableReplacer> _instance = new Lazy<VariableReplacer>(() => new VariableReplacer());
		private readonly Dictionary<string,string> _varsKeyValues= new Dictionary<string,string>();

		public static VariableReplacer Instance => _instance.Value;

		private VariableReplacer() { }

		public async Task LoadFromFile(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
				return;

			var fileReader = new FileReader();

			var variableValueKeyValues = await fileReader.GetReplacementVariablesFromYaml(filePath);

			var varsDic = GetVariables(variableValueKeyValues);

			AddVariables(varsDic);
		}

		/// <summary>
		/// variables separated by ;. example: a=1;b=2;
		/// </summary>
		/// <param name="variables"></param>
		/// <returns></returns>
		public Dictionary<string,string> LoadFromString(string variables)
		{
			if (string.IsNullOrEmpty(variables))
				return new Dictionary<string, string>();

			var variableValueKeyValues = variables.Split(";");

			return GetVariables(variableValueKeyValues);
		}

		/// <summary>
		/// variables separated by ;. example: a=1;b=2;
		/// </summary>
		/// <param name="variables"></param>
		/// <returns></returns>
		public Dictionary<string, string> GetVariables()
		{
			return _varsKeyValues;
		}


		/// <summary>
		/// variables separated by ;. example: a=1;b=2;
		/// </summary>
		/// <param name="variables"></param>
		/// <returns></returns>
		public void Load(string variables)
		{
			if (string.IsNullOrEmpty(variables))
				return;

			var variableValueKeyValues = variables.Split(";");

			var varsDic = GetVariables(variableValueKeyValues);

			AddVariables(varsDic);
		}

		public string ReplaceVariablesWithValues(string content, string localVariables=null)
		{
			var localVars = LoadFromString(localVariables);

			var mergedVariables = MergeLocalAndGlobalVariables(localVars);

			foreach (var kv in mergedVariables)
			{
				content = content.Replace(kv.Key, kv.Value);
			}

			return content;
		}

		private Dictionary<string,string> MergeLocalAndGlobalVariables(Dictionary<string,string> localVariables)
		{
			var mergedVariables = new Dictionary<string, string>(_varsKeyValues);

			foreach (var kv in localVariables)
			{
				mergedVariables[kv.Key] =  kv.Value;
			}

			return mergedVariables;
		}

		private void AddVariables(Dictionary<string, string> varsDic)
		{
			foreach (var kv in varsDic)
			{
				_varsKeyValues[kv.Key] = kv.Value;
			}
		}

		private Dictionary<string,string> GetVariables(string[] variableValueKeyValues)
		{
			var result = new Dictionary<string,string>();

			foreach (var variableValueKeyValue in variableValueKeyValues)
			{
				var keyVal = variableValueKeyValue.CreateReplacementKeyValue();
				result[keyVal.Key] = keyVal.Value;
			}

			return result;
		}
	}
}
