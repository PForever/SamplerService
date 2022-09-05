using SamplerService.Workers.PromouteRegistration.WorkerServices;
using SamplerService.Tests.CommonBuilders;
using Moq;

namespace SamplerService.Tests.RegistrationService_Tests.TestBuilders.ServiceBuilders;
internal partial class RegistrationService_TestBuilder
{
    internal class ResposeCacheBuilder : BuilderBase<IResposeCache>
    {
        public ResposeCacheBuilder()
        {
            DateOfRegistration = new(Setup);
        }
        public DateOfRegistrationSetuper DateOfRegistration { get; }
        public class DateOfRegistrationSetuper
        {
            public static ReservInfo? DefaultOutput { get; } = new(Date: new(2022, 06, 2), TimeId: 1);

            private readonly Action<Action<Mock<IResposeCache>>> _settuper;

            public DateOfRegistrationSetuper(Action<Action<Mock<IResposeCache>>> settuper) => _settuper = settuper;

            private readonly List<ReservInfo?> _inputs = new();
            public IReadOnlyCollection<ReservInfo?> Inputs => _inputs;
            public DateOfRegistrationSetuper Setup(ReservInfo? output = default)
            {
                var setup = (Mock<IResposeCache> mock) =>
                {
                    var cache = new MessageCache();
                    ReservInfo? outputValue = output == null ? output : DefaultOutput;
                    Action<ReservInfo?> invokeCallback = m => _inputs.Add(m);
                    mock.SetupSet(s => s.DateOfRegistration).Callback(invokeCallback);
                    mock.SetupGet(s => s.DateOfRegistration).Returns(output);
                };
                _settuper(setup);
                return this;
            }
        }
        protected override void DefaultSetup(Mock<IResposeCache> mock)
        {
            base.DefaultSetup(mock);

            DateOfRegistration.Setup();
        }
    }
}
