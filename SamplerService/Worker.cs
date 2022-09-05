using SamplerService.Workers;

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
        var input = Task.Run(Console.ReadKey);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var worker = await DoWorkAsync(stoppingToken);

                var delay = Task.Delay(worker.Delay, stoppingToken);

                await Task.WhenAny(input, delay);
                if(input.IsCompletedSuccessfully && input.Result.Key == ConsoleKey.Escape) {
                    await StopAsync(stoppingToken);
                    _applicationLifetime.StopApplication();
                    return;
            }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception e) {
                _logger.LogError(e, "Job failed");
            }
        }
    }

    private async Task<IBusinessWorker> DoWorkAsync(CancellationToken token){
        using var scope = _scopeFactory.CreateScope();
        var worker = scope.ServiceProvider.GetRequiredService<IBusinessWorker>();
        await worker.DoWorkAsync(token);
        return worker;
    }
}