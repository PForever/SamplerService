using SamplerService;
using Telegram.Bot;

interface IBotService{
    Task SendMessage(string message, CancellationToken token);
}

class BotService : IBotService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IMessageCache _messageCache;
    private readonly ILogger<BotService> _logger;
    public BotService(ILogger<BotService> logger, ITelegramBotClient botClient){
        _logger = logger;
        _botClient = botClient;
    }
    public async Task SendMessage(string message, CancellationToken token){
        var me = await _botClient.GetMeAsync(token);
        _logger.LogInformation("Start sending message", me.Username ?? "My Awesome Bot");

        try
        {
            await _botClient.SendTextMessageAsync("-1001726786731", message, cancellationToken: token);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Can't send message to telegram");
        }
    }
}