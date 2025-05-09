using Microsoft.Extensions.Configuration;
using Serilog;

namespace KafkaConsumerService.Utils
{
    public static class ConfigurationLoader
    {
        public static IConfiguration Load(string[] args = null)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
                .AddEnvironmentVariables();

            if (args != null)
            {
                builder.AddCommandLine(args);
            }

            return builder.Build();
        }
        public static void InitializeLogging(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Console()
                .WriteTo.File("logs/log_.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}