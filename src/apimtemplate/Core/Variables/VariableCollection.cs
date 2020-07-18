using System.Collections.Generic;
using System.Linq;

namespace Apim.DevOps.Toolkit.Core.Variables
{
	public class VariableCollection
	{
		private Dictionary<string, Variable> _variableDict;

		public IReadOnlyCollection<Variable> Variables => _variableDict.Values;

		public VariableCollection()
		{
			_variableDict = new Dictionary<string, Variable>();
		}

		public VariableCollection(IEnumerable<Variable> variables)
		{
			_variableDict = variables.ToDictionary(variable => variable.Key);
		}

		public void Add(Variable variable)
		{
			_variableDict.Add(variable.Key, variable);
		}
		public bool ContainsKey(string variableKey)
		{
			return _variableDict.ContainsKey(variableKey);
		}

		public VariableCollection Merge(VariableCollection variableCollection)
		{
			return this.Merge(variableCollection.Variables);
		}

		public VariableCollection Merge(IEnumerable<Variable> variables)
		{
			var newCollection = this._variableDict.ToDictionary(kv => kv.Key, kv => kv.Value);

			foreach (var variable in variables)
			{
				newCollection[variable.Key] = variable;
			}

			return new VariableCollection(newCollection.Values);
		}
	}
}
