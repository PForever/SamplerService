using Moq;
using SamplerService.Tests.CommonBuilders;
using SamplerService.Workers.PromouteRegistration.WorkerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SamplerService.Tests.RegistrationService_Tests.TestBuilder.ServiceBuilders;
internal class RegistrationHttpClientBuilder : BuilderBase<IRegistrationHttpClient>
{
    public RegistrationHttpClientBuilder()
    {
        GetAvalableDate = new(Setup);
        GetAvalableTimeId = new(Setup);
        GetReserveInfo = new(Setup);
        InsertRegistration = new(Setup);
        UpdateRegistration = new(Setup);
    }

    protected override void DefaultSetup(Mock<IRegistrationHttpClient> mock)
    {
        base.DefaultSetup(mock);

        GetAvalableDate.Setup();
        GetAvalableTimeId.Setup();
        GetReserveInfo.Setup();
        InsertRegistration.Setup();
        UpdateRegistration.Setup();
    }

    public InsertRegistrationSetuper InsertRegistration { get; }
    public class InsertRegistrationSetuper
    {
        public static HttpResponseMessage DefaultOutput { get; } = new(HttpStatusCode.OK);

        private readonly Action<Action<Mock<IRegistrationHttpClient>>> _settuper;

        public InsertRegistrationSetuper(Action<Action<Mock<IRegistrationHttpClient>>> settuper) => _settuper = settuper;

        private readonly List<(string UserToken, string UserPhone, DateOnly AvalalbleDate, int TimeId)> _inputs = new();
        public IReadOnlyCollection<(string UserToken, string UserPhone, DateOnly AvalalbleDate, int TimeId)> Inputs => _inputs;
        public InsertRegistrationSetuper Setup(string outputFilePath)
        {
            using var outputFile = File.OpenRead(outputFilePath);
            using var reader = new StreamReader(outputFile);
            var content = new StringContent(reader.ReadToEnd());
            var message = new HttpResponseMessage(HttpStatusCode.Created) { Content = content };
            return Setup(message);
        }
        public InsertRegistrationSetuper Setup(HttpResponseMessage? output = default)
        {
            var setup = (Mock<IRegistrationHttpClient> mock) =>
            {
                var outputValue = output != null ? output : DefaultOutput;
                Action<string, string, DateOnly, int, CancellationToken> invokeCallback = (userToken, userPhone, avalableDate, timeId, _) => _inputs.Add((userToken, userPhone, avalableDate, timeId));
                mock.Setup(s => s.InsertRegistration(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(outputValue)
                    .Callback(invokeCallback);
            };
            _settuper(setup);
            return this;
        }
    }
    public UpdateRegistrationSetuper UpdateRegistration { get; }
    public class UpdateRegistrationSetuper
    {
        public static HttpResponseMessage DefaultOutput { get; } = new(HttpStatusCode.OK);

        private readonly Action<Action<Mock<IRegistrationHttpClient>>> _settuper;

        public UpdateRegistrationSetuper(Action<Action<Mock<IRegistrationHttpClient>>> settuper) => _settuper = settuper;

        private readonly List<(string UserToken, DateOnly AvalalbleDate, int TimeId)> _inputs = new();
        public IReadOnlyCollection<(string UserToken, DateOnly AvalalbleDate, int TimeId)> Inputs => _inputs;
        public UpdateRegistrationSetuper Setup(string outputFilePath)
        {
            using var outputFile = File.OpenRead(outputFilePath);
            using var reader = new StreamReader(outputFile);
            var content = new StringContent(reader.ReadToEnd());
            var message = new HttpResponseMessage(HttpStatusCode.Created) { Content = content };
            return Setup(message);
        }
        public UpdateRegistrationSetuper Setup(HttpResponseMessage? output = default)
        {
            var setup = (Mock<IRegistrationHttpClient> mock) =>
            {
                var outputValue = output != null ? output : DefaultOutput;
                Action<string, DateOnly, int, CancellationToken> invokeCallback = (userToken, avalableDate, timeId, _) => _inputs.Add((userToken, avalableDate, timeId));
                mock.Setup(s => s.UpdateRegistration(It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(outputValue)
                    .Callback(invokeCallback);
            };
            _settuper(setup);
            return this;
        }
    }

    public GetReserveInfoSetuper GetReserveInfo { get; }
    public class GetReserveInfoSetuper
    {
        public static HttpResponseMessage DefaultOutput { get; } = new(HttpStatusCode.OK);

        private readonly Action<Action<Mock<IRegistrationHttpClient>>> _settuper;

        public GetReserveInfoSetuper(Action<Action<Mock<IRegistrationHttpClient>>> settuper) => _settuper = settuper;

        private readonly List<string> _inputs = new();
        public IReadOnlyCollection<string> Inputs => _inputs;
        public GetReserveInfoSetuper Setup(string outputFilePath)
        {
            using var outputFile = File.OpenRead(outputFilePath);
            using var reader = new StreamReader(outputFile);
            var content = new StringContent(reader.ReadToEnd());
            var message = new HttpResponseMessage(HttpStatusCode.Created) { Content = content };
            return Setup(message);
        }
        public GetReserveInfoSetuper Setup(HttpResponseMessage? output = default)
        {
            var setup = (Mock<IRegistrationHttpClient> mock) =>
            {
                var outputValue = output != null ? output : DefaultOutput;
                Action<string, CancellationToken> invokeCallback = (userToken, _) => _inputs.Add(userToken);
                mock.Setup(s => s.GetReserveInfo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(outputValue)
                    .Callback(invokeCallback);
            };
            _settuper(setup);
            return this;
        }
    }


    public GetAvalableTimeIdSetuper GetAvalableTimeId { get; }
    public class GetAvalableTimeIdSetuper
    {
        public static HttpResponseMessage DefaultOutput { get; } = new(HttpStatusCode.OK);

        private readonly Action<Action<Mock<IRegistrationHttpClient>>> _settuper;

        public GetAvalableTimeIdSetuper(Action<Action<Mock<IRegistrationHttpClient>>> settuper) => _settuper = settuper;

        private readonly List<(DateOnly Avalalble, DateOnly Travel, string Token)> _inputs = new();
        public IReadOnlyCollection<(DateOnly Avalalble, DateOnly Travel, string Token)> Inputs => _inputs;
        public GetAvalableTimeIdSetuper Setup(string outputFilePath)
        {
            using var outputFile = File.OpenRead(outputFilePath);
            using var reader = new StreamReader(outputFile);
            var content = new StringContent(reader.ReadToEnd());
            var message = new HttpResponseMessage(HttpStatusCode.Created) { Content = content };
            return Setup(message);
        }
        public GetAvalableTimeIdSetuper Setup(HttpResponseMessage? output = default)
        {
            var setup = (Mock<IRegistrationHttpClient> mock) =>
            {
                var outputValue = output != null ? output : DefaultOutput;
                Action<DateOnly, DateOnly, string, CancellationToken> invokeCallback = (avalableDate, travelDate, token, _) => _inputs.Add((avalableDate, travelDate, token));
                mock.Setup(s => s.GetAvalableTimeId(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(outputValue)
                    .Callback(invokeCallback);
            };
            _settuper(setup);
            return this;
        }
    }


    public GetAvalableDateSetuper GetAvalableDate { get; }
    public class GetAvalableDateSetuper
    {
        public static HttpResponseMessage DefaultOutput { get; } = new(HttpStatusCode.OK);

        private readonly Action<Action<Mock<IRegistrationHttpClient>>> _settuper;

        public GetAvalableDateSetuper(Action<Action<Mock<IRegistrationHttpClient>>> settuper) => _settuper = settuper;

        private readonly List<string> _inputs = new();
        public IReadOnlyCollection<string> Inputs => _inputs;
        public GetAvalableDateSetuper Setup(string outputFilePath)
        {
            using var outputFile = File.OpenRead(outputFilePath);
            using var reader = new StreamReader(outputFile);
            var content = new StringContent(reader.ReadToEnd());
            var message = new HttpResponseMessage(HttpStatusCode.Created) { Content = content };
            return Setup(message);
        }
        public GetAvalableDateSetuper Setup(HttpResponseMessage? output = default)
        {
            var setup = (Mock<IRegistrationHttpClient> mock) =>
            {
                var outputValue = output != null ? output : DefaultOutput;
                Action<string, CancellationToken> invokeCallback = (token, _) => _inputs.Add(token);
                mock.Setup(s => s.GetAvalableDate(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(outputValue)
                    .Callback(invokeCallback);
            };
            _settuper(setup);
            return this;
        }
    }
}
