using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;


IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()      // <--- important to run as service
    .ConfigureServices(services =>
    {
        services.AddHostedService<NavJobsWorker>();
    })
    .Build();

await host.RunAsync();

