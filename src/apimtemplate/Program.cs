using Apim.DevOps.Toolkit.CommandLine;
using System.Threading.Tasks;
using CommandLine;
using System.Collections.Generic;
using System;
using Apim.DevOps.Toolkit.CommandLine.Commands;
using Apim.DevOps.Toolkit.Core.Mapping;
using AutoMapper;

namespace Apim.DevOps.Toolkit
{
	public static class Program
	{
		private static int errorCode;
		public static Task<int> Main(string[] args)
		{
			var mapper = MappingConfiguration.Map();

			var result = Parser.Default.ParseArguments<CommandLineOption>(args);

			result.MapResult(
				async option => await ProcessCommand(option, mapper),
				async errors => await ProcessError(errors));

			return Task.FromResult(errorCode);
		}

		private static Task ProcessError(IEnumerable<Error> errors)
		{
			foreach (var error in errors)
			{
				Console.WriteLine(error);
			}

			errorCode = -1;

			return Task.CompletedTask;
		}

		private static Task ProcessCommand(CommandLineOption option, IMapper mapper)
		{
			try
			{
				var createCommand = new CreateCommand(mapper);
				createCommand.ProcessAsync(option).Wait();
			}
			catch (Exception e)
			{
				errorCode = -1;
				Console.Error.WriteLine(e.ToString());
			}

			return Task.CompletedTask;
		}
	}
}