using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create;
using Apim.DevOps.Toolkit.CommandLine;
using System.Threading.Tasks;
using CommandLine;
using System.Collections.Generic;
using System;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates
{
	class Program
	{
		private static int errorCode = 0;
		public static Task<int> Main(string[] args)
		{
			var result = Parser.Default.ParseArguments<CommandLineOption>(args);

			result.MapResult(
				async option => await ProcessCommand(option),
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

		private static Task ProcessCommand(CommandLineOption option)
		{
			try
			{
				var createCommand = new CreateCommand();
				createCommand.Process(option).Wait();
			}
			catch (Exception e)
			{
				errorCode = -1;
				Console.WriteLine(e.ToString());
			}

			return Task.CompletedTask;
			
		}
	}
}