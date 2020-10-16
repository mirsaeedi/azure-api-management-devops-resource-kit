using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
	public static class MappingConfiguration
	{
		public static AutoMapper.IMapper Map()
		{
			var config = new MapperConfiguration(cfg =>
			{
				cfg.AllowNullCollections = true;
				cfg.AllowNullDestinationValues = true;

				foreach (var mapper in DiscoverMappers())
				{
					mapper.Map(cfg);
				}
			});

			return config.CreateMapper();
		}

		private  static IEnumerable<IMapper> DiscoverMappers()
		{
			return Assembly
				.GetExecutingAssembly()
				.GetTypes()
				.Where(x => typeof(IMapper).IsAssignableFrom(x) && !x.IsInterface)
				.Select(x => (IMapper)Activator.CreateInstance(x));
		}
	}
}
