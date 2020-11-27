using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestWorkService.DataBase;
using Serilog;
using Serilog.Events;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace TestWorkService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(@"C:\Users\Administrator\Kiril\WorkerService\LogFile.txt")
                .CreateLogger();
            try
            {
                Log.Information("Worker service is starting...");
                CreateHostBuilder(args).Build().Run();
                return;
            }
            catch (Exception e)
            {
                Log.Fatal(e, "The worker service could NOT start!");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder
                   .AddJsonFile("appsettings.json")
                   .AddEnvironmentVariables()
                   .Build();
            })
            .ConfigureServices((hostContext, services) =>
            { 
                var optionsBuilder = new DbContextOptionsBuilder<SearchEngineContext>();
                //optionsBuilder.UseSqlServer(hostContext.Configuration.GetConnectionString("Conn2"));
                //optionsBuilder.UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = SearchEngine; Integrated Security = false; User ID = Koko; Password = koko2403;");
                services.AddScoped<SearchEngineContext>(s => new SearchEngineContext(optionsBuilder.Options));

                services.AddHostedService<Worker>();
                })
            .UseSerilog();
    }
}
