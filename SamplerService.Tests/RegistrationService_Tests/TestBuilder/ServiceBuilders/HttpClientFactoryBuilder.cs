using SamplerService.Tests.CommonBuilders;

namespace SamplerService.Tests.RegistrationService_Tests.TestBuilders.ServiceBuilders;
internal partial class RegistrationService_TestBuilder
{
    internal class HttpClientFactoryBuilder : IBuilder<IHttpClientFactory>
    {
        private readonly HttpClientBuilder _httpClientBuilder = new();
        public HttpClientFactoryBuilder SetupHttpClient(Action<HttpClientBuilder> setup)
        {
            setup(_httpClientBuilder);
            return this;
        }
        public IHttpClientFactory Build() => new HttpClientFactoryMock(_httpClientBuilder.Build());
    }
    internal class HttpClientFactoryMock : IHttpClientFactory
    {
        private readonly HttpClient _httpClient;

        public HttpClientFactoryMock(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public HttpClient CreateClient(string name) => _httpClient;
    }
}
