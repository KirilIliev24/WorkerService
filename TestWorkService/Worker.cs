using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using TestWorkService.DataBase;

namespace TestWorkService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private CustomGoogleSearch googleSearch;
        private LinkPositionDataAccess linkPosition;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            googleSearch = new CustomGoogleSearch();
            linkPosition = new LinkPositionDataAccess();
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    string[] allKeyWords = linkPosition.getAllKeywords();
                    if (allKeyWords != null)
                    {
                        foreach (string s in allKeyWords)
                        {
                            await googleSearch.GetWebSearchResul(s);
                            _logger.LogInformation($"Worker searched for: {s}");
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"There are no keywords in the database");
                        continue;
                    }

                    TimeSpan time = new TimeSpan(24, 0, 0);
                    await Task.Delay(time, stoppingToken);
                }
                catch (Exception e)
                {
                    Log.Fatal(e, "The worker caught an exception.");
                    Log.Fatal(e.Message);
                    return;

                }
                finally
                {
                    Log.CloseAndFlush();
                }

            }
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            linkPosition.CloseConnection();
            _logger.LogInformation($"The worker is shutting down.");
            return base.StopAsync(cancellationToken);
        }


    }
}
