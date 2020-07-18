using System;
using System.Text.RegularExpressions;

namespace Apim.DevOps.Toolkit.Core.Variables
{
	public class Variable
	{
		private static string _variableRegexPattern = "(?<key>[a-zA-Z][a-zA-Z0-9]+)=(?<value>.+)";
		public string Key { get; set; }

		public string Value { get; set; }

		public static Variable FromString(string keyValue)
		{
			var regex = new Regex(_variableRegexPattern, RegexOptions.IgnoreCase);
			var match = regex.Match(keyValue);

			if (!match.Success)
			{
				throw new ArgumentException($"the variable {keyValue} does not have valid format");
			}

			return new Variable
			{
				Key = $"$({match.Groups["key"].Value})",
				Value = match.Groups["value"].Value,
			};
		}
	}
}
