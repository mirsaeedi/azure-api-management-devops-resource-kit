﻿using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using System;
using System.Collections.Generic;

namespace Apim.DevOps.Toolkit.Core.Templates
{
	public abstract class TemplateResource
	{
		private List<string> _dependencies = new List<string>();

		public TemplateResource(string name, string type, IEnumerable<string> dependencies)
		{
			Name = name;
			Type = type;
			_dependencies.AddRange(dependencies);
		}

		public string Name { get; set; }

		public string Type { get; set; }

		public string ApiVersion { get; set; } = GlobalConstants.ApiVersion;

		public string Scale { get; set; }

		public IReadOnlyList<string> DependsOn => _dependencies.AsReadOnly();

		public IReadOnlyList<TemplateResource> Resources { get; set; }

		internal void AddDependencies(IEnumerable<TemplateResource> dependencies)
		{
			foreach (var dependency in dependencies)
			{
				_dependencies.Add(dependency.ResourceId);
			}
		}

		internal void AddDependency(string api, string name)
		{
			throw new NotImplementedException();
		}

		public string ResourceId => $"[resourceId('{Type}', parameters('ApimServiceName'), '{Name}')]";
	}

	public class TemplateResource<TProperties> : TemplateResource
	{
		public TemplateResource(string name, string type, TProperties properties, IEnumerable<string> dependencies)
			: base(name, type, dependencies)
		{
			Properties = properties;
		}

		public TProperties Properties { get; set; }
	}
}