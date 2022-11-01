using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using NLog.Web;
using SamplerService;
using SamplerService.CommonServices;
using SamplerService.CommonServices.Proxy;
using SamplerService.SystemHelpers;
using SamplerService.Workers;
using SamplerService.Workers.PromouteRegistration;
using SamplerService.Workers.PromouteRegistration.WorkerServices;
using System.Net;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonDateConverter());
});

services.AddLogging();

services.AddHostedService<Worker>();
services.AddHttpClient(RegistrationHttpClient.ClientName, httpClient =>
{
    httpClient.BaseAddress = new Uri(RegistrationHttpClient.BaseUrl);
}).ConfigurePrimaryHttpMessageHandler(builder =>
{
    var settings = builder.GetRequiredService<ITorSharpSettingsFactory>().Settings;
    return new HttpClientHandler
    {
        Proxy = new WebProxy(new Uri("http://localhost:" + settings.PrivoxySettings.Port))
    };
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
services.AddSingleton<ITorSharpSettingsFactory, TorSharpSettingsFactory>();
services.AddSingleton<ITorProxy, TorProxy>();
services.AddSingleton<IVpnService, VpnService>();

var app = builder.Build();

app.UseHttpLogging();

await app.RunAsync();