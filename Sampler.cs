namespace SamplerService;

using System.Net;

interface ISampler{
    Task<(SampleResult Result, bool ValidResponseChanged)> SampleAsync(CancellationToken token);
}
class Sampler : ISampler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IResposeCache _resposeCache;
    private readonly ILogger<Sampler> _logger;

    public Sampler(IHttpClientFactory httpClientFactory, IResposeCache resposeCache, ILogger<Sampler> logger){
        _httpClientFactory = httpClientFactory;
        _resposeCache = resposeCache;
        _logger = logger;
    }
    public async Task<(SampleResult Result, bool ValidResponseChanged)> SampleAsync(CancellationToken token){
        var httpClient = _httpClientFactory.CreateClient(HttpClientNames.VisaTimetable);

        string? content = null;
        HttpStatusCode code = default;
        try
        {
            var httpResponseMessage = await httpClient.GetAsync("vcs/get_nearest.htm?center=11&persons=&urgent=0&lang=ru", token);
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync(token);
            using var reader = new StreamReader(contentStream);
            content = await reader.ReadToEndAsync();
            code = httpResponseMessage.StatusCode;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Getting result failed");
        }

        var response = new SampleResult(content, HasNoRegistration(content), code);

        if(response.StatusCode != HttpStatusCode.OK)
            return (response, false);

        if(_resposeCache.CashedRespose == response)
            return (response, false);

        _resposeCache.CashedRespose = response;
        return (response, true);
    }
    private static bool HasNoRegistration(string? rowResult) => rowResult == "На ближайшие 2 недели записи нет";
}
public readonly record struct SampleResult(string? RowResult, bool? HasNoRegistration, HttpStatusCode StatusCode);