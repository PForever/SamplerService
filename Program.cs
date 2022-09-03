using SamplerService;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddHostedService<Worker>();
services.AddHttpClient(HttpClientNames.VisaTimetable, httpClient =>
{
    httpClient.BaseAddress = new Uri("https://italy-vms.ru/");
});

services.AddConfiguration<BotConfiguration>(builder.Configuration);
services.AddConfiguration<BotConfiguration>(builder.Configuration);

services.AddScoped<LoggingHandler>();

services.AddHttpClient("telegram_bot_client")
.AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
{
    BotConfiguration? botConfig = sp.GetConfiguration<BotConfiguration>();
    TelegramBotClientOptions options = new(botConfig.BotToken);
    return new TelegramBotClient(options, httpClient);
}).AddHttpMessageHandler<LoggingHandler>();

services.AddScoped<IRegistrationService, RegistrationService>();
services.AddScoped<IBotService, BotService>();
services.AddScoped<IBusinessWorker, BusinessWorker>();
services.AddSingleton<IResposeCache, ResposeCache>();
services.AddSingleton<IMessageCache, MessageCache>();

var app = builder.Build();

app.UseHttpLogging();

await app.RunAsync();