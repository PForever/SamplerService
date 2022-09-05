using Moq;
using SamplerService.SystemHelpers;
using SamplerService.Tests.CommonBuilders;
using SamplerService.Workers.PromouteRegistration.WorkerServices;
using System.Net;

namespace SamplerService.Tests.RegistrationService_Tests.TestBuilders.ServiceBuilders;
internal partial class RegistrationService_TestBuilder
{

    HttpClientBuilder => RegistrationHttpClientBuilder
    Post, Get => GetAvalableDate, GetAvalableTimeId, GetReserveInfo, UpdateRegistration, InsertRegistration

    internal class HttpClientBuilder : BuilderBase<HttpClient>
    {
        public HttpClientBuilder()
        {
            Post = new(Setup);
            Get = new(Setup);
        }

        protected override void DefaultSetup(Mock<HttpClient> mock)
        {
            base.DefaultSetup(mock);

            Post.Setup();
            Get.Setup();
        }

        public PostSetuper Post { get; }
        public class PostSetuper
        {
            public static HttpResponseMessage DefaultOutput { get; } = new(HttpStatusCode.Created);

            private readonly Action<Action<Mock<HttpClient>>> _settuper;

            public PostSetuper(Action<Action<Mock<HttpClient>>> settuper) => _settuper = settuper;

            private readonly List<(string? Url, HttpContent? Content)> _inputs = new();
            public async Task<IReadOnlyCollection<(string? Url, string? Content)>> GetStringInputs()
            {
                var list = new List<(string?, string?)>();
                foreach (var (url, content) in _inputs)
                {
                    var str = content is null ? null : await content.ReadAsStringAsync();
                    list.Add((url, str));
                }
                return list;
            }
            public PostSetuper Setup(string outputFilePath)
            {
                using var outputFile = File.OpenRead(outputFilePath);
                using var reader = new StreamReader(outputFile);
                var content = new StringContent(reader.ReadToEnd());
                var message = new HttpResponseMessage(HttpStatusCode.Created) { Content = content };
                return Setup(message);
            }
            public PostSetuper Setup(HttpResponseMessage? output = default)
            {
                var setup = (Mock<HttpClient> mock) =>
                {
                    var outputValue = output != null ? output : DefaultOutput;
                    Action<string?, HttpContent?, CancellationToken> invokeCallback = (url, contenct, _) => _inputs.Add((url, contenct));
                    mock.Setup(s => s.PostAsync(It.IsAny<string?>(), It.IsAny<HttpContent?>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(outputValue)
                        .Callback(invokeCallback);
                };
                _settuper(setup);
                return this;
            }
        }


        public GetSetuper Get { get; }
        public class GetSetuper
        {
            public static HttpResponseMessage DefaultOutput { get; } = new(HttpStatusCode.Created);

            private readonly Action<Action<Mock<HttpClient>>> _settuper;

            public GetSetuper(Action<Action<Mock<HttpClient>>> settuper) => _settuper = settuper;

            private readonly List<(string? Url, HttpContent? Content)> _inputs = new();
            public async Task<IReadOnlyCollection<(string? Url, string? Content)>> GetStringInputs()
            {
                var list = new List<(string?, string?)>();
                foreach (var (url, content) in _inputs)
                {
                    var str = content is null ? null : await content.ReadAsStringAsync();
                    list.Add((url, str));
                }
                return list;
            }
            public GetSetuper Setup(string outputFilePath)
            {
                using var outputFile = File.OpenRead(outputFilePath);
                using var reader = new StreamReader(outputFile);
                var content = new StringContent(reader.ReadToEnd());
                var message = new HttpResponseMessage(HttpStatusCode.Created) { Content = content };
                return Setup(message);
            }
            public GetSetuper Setup(HttpResponseMessage? output = default)
            {
                var setup = (Mock<HttpClient> mock) =>
                {
                    var outputValue = output != null ? output : DefaultOutput;
                    Action<string?, HttpContent?, CancellationToken> invokeCallback = (url, contenct, _) => _inputs.Add((url, contenct));
                    mock.Setup(s => s.GetAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(outputValue)
                        .Callback(invokeCallback);
                };
                _settuper(setup);
                return this;
            }
        }
    }
}
