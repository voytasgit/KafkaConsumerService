using KafkaConsumerService.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

public class Program
{
    public static async Task Main(string[] args)
    {
        var configuration = ConfigurationLoader.Load(args);
        // Serilog global konfigurieren
        ConfigurationLoader.InitializeLogging(configuration);

        // Unbehandelte Fehler erfassen
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Log.Fatal(e.ExceptionObject as Exception, "UNHANDLED EXCEPTION");
        };

        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            Log.Error(e.Exception, "UNOBSERVED TASK EXCEPTION");
            e.SetObserved();
        };

        try
        {
            Log.Information("S T A R T host");
            var host = CreateHostBuilder(args, configuration).Build();
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog() // WICHTIG: Serilog integrieren
            .ConfigureServices((hostContext, services) =>
            {
                ServiceRegistrator.RegisterServices(services, configuration);
            });
}
