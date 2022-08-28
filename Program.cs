using SamplerService;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddHostedService<Worker>();
services.AddHttpClient(HttpClientNames.VisaTimetable, httpClient =>
{
    httpClient.BaseAddress = new Uri("https://italy-vms.ru/");
});

services.Configure<BotConfiguration>(builder.Configuration.GetSection(BotConfiguration.Configuration));
services.AddScoped<LoggingHandler>();

services.AddHttpClient("telegram_bot_client")
.AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
{
    BotConfiguration? botConfig = sp.GetConfiguration<BotConfiguration>();
    TelegramBotClientOptions options = new(botConfig.BotToken);
    return new TelegramBotClient(options, httpClient);
}).AddHttpMessageHandler<LoggingHandler>();

services.AddScoped<ISampler, Sampler>();
services.AddScoped<IBotService, BotService>();
services.AddScoped<IBusinessWorker, BusinessWorker>();

var app = builder.Build();

app.UseHttpLogging();

await app.RunAsync();