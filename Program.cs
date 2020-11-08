using System;
using Illusive.Illusive.Logging.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Datadog.Logs;

namespace Illusive {
    public class Program {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
            // try {
            //     CreateHostBuilder(args).Build().Run();
            // } catch ( Exception ex ) {
            //     Log.Fatal(ex, "Application start-up failed");
            // }
            // finally {
            //     Log.CloseAndFlush();
            // }
        }

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
                    ddOptions.ApiKey,
                    source: ".NET",
                    service: ddOptions.ServiceName ?? "Illusive",
                    host: ddOptions.HostName ?? Environment.MachineName,
                    new string[] {
                        $"env:{(ddOptions.EnvironmentName ?? context.HostingEnvironment.EnvironmentName)}",
                        $"assembly:{(ddOptions.AssemblyName ?? context.HostingEnvironment.ApplicationName)}"
                    },
                    ddOptions.ToDatadogConfiguration(),

                    // no need for debug logs in datadag
                    logLevel: ddOptions.OverrideLogLevel ?? LogEventLevel.Verbose
                );
            }
        }
    }
}
