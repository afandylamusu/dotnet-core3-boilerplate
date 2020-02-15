#define HOSTING_OPTIONS

using App.Metrics.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Moonlay.MasterData.ApiGrpc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
#if REPORTING
                .ConfigureMetricsWithDefaults(builder =>
                {
                    builder.Report.ToConsole(TimeSpan.FromSeconds(1));
                })
#endif
#if HOSTING_OPTIONS
                .ConfigureAppMetricsHostingConfiguration(options =>
                {
                    // options.AllEndpointsPort = 3333;
                    options.EnvironmentInfoEndpoint = "/my-env";
                    //options.EnvironmentInfoEndpointPort = 1111;
                    options.MetricsEndpoint = "/my-metrics";
                    //options.MetricsEndpointPort = 2222;
                    options.MetricsTextEndpoint = "/my-metrics-text";
                    //options.MetricsTextEndpointPort = 3333;
                })
#endif
                .UseMetrics()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
