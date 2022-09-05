using SamplerService;
using SamplerService.CommonServices;
using SamplerService.SystemHelpers;
using SamplerService.Workers;
using SamplerService.Workers.PromouteRegistration;
using SamplerService.Workers.PromouteRegistration.WorkerServices;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddHostedService<Worker>();
services.AddHttpClient(HttpClientNames.VisaTimetable, httpClient =>
{
    httpClient.BaseAddress = new Uri("https://italy-vms.ru/");
});

services.AddConfiguration<BotServiceSettings>(builder.Configuration);
services.AddConfiguration<RegistrationServiceSettings>(builder.Configuration);
services.AddConfiguration<PromouteRegistrationWorkerSettings>(builder.Configuration);

services.AddScoped<LoggingHandler>();

services.AddHttpClient("telegram_bot_client")
.AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
{
    BotServiceSettings? botConfig = sp.GetConfiguration<BotServiceSettings>();
    TelegramBotClientOptions options = new(botConfig.BotToken);
    return new TelegramBotClient(options, httpClient);
}).AddHttpMessageHandler<LoggingHandler>();

services.AddScoped<IRegistrationHttpClient, RegistrationHttpClient>();
services.AddScoped<IRegistrationService, RegistrationService>();
services.AddScoped<IBotService, BotService>();
services.AddScoped<IBusinessWorker, PromouteRegistrationWorker>();
services.AddSingleton<IResposeCache, ResposeCache>();
services.AddSingleton<IMessageCache, MessageCache>();

var app = builder.Build();

app.UseHttpLogging();

await app.RunAsync();