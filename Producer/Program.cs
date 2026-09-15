using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Producer.Services;

namespace Producer;

public class Program
{
    public static async Task Main(string[] args)
    {
        HostApplicationBuilder builder =
            Host.CreateApplicationBuilder(args);

        builder.Services.AddHttpClient();

        builder.Services.AddHostedService<StationStatusService>();
        builder.Services.AddHostedService<StationInformationService>();

        using IHost host = builder.Build();

        await host.RunAsync();
    }
}
