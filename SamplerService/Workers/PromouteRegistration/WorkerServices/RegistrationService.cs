using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using SamplerService.SystemHelpers;
using System.Net;
using System.Net.Http;

namespace SamplerService.Workers.PromouteRegistration.WorkerServices;
public interface IRegistrationService
{
    Task<ValueResult<ReservInfo>> TryGetAvalableRegistration(CancellationToken token);
    Task<ValueResult<ReservInfo>> TryGetReserveInfo(CancellationToken token);
    Task<Result> TryUpdateRegistration(ReservInfo reservInfo, CancellationToken token);
    Task<Result> TryInsertRegistration(ReservInfo reservInfo, CancellationToken token);
}
public class RegistrationService : IRegistrationService
{
    private readonly IRegistrationHttpClient _registrationHttpClient;
    private readonly IResposeCache _resposeCache;
    private readonly ILogger<RegistrationService> _logger;
    private readonly RegistrationServiceSettings _settings;

    public RegistrationService(IResposeCache resposeCache, IOptions<RegistrationServiceSettings> settings, IRegistrationHttpClient registrationHttpClient, ILogger<RegistrationService> logger)
    {
        _resposeCache = resposeCache;
        _settings = settings.Value;
        _logger = logger;
        _registrationHttpClient = registrationHttpClient;
    }
    public async Task<ValueResult<ReservInfo>> TryGetAvalableRegistration(CancellationToken token)
    {

        var date = await TryGetAvalableDate(token);
        if (!date.IsOk) return new();

        var time = await TryGetAvalableTimeId(date.Value, token);
        if (!time.IsOk) return new();

        return new(new(date.Value, time.Value));
    }

    private async Task<ValueResult<DateOnly>> TryGetAvalableDate(CancellationToken token)
    {
        var request = () => _registrationHttpClient.GetAvalableDate(token);
        var processor = () => request.InokeRequest(_logger);
        var response = await processor.RetryFor(_settings.TryCont, TimeSpan.FromSeconds(5));
        if (!response.IsOk) return new();

        using var contentStream = await response.Value.Content.ReadAsStreamAsync(token);
        using var reader = new StreamReader(contentStream);
        var content = await reader.ReadToEndAsync();

        var date = ToDate(content);
        if (date.HasValue) return new(date.Value);

        if (HasNoRegistration(content)) _logger.LogInformation(content);
        else _logger.LogError($"Can't parse response to avalableDate: {content}");
        return new();
    }

    private async Task<ValueResult<int>> TryGetAvalableTimeId(DateOnly avalableDate, CancellationToken token)
    {
        var avalableTimeId = _settings.UserTrevalDate;
        var request = () => _registrationHttpClient.GetAvalableTimeId(avalableDate, DateOnly.FromDateTime(avalableTimeId), token);
        var processor = () => request.InokeRequest(_logger);
        var response = await processor.RetryFor(_settings.TryCont, TimeSpan.FromSeconds(5));
        if (!response.IsOk) return new();

        using var contentStream = await response.Value.Content.ReadAsStreamAsync(token);
        var time = await TryGetTimeId(contentStream);
        return time;
    }

    private async Task<ValueResult<int>> TryGetTimeId(Stream contentStream)
    {
        try
        {
            var html = new HtmlDocument();
            html.Load(contentStream);
            string? value = html.DocumentNode
            .SelectNodes("content/node/id")
            .Select(e => e.GetDirectInnerText())
            .Where(e => e != "0").FirstOrDefault();
            if (value is null)
            {
                _logger.LogWarning("There are no avalable time range");
                return new();
            }
            if (int.TryParse(value, out int id)) return new(id);

            _logger.LogError($"Can't convert id \"{value}\" to int");
            return new();
        }
        catch (Exception e)
        {
            using var reader = new StreamReader(contentStream);
            _logger.LogError(e, $"Unexpected content in GetTime response: {await reader.ReadToEndAsync()}");
            return new();
        }

    }
    private static bool HasNoRegistration(string? rowResult) => rowResult == "На ближайшие 2 недели записи нет";
    private static DateOnly? ToDate(string? rowResult) => DateOnly.TryParseExact(rowResult?.Trim(), "dd.MM.yyyy", out var result) ? result : null;





    public async Task<ValueResult<ReservInfo>> TryGetReserveInfo(CancellationToken token)
    {
        if (_resposeCache.DateOfRegistration.HasValue) return new(_resposeCache.DateOfRegistration.Value);

        var request = () => _registrationHttpClient.GetReserveInfo(_settings.UserToken, token);
        var processor = () => request.InokeRequest(_logger);
        var message = await processor.RetryFor(_settings.TryCont, TimeSpan.FromSeconds(5));
        if (!message.IsOk) return new();

        return await GetReserveInfoFromMessage(message.Value, token);
    }

    private static async Task<ValueResult<ReservInfo>> GetReserveInfoFromMessage(HttpResponseMessage message, CancellationToken token)
    {

        var html = new HtmlDocument();
        html.Load(await message.Content.ReadAsStreamAsync(token));

        var elemnt = html.GetElementbyId("appdate");
        var value = elemnt.GetAttributeValue("value", null);
        return !DateOnly.TryParseExact(value, "dd.MM.yyyy", out var date) ? (new()) : (new(new(date, -1/*TODO Get time*/)));
    }



    public async Task<Result> TryInsertRegistration(ReservInfo reservInfo, CancellationToken token)
    {
        var request = () => _registrationHttpClient.InsertRegistration(_settings.UserToken, _settings.UserPhone, reservInfo.Date, reservInfo.TimeId, token);
        var processor = () => request.InokeRequest(_logger);
        var message = await processor.RetryFor(_settings.TryCont, TimeSpan.FromSeconds(5));

        _resposeCache.DateOfRegistration = message.IsOk ? reservInfo : null; //if respose is not ok we can't be sure avalableDate updated or not, so we must to drop cache
        return new(true);
    }
    public async Task<Result> TryUpdateRegistration(ReservInfo reservInfo, CancellationToken token)
    {
        var request = () => _registrationHttpClient.UpdateRegistration(_settings.UserToken, reservInfo.Date, reservInfo.TimeId, token);
        var processor = () => request.InokeRequest(_logger);
        var message = await processor.RetryFor(_settings.TryCont, TimeSpan.FromSeconds(5));

        _resposeCache.DateOfRegistration = message.IsOk ? reservInfo : null; //if respose is not ok we can't be sure avalableDate updated or not, so we must to drop cache
        return new(true);
    }
}
public readonly record struct SampleResult(string? RowResult, DateOnly? DateResult, bool? HasNoRegistration, HttpStatusCode StatusCode);


public readonly record struct ReservInfo(DateOnly Date, int TimeId);
