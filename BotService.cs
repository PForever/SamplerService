using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

interface IBotService{
    Task SendMessage(string message, CancellationToken token);
}

class BotService : IBotService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<BotService> _logger;
    public BotService(ILogger<BotService> logger, ITelegramBotClient botClient){
        _logger = logger;
        _botClient = botClient;
    }
    public async Task SendMessage(string message, CancellationToken token){
        var me = await _botClient.GetMeAsync(token);
        _logger.LogInformation("Start sending message", me.Username ?? "My Awesome Bot");

        await _botClient.SendTextMessageAsync("-1001726786731", message, cancellationToken: token);
    }
}