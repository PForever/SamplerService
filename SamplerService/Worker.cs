using SamplerService.SystemHelpers;
using SamplerService.Workers;
using System.Runtime.CompilerServices;

namespace SamplerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IServiceScopeFactory _scopeFactory;

    public Worker(ILogger<Worker> logger, IHostApplicationLifetime applicationLifetime, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _applicationLifetime = applicationLifetime;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = GetSettings();
        var jobDelay = TimeSpan.FromSeconds(settings.JobDelaySeconds);
        var input = Task.Run(() =>
        {
            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape) return;
            }
        });
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (input.IsCompletedSuccessfully)
                {
                    await StopAsync(stoppingToken);
                    _applicationLifetime.StopApplication();
                    return;
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await foreach (var result in DoWorkAsync(stoppingToken))
                {
                    if (result.IsOk) _logger.LogInformation("Job succeed");
                    else _logger.LogError("Job failed");
                }

                var delay = Task.Delay(jobDelay, stoppingToken);

                await Task.WhenAny(input, delay);

            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception e) {
                _logger.LogError(e, "Job failed");
                var delay = Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                await Task.WhenAny(input, delay);
            }
        }
    }

    private JobWorkerSettings GetSettings()
    {
        using var scope = _scopeFactory.CreateScope();
        return scope.ServiceProvider.GetConfiguration<JobWorkerSettings>();
    }
    private async IAsyncEnumerable<Result> DoWorkAsync([EnumeratorCancellation] CancellationToken token)
    {
        using var scope = _scopeFactory.CreateScope();
        var workers = scope.ServiceProvider.GetServices<IBusinessWorker>();
        foreach (var worker in workers)
        {
            bool result;
            try
            {
                await worker.DoWorkAsync(token);
                token.ThrowIfCancellationRequested();
                result = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Job failed");
                result = false;
            }
            yield return new(result);
        }
    }
}

sealed class JobWorkerSettings
{
    public int JobDelaySeconds { get; private set; }
}
