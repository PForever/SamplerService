using Microsoft.Extensions.Options;

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

interface IBusinessWorker{
    Task DoWorkAsync(CancellationToken token);
    public TimeSpan Delay { get; }
}
class BusinessWorker : IBusinessWorker{

    private readonly IRegistrationService registrationService;
    private readonly IBotService _botService;
    private readonly IMessageCache _messageCache;
    private readonly RegistrationServiceSettings _settings;
    private readonly ILogger<BusinessWorker> _logger;
    public TimeSpan Delay => TimeSpan.FromSeconds(_settings.JobDelaySeconds);
    public BusinessWorker(IRegistrationService sampler, IBotService botService, IMessageCache messageCache, IOptions<RegistrationServiceSettings> settings, ILogger<BusinessWorker> logger){
        registrationService = sampler;
        _botService = botService;
        _messageCache = messageCache;
        _settings = settings.Value;
        _logger = logger;
    }
    public async Task DoWorkAsync(CancellationToken token){
        
        //1. Sample last avalable registration Date
        _logger.LogInformation("Sampling..");
        var avalableReservInfo = await registrationService.TryGetAvalableRegistration(token);

        //2. Send message to telegram bot
        await SentAvalableReservInfo(avalableReservInfo, token);

        if (!avalableReservInfo.IsOk)
        {
            _logger.LogInformation($"Reservation anavalable");
            return;
        }

        //3. Get Date for current registration of user
        _logger.LogInformation("Get reservation info..");
        var currentReservInfo = await registrationService.TryGetReserveInfo(token);
        if (!currentReservInfo.IsOk)
        {
            _logger.LogError($"Finally we couldn't take reservation info");
            return;
        }

        //4 Skip, if don't need to change date
        if (!NeedToChangeReserve(currentReservInfo.Value, avalableReservInfo.Value))
        {
            _logger.LogInformation($"Don't need to reserv: {currentReservInfo.Value} <= {avalableReservInfo.Value}");
            return;
        }

        //5. Try to change registred Date
        var result = await registrationService.TryUpdateRegistration(currentReservInfo.Value, token);
        if (!result.IsOk) _logger.LogError($"Finally we couldn't update registration date");

        //6. Send message to telegram

        await _botService.SendMessage($"Дата резервации изменена с {currentReservInfo.Value.Date} на {avalableReservInfo.Value.Date})", token);
    }

    private bool NeedToChangeReserve(ReservInfo current, ReservInfo avalable)
    {
        if(avalable.Date == DateOnly.FromDateTime(DateTime.Now))
        {
            _logger.LogWarning("Registeration in current day will be skiped");
            return false;
        }
        return current.Date > avalable.Date;
    }

    private async Task SentAvalableReservInfo(ValueResult<ReservInfo> avalableReservInfo, CancellationToken token)
    {
        var message = $"{(avalableReservInfo.IsOk ? $"Доступна регистрация на {avalableReservInfo.Value.Date}" : "Регистрация недоступна")}";
        if (_messageCache.LastMessage == message) return;
        await _botService.SendMessage(message, token);
        _messageCache.LastMessage = message;
    }
}