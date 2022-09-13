using Microsoft.Extensions.Options;
using SamplerService.CommonServices;
using SamplerService.CommonServices.Proxy;
using SamplerService.SystemHelpers;
using SamplerService.Workers.PromouteRegistration.WorkerServices;

namespace SamplerService.Workers.PromouteRegistration;

public class PromouteRegistrationWorker : IBusinessWorker
{

    private readonly IRegistrationService _registrationService;
    private readonly IBotService _botService;
    private readonly IMessageCache _messageCache;
    private readonly IVpnService _proxy;
    private readonly PromouteRegistrationWorkerSettings _settings;
    private readonly ILogger<PromouteRegistrationWorker> _logger;
    public TimeSpan Delay => TimeSpan.FromSeconds(_settings.JobDelaySeconds);
    public PromouteRegistrationWorker(IRegistrationService registrationService, IBotService botService, IMessageCache messageCache, IOptions<PromouteRegistrationWorkerSettings> settings,
    IVpnService proxy, ILogger<PromouteRegistrationWorker> logger)
    {
        _registrationService = registrationService;
        _botService = botService;
        _messageCache = messageCache;
        _proxy = proxy;
        _settings = settings.Value;
        _logger = logger;
    }
    public async Task DoWorkAsync(CancellationToken token)
    {
        //0. Change IP address
        await _proxy.ChangeIp();

        //1. Sample last avalable registration Date
        _logger.LogInformation("Sampling..");
        var avalableReservInfo = await _registrationService.TryGetAvalableRegistration(token);

        //2. Send message to telegram bot
        await SentAvalableReservInfo(avalableReservInfo, token);

        if (!avalableReservInfo.IsOk)
        {
            _logger.LogInformation($"Reservation anavalable");
            return;
        }

        if (!_settings.DoRegisration)
            return;

        if (!_settings.IsRegistred) //3.1 Then Try to get registration
        {
            //2. Send message to telegram
            var insertResult = await _registrationService.TryInsertRegistration(avalableReservInfo.Value, token);
            if (!insertResult.IsOk) _logger.LogError($"Finally we couldn't update registration date");

            //2. Send message to telegram
            await _botService.SendMessage($"Дата зарезервирована на {avalableReservInfo.Value.Date}). Закончите регестрацию!", token);
        }
        else //3.2 Then Try to update registration
        {
            //1. Get Date for current registration of user
            _logger.LogInformation("Get reservation info..");
            var currentReservInfo = await _registrationService.TryGetReserveInfo(token);

            if (!currentReservInfo.IsOk) //4.1 Then Try insert
            {
                _logger.LogError($"Finally we couldn't take reservation info");
                return;
            }

            //2. Skip, if don't need to change date
            if (!NeedToChangeReserve(currentReservInfo.Value, avalableReservInfo.Value))
            {
                _logger.LogInformation($"Don't need to reserv: {currentReservInfo.Value} <= {avalableReservInfo.Value}");
                return;
            }

            //3. Try to change registred Date
            var result = await _registrationService.TryUpdateRegistration(avalableReservInfo.Value, token);
            if (!result.IsOk) _logger.LogError($"Finally we couldn't update registration date");

            //4. Send message to telegram
            await _botService.SendMessage($"Дата резервации изменена с {currentReservInfo.Value.Date} на {avalableReservInfo.Value.Date})", token);
        }


    }

    private bool NeedToChangeReserve(ReservInfo current, ReservInfo avalable)
    {
        if (avalable.Date == DateOnly.FromDateTime(DateTime.Now))
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
