namespace SamplerService;

using System.Net;

interface ISampler{
    Task<(SampleResult Result, bool ResponseChanged)> SampleAsync(CancellationToken token);
}
class Sampler : ISampler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IResposeCache _resposeCache;

    public Sampler(IHttpClientFactory httpClientFactory, IResposeCache resposeCache){
        _httpClientFactory = httpClientFactory;
        _resposeCache = resposeCache;
    }
    public async Task<(SampleResult Result, bool ResponseChanged)> SampleAsync(CancellationToken token){
        var httpClient = _httpClientFactory.CreateClient(HttpClientNames.VisaTimetable);
        var httpResponseMessage = await httpClient.GetAsync(
            "vcs/get_nearest.htm?center=11&persons=&urgent=0&lang=ru", token);

        using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync(token);
        using var reader = new StreamReader(contentStream);
        var content = await reader.ReadToEndAsync();

        var response = new SampleResult(content, HasNoRegistration(content), httpResponseMessage.StatusCode);

        if(_resposeCache.CashedRespose == response)
            return (response, false);

        _resposeCache.CashedRespose = response;
        return (response, true);
    }
    private static bool HasNoRegistration(string rowResult) => rowResult == "На ближайшие 2 недели записи нет";
}
public readonly record struct SampleResult(string? RowResult, bool? HasNoRegistration, HttpStatusCode StatusCode);