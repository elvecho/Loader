using System;
using System.Collections.Generic;
using System.Net.Http;
using Loader.Configuration;
using Loader.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Serilog;

namespace Loader
{
    public class Program
    {
        private static IConfigurationRoot Configuration { get; set; }

        public static void Main(string[] args)
        {
            Startup();

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            } catch(Exception e)
            {
                Log.Fatal("Start-up failed {}", e);
            } finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void Startup()
        {
            Configuration = new ConfigurationBuilder()
                                                       .SetBasePath(@"C:\Retex\loader\config")//modifica il percorso per provare
                                                        .AddJsonFile("appsettings.json").Build();

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<QuartzHostedService>();
                    services.Configure<JobsConfig>(Configuration.GetSection("Jobs"));
                    services.Configure<Error>(Configuration.GetSection("Error"));
                    // Add Quartz services
                    services.AddSingleton<IJobFactory, SingletonJobFactory>();
                    services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
                    SetupHttpClient(services, "RtServerGeneric");

                    if (bool.Parse(Configuration.GetSection("Jobs:RtServerStatusEnable").Value))
                    {
                        services.AddSingleton<RtServerStatusJob>();
                        services.AddSingleton(new JobSchedule(typeof(RtServerStatusJob), Configuration.GetSection("Jobs:RtServerStatusCron").Value));
                    }

                    if (bool.Parse(Configuration.GetSection("Jobs:RtServerTransmissionsEnable").Value))
                    {
                        services.AddSingleton<RtServerTransmissionsJob>();
                        services.AddSingleton(new JobSchedule(typeof(RtServerTransmissionsJob), Configuration.GetSection("Jobs:RtServerTransmissionsCron").Value));
                    }

                    if (bool.Parse(Configuration.GetSection("Jobs:RtServerTransactionsEnable").Value))
                    {
                        services.AddSingleton<RtServerTransactionsJob>();
                        services.AddSingleton(new JobSchedule(typeof(RtServerTransactionsJob), Configuration.GetSection("Jobs:RtServerTransactionsCron").Value));
                    }
                }).ConfigureLogging((hostContext, logging) =>
                {
                    logging.AddSerilog();
                });

        private static void SetupHttpClient(IServiceCollection services, string name)
        {
            services.AddHttpClient(name, c =>
            {
                //c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AllowAutoRedirect = false,
                UseDefaultCredentials = true,
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });
        }
    }
}
