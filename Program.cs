using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.Functions.Worker.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCFAzureDurableFunctions.Src.Services.Helpers;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(worker =>
    {
        // Register custom middleware
        worker.UseMiddleware<AuthenticationMiddleware>();
    })
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // ✅ Register helpers and services
        services.AddScoped<JwtHelper>(provider =>
        {
            return new JwtHelper(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                configuration["Jwt:Key"]
            );
        });

        services.AddScoped<EncryptionHelper>(provider =>
        {
            return new EncryptionHelper(configuration["EncryptionKey"]);
        });

        // ✅ Register Durable Functions using Azure Storage (instead of SQL Server)
        services.Configure<DurableTaskOptions>(options =>
        {
            // Replace 'AzureStorageDurabilityProvider' with a valid accessible type or configuration
            options.StorageProvider = new Dictionary<string, object>
            {
                { "TaskHubName", configuration["DurableTask:HubName"] }, // ✅ Ensure hub name is set
                { "ConnectionString", configuration.GetConnectionString("AzureStorageConnection") }
            };
        });

        // ✅ Ensure logging services are registered
        services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Information);
        });

    })
    .Build();

host.Run();