
using WebsiteChecker.Business.Settings;
using WebsiteChecker.Worker;

IHost host = Host.CreateDefaultBuilder(args)
	.ConfigureServices(services =>
	{
	
        services.AddHostedService<WebsiteCheckerWorker>();
		services.AddSingleton<IWebsiteCheckerWorkerSettings, WebsiteCheckerWorkerSettings>();
 

    })
	.Build();

await host.RunAsync();