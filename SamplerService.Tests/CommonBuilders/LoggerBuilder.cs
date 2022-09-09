using Microsoft.Extensions.Logging;
using SamplerService.Workers.PromouteRegistration;
using Moq;
using SamplerService.SystemHelpers;

namespace SamplerService.Tests.CommonBuilders;

internal class LoggerBuilder<T> : BuilderBase<ILogger<T>>
{
    public LoggerBuilder()
    {
        Log = new(Setup);
    }
    public LogSetuper Log { get; }
    public class LogSetuper
    {
        public static Result DefaultOutput { get; } = new(true);

        private readonly Action<Action<Mock<ILogger<T>>>> _settuper;

        public LogSetuper(Action<Action<Mock<ILogger<T>>>> settuper) => _settuper = settuper;

        public IReadOnlyCollection<string?> Info => _inputs?[LogLevel.Information] ?? new();
        private readonly Dictionary<LogLevel, List<string?>> _inputs = new();
        public LogSetuper Setup(Result? output = default)
        {
            var setup = (Mock<ILogger<T>> mock) =>
            {
                var outputValue = output.HasValue ? output.Value : DefaultOutput;
                Action<LogLevel, EventId, object, Exception?, Delegate> invokeCallback = (l, _, s, ex, f) =>
                {
                    var message = f.DynamicInvoke(s, ex) as string;
                    if (!_inputs.ContainsKey(l)) _inputs.Add(l, new List<string?>());
                    _inputs[l].Add(message);
                };
                mock.Setup(s => s.Log(It.IsAny<LogLevel>(),
                                      It.IsAny<EventId>(),
                                      It.IsAny<It.IsAnyType>(),
                                      It.IsAny<Exception?>(),
                                      It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
                    .Callback(invokeCallback);
            };
            _settuper(setup);
            return this;
        }
    }
    protected override void DefaultSetup(Mock<ILogger<T>> mock)
    {
        base.DefaultSetup(mock);

        Log.Setup();
    }
}
