using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using SamplerService.SystemHelpers;
using System.Net;

namespace SamplerService;
interface IRegistrationService{
    Task<ValueResult<ReservInfo>> TryGetAvalableRegistration(CancellationToken token);
    Task<ValueResult<ReservInfo>> TryGetReserveInfo(CancellationToken token);
    Task<Result> TryUpdateRegistration(ReservInfo reservInfo, CancellationToken token);
}
class RegistrationService : IRegistrationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IResposeCache _resposeCache;
    private readonly ILogger<RegistrationService> _logger;
    private readonly RegistrationServiceSettings _settings;

    public RegistrationService(IHttpClientFactory httpClientFactory, IResposeCache resposeCache, IOptions<RegistrationServiceSettings> settings, ILogger<RegistrationService> logger){
        _httpClientFactory = httpClientFactory;
        _resposeCache = resposeCache;
        _settings = settings.Value;
        _logger = logger;
    }
    public async Task<ValueResult<ReservInfo>> TryGetAvalableRegistration(CancellationToken token){
        using var httpClient = _httpClientFactory.CreateClient(HttpClientNames.VisaTimetable);

        var date = await TryGetAvalableDate(httpClient, token);
        if (!date.IsOk) return new();

        var time = await TryGetAvalableTimeId(httpClient, date.Value, token);
        if (!time.IsOk) return new();

        return new(new(date.Value, time.Value));
    }

    private async Task<ValueResult<DateOnly>> TryGetAvalableDate(HttpClient httpClient, CancellationToken token)
    {
        var request = () => httpClient.GetAsync("vcs/get_nearest.htm?center=11&persons=&urgent=0&lang=ru", token);
        var processor = () => request.InokeRequest(_logger);
        var response = await processor.RetryFor(_settings.TryCont, TimeSpan.FromSeconds(5));
        if (!response.IsOk) return new();

        using var contentStream = await response.Value.Content.ReadAsStreamAsync(token);
        using var reader = new StreamReader(contentStream);
        var content = await reader.ReadToEndAsync();

        var date = ToDate(content);
        if (date.HasValue) return new(date.Value);

        if (HasNoRegistration(content)) _logger.LogInformation(content);
        else _logger.LogError($"Can't parse response to date: {content}");
        return new();
    }

    private async Task<ValueResult<int>> TryGetAvalableTimeId(HttpClient httpClient, DateOnly date, CancellationToken token)
    {
        var request = () => httpClient.GetAsync($"vcs/get_times.htm?vtype=6&center=11&persons=1&appdate={date:dd.MM.yyyy}&fdate={/*TODO Add GetTravelDate*/_settings.UserTrevalDate:dd.MM.yyyy}&lang=ru", token);
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
            if(int.TryParse(value, out int id)) return new(id);

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

        var httpClient = _httpClientFactory.CreateClient(HttpClientNames.VisaTimetable);
        var url = $"http://italy-vms.ru/autoform/?t={_settings.UserToken}&lang=ru";
        var content = new StringContent($"action=reschedule&appdata=");

        var processor = () => TryGetReserveInfoInternal(httpClient, url, content, token);
        return await processor.RetryFor(_settings.TryCont, TimeSpan.FromSeconds(5));
    }

    private static async Task<ValueResult<ReservInfo>> TryGetReserveInfoInternal(HttpClient httpClient, string url, StringContent content, CancellationToken token)
    {
        var response = await httpClient.PostAsync(url, content, token);

        var html = new HtmlDocument();
        html.Load(response.Content.ReadAsStream());

        var elemnt = html.GetElementbyId("appdate");
        var value = elemnt.GetAttributeValue<DateTime?>("value", null);

        return value.HasValue ? new(new(DateOnly.FromDateTime(value.Value), -1/*TODO Get time*/)) : new();
    }



    public async Task<Result> TryUpdateRegistration(ReservInfo reservInfo, CancellationToken token)
    {
        var httpClient = _httpClientFactory.CreateClient(HttpClientNames.VisaTimetable);
        var url = $"http://italy-vms.ru/autoform/?t={_settings.UserToken}&lang=ru";
        var content = new StringContent($"action=reschedule&appdata=&appdate={reservInfo.Date:dd.MM.yyyy}&apptime={reservInfo.TimeId}");

        var processor = () => TryUpdateRegistrationInternal(httpClient, url, content, token);
        var result = await processor.RetryFor(_settings.TryCont, TimeSpan.FromSeconds(5));

        _resposeCache.DateOfRegistration = result.IsOk? reservInfo : null; //if respose is not ok we can't be sure date updated or not, so we must to drop cache
        return result;
    }

    private async Task<Result> TryUpdateRegistrationInternal(HttpClient httpClient, string url, StringContent content, CancellationToken token)
    {
        try
        {
            var response = await httpClient.PostAsync(url, content, token);
            if (response.IsSuccessStatusCode) return new(true);
            _logger.LogError($"[{response.StatusCode.ToString()}]: {response.Content.ReadAsStringAsync()}");
            return new(false);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Http request failed");
            return new(false);
        }
    }
}
public readonly record struct SampleResult(string? RowResult, DateOnly? DateResult, bool? HasNoRegistration, HttpStatusCode StatusCode);


public readonly record struct ReservInfo(DateOnly Date, int TimeId);
