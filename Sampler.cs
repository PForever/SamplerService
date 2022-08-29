namespace SamplerService;

using System.Net;

interface ISampler{
    Task<SampleResult> SampleAsync(CancellationToken token);
}
class Sampler : ISampler
{
    private readonly IHttpClientFactory _httpClientFactory;
    public Sampler(IHttpClientFactory httpClientFactory){
        _httpClientFactory = httpClientFactory;
    }
    public async Task<SampleResult> SampleAsync(CancellationToken token){
        var httpClient = _httpClientFactory.CreateClient(HttpClientNames.VisaTimetable);
        var httpResponseMessage = await httpClient.GetAsync(
            "vcs/get_nearest.htm?center=11&persons=&urgent=0&lang=ru", token);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync(token);
            using var reader = new StreamReader(contentStream);
            var content = await reader.ReadToEndAsync();
            
            return new(content, HasNoRegistration(content), httpResponseMessage.StatusCode);
        }
        return new(null, null, httpResponseMessage.StatusCode);
    }
    private static bool HasNoRegistration(string rowResult) => rowResult == "На ближайшие 2 недели записи нет";
}
public readonly record struct SampleResult(string? RowResult, bool? HasNoRegistration, HttpStatusCode StatusCode);