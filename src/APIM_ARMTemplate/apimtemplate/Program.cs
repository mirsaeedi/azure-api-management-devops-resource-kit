using System;
using Colors.Net;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Extract;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates
{
    class Program
    {
        public static void Main(string[] args)
        {
            var app = ConfigureApplication();
            app.Execute(args);
        }

        private static CommandLineApplication ConfigureApplication()
        {
            var app = new CommandLineApplication()
            {
                Name = GlobalConstants.AppShortName,
                FullName = GlobalConstants.AppLongName,
                Description = GlobalConstants.AppDescription
            };

            app.HelpOption(inherited: true);
            app.Commands.Add(new CreateCommand());
            app.Commands.Add(new ExtractCommand());

            app.OnExecute(() =>
            {
                ColoredConsole.Error.WriteLine("No commands specified, please specify a command");
                app.ShowHelp();
                return 1;
            });
            return app;
        }
    }
}