using System;
using Illusive.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Illusive {
    public class Program {
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog(ConfigureSerilog, true)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
        }

        private static void ConfigureSerilog(HostBuilderContext context, LoggerConfiguration config) {
            config.ReadFrom.Configuration(context.Configuration);
            var ddOptions = context.Configuration.GetSection("Serilog")?.GetSection("DataDog")
                ?.Get<DatadogOptions>();

            if ( ddOptions != null ) {
                config.WriteTo.DatadogLogs(
                    apiKey: ddOptions.ApiKey,
                    source: ".NET",
                    service: ddOptions.ServiceName ?? "Illusive",
                    host: ddOptions.HostName ?? Environment.MachineName,
                    tags: new[] {
                        $"env:{ddOptions.EnvironmentName ?? context.HostingEnvironment.EnvironmentName}",
                        $"assembly:{ddOptions.AssemblyName ?? context.HostingEnvironment.ApplicationName}"
                    },
                    configuration: ddOptions.ToDatadogConfiguration(),
                    logLevel: ddOptions.OverrideLogLevel ?? LogEventLevel.Verbose
                );
            }
        }
    }
}
