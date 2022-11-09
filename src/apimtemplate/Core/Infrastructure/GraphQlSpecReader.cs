using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using Apim.DevOps.Toolkit.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apim.DevOps.Toolkit.Core.Infrastructure
{
	public class GraphQlSpecReader
	{
		private readonly FileReader _fileReader = new();
		private string _graphQlFilePath;
		public GraphQlSpecReader(string graphQlFilePath)
		{
			_graphQlFilePath = graphQlFilePath;
		}

		/// <summary>
		/// If the GraphQL spec is a file, return null, we'll import it as a 'apis/schemas' resource type.
		/// </summary>
		public string GetGraphQlFormat()
		{
			return _graphQlFilePath.IsUri(out _) ? GraphQlFormat.GraphQlLink : null;
		}

		/// <summary>
		/// If the GraphQL spec is a file, return null, we'll import it as a 'apis/schemas' resource type.
		/// </summary>
		public string GetValue()
		{
			return _graphQlFilePath.IsUri(out _) ? _graphQlFilePath : null;
		}

		public async Task<string> GetFileContent()
		{
			return await _fileReader.RetrieveFileContentsAsync(_graphQlFilePath);
		}
	}
}
