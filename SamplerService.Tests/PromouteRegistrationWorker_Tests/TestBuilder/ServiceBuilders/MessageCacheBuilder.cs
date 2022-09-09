using SamplerService.Workers.PromouteRegistration.WorkerServices;
using Moq;
using SamplerService.Tests.CommonBuilders;

namespace SamplerService.Tests.PromouteRegistrationWorker_Tests.TestBuilder.ServiceBuilders;
internal partial class PromouteRegistrationWorker_TestBuilder
{
    public class MessageCacheBuilder : BuilderBase<IMessageCache>
    {
        public MessageCacheBuilder()
        {
            LastMessage = new(Setup);
        }
        public LastMessageSetuper LastMessage { get; }
        public class LastMessageSetuper
        {
            public static string? DefaultOutput { get; } = null;

            private readonly Action<Action<Mock<IMessageCache>>> _settuper;

            public LastMessageSetuper(Action<Action<Mock<IMessageCache>>> settuper) => _settuper = settuper;

            private readonly List<string?> _inputs = new();
            public IReadOnlyCollection<string?> Inputs => _inputs;
            public LastMessageSetuper Setup(string? output = default)
            {
                var setup = (Mock<IMessageCache> mock) =>
                {
                    var cache = new MessageCache();
                    string? outputValue = output == null ? output : DefaultOutput;
                    Action<string> invokeCallback = m => _inputs.Add(m);
                    mock.SetupSet(s => s.LastMessage).Callback(invokeCallback);
                    mock.SetupGet(s => s.LastMessage).Returns(output);
                };
                _settuper(setup);
                return this;
            }
        }
        protected override void DefaultSetup(Mock<IMessageCache> mock)
        {
            base.DefaultSetup(mock);

            LastMessage.Setup();
        }
    }
}
