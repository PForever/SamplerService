using SamplerService.CommonServices;
using Moq;
using SamplerService.Tests.CommonBuilders;

namespace SamplerService.Tests.PromouteRegistrationWorker_Tests.TestBuilder.ServiceBuilders;
internal partial class PromouteRegistrationWorker_TestBuilder
{
    public class BotServiceBuilder : BuilderBase<IBotService>
    {
        public BotServiceBuilder()
        {
            SendMessage = new(Setup);
        }
        public SendMessageSetuper SendMessage { get; }
        public class SendMessageSetuper
        {

            private readonly Action<Action<Mock<IBotService>>> _settuper;

            public SendMessageSetuper(Action<Action<Mock<IBotService>>> settuper) => _settuper = settuper;

            private readonly List<string> _inputs = new();
            public IReadOnlyCollection<string> Inputs => _inputs;
            public SendMessageSetuper Setup()
            {
                var setup = (Mock<IBotService> mock) =>
                {
                    Action<string, CancellationToken> invokeCallback = (r, _) => _inputs.Add(r);
                    mock.Setup(s => s.SendMessage(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                        .Callback(invokeCallback);
                };
                _settuper(setup);
                return this;
            }
        }
        protected override void DefaultSetup(Mock<IBotService> mock)
        {
            base.DefaultSetup(mock);

            SendMessage.Setup();
        }
    }
}
