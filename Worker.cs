using System.Security.Cryptography.X509Certificates;

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
                var delay = Task.Delay(30_000, stoppingToken);

                await DoWorkAsync(stoppingToken);

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
            
        }
    }

    private async Task DoWorkAsync(CancellationToken token){
        using var scope = _scopeFactory.CreateScope();
        var worker = scope.ServiceProvider.GetRequiredService<IBusinessWorker>();
        await worker.DoWorkAsync(token);
    }
}

interface IBusinessWorker{
    Task DoWorkAsync(CancellationToken token);
}
class BusinessWorker : IBusinessWorker{

    private readonly ISampler _sampler;
    private readonly IBotService _botService;
    private readonly ILogger<BusinessWorker> _logger;
    public BusinessWorker(ISampler sampler, IBotService botService, ILogger<BusinessWorker> logger){
        _sampler = sampler;
        _botService = botService;
        _logger = logger;
    }
    public async Task DoWorkAsync(CancellationToken token){
        _logger.LogInformation("Sampling..");
        var (result, resposeChanged) = await _sampler.SampleAsync(token);
        _logger.LogInformation(result.StatusCode.ToString());
        _logger.LogInformation(result.RowResult);

        if (!resposeChanged) return;

        _logger.LogInformation("Sending..");
        await _botService.SendMessage($"Response: {result.RowResult} (Http code: {result.StatusCode}).{(resposeChanged ? "Go to https://italy-vms.ru/autoform/ NOW!" : "")}", token);
        _logger.LogInformation("Message send");
    }
}