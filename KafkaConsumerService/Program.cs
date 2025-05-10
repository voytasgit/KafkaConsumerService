using KafkaConsumerService.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

public class Program
{
    // Main entry point of the application
    public static async Task Main(string[] args)
    {
        // Load configuration (appsettings.json, environment variables, etc.)
        var configuration = ConfigurationLoader.Load(args);

        // Initialize global Serilog configuration for logging
        ConfigurationLoader.InitializeLogging(configuration);

        // Capture and log unhandled exceptions globally
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Log.Fatal(e.ExceptionObject as Exception, "UNHANDLED EXCEPTION");
        };

        // Capture and log unobserved task exceptions
        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            Log.Error(e.Exception, "UNOBSERVED TASK EXCEPTION");
            e.SetObserved(); // Mark the exception as observed to prevent termination of the process
        };

        try
        {
            // Log the start of the host
            Log.Information("S T A R T host");

            // Create and build the host
            var host = CreateHostBuilder(args, configuration).Build();

            // Run the host asynchronously
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            // Log fatal error if the host terminates unexpectedly
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            // Ensure to flush and close the log at the end
            Log.CloseAndFlush();
        }
    }

    // Create the IHostBuilder, which configures the application
    public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog() // IMPORTANT: Integrate Serilog for logging
            .ConfigureServices((hostContext, services) =>
            {
                // Register application services
                ServiceRegistrator.RegisterServices(services, configuration);
            });
}
