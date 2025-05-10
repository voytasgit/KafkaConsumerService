using Microsoft.Extensions.Configuration;
using Serilog;

namespace KafkaConsumerService.Utils
{
    // Static class to load configuration and initialize logging.
    public static class ConfigurationLoader
    {
        // Method to load configuration from various sources like JSON files and environment variables.
        public static IConfiguration Load(string[] args = null)
        {
            // Create a configuration builder
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Set the base path to the current directory
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Load appsettings.json (required)
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true) // Load machine-specific configuration (optional)
                .AddEnvironmentVariables(); // Load environment variables for configuration overrides

            // If command-line arguments are provided, add them to the configuration.
            if (args != null)
            {
                builder.AddCommandLine(args);
            }

            // Build and return the configuration.
            return builder.Build();
        }

        // Method to initialize logging based on the configuration provided.
        public static void InitializeLogging(IConfiguration configuration)
        {
            // Configure Serilog logging based on the provided configuration.
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration) // Read logging settings from configuration
                .WriteTo.Console() // Write logs to the console
                .WriteTo.File("logs/log_.txt", rollingInterval: RollingInterval.Day) // Write logs to a file, rolling every day
                .CreateLogger(); // Create and initialize the logger
        }
    }
}
