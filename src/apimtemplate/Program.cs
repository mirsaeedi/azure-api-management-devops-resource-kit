using System;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using System.Collections.Generic;
using CommandLine;
using Apim.DevOps.Toolkit.CommandLine;
using System.Threading.Tasks;
using Apim.DevOps.Toolkit;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates
{
    class Program
    {
		public static Task Main(string[] args)
		{
			var result = Parser.Default.ParseArguments<CommandLineOption>(args);

			result.MapResult(
				async option => await ProcessCommand(option),
				async errors => await ProcessError(errors));

			return Task.CompletedTask;
		}

		private static Task ProcessError(IEnumerable<Error> errors)
		{
			foreach (var error in errors)
			{
				Console.WriteLine(error);
			}

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
				Console.WriteLine(e.ToString());
			}

			return Task.CompletedTask;
			
		}
    }
}