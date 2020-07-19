using System.Collections.Generic;
using System.Linq;

namespace Apim.DevOps.Toolkit.Core.Variables
{
	public class VariableCollection
	{
		private Dictionary<string, Variable> _variableCollection;

		public IReadOnlyCollection<Variable> Variables => _variableCollection.Values;

		public VariableCollection()
		{
			_variableCollection = new Dictionary<string, Variable>();
		}

		public VariableCollection(IEnumerable<Variable> variables)
		{
			_variableCollection = variables.ToDictionary(variable => variable.Key);
		}

		public void Add(Variable variable)
		{
			_variableCollection.Add(variable.Key, variable);
		}
		public bool ContainsKey(string variableKey)
		{
			return _variableCollection.ContainsKey(variableKey);
		}

		public VariableCollection Merge(VariableCollection variableCollection)
		{
			return this.Merge(variableCollection.Variables);
		}

		public VariableCollection Merge(IEnumerable<Variable> variables)
		{
			var newVariableCollection = this._variableCollection.ToDictionary(kv => kv.Key, kv => kv.Value);

			foreach (var variable in variables)
			{
				newVariableCollection[variable.Key] = variable;
			}

			return new VariableCollection(newVariableCollection.Values);
		}
	}
}
