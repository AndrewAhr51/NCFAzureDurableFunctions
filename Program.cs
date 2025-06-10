using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCFAzureDurableFunctions.Src.Services.Helpers;
using Microsoft.DurableTask.SqlServer.AzureFunctions;
using Microsoft.DurableTask.SqlServer;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(worker =>
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
        services.AddSingleton<JwtHelper>(provider =>
        {
            return new JwtHelper(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                configuration["Jwt:Key"]
            );
        });

        services.AddSingleton<EncryptionHelper>(provider =>
        {
            return new EncryptionHelper(configuration["EncryptionKey"]);
        });

        // ✅ Register Durable Functions with MSSQL storage provider
        services.AddDurableTaskSqlServer(options =>
        {
            options.ConnectionString = configuration.GetConnectionString("DefaultConnection");
        });

        // ✅ Ensure logging services are registered
        services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Information);
        });

    })
    .Build();

host.Run();