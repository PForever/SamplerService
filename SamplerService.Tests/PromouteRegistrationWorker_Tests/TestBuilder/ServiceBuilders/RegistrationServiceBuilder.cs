using SamplerService.Workers.PromouteRegistration.WorkerServices;
using Moq;
using SamplerService.SystemHelpers;
using SamplerService.Tests.CommonBuilders;

namespace SamplerService.Tests.PromouteRegistrationWorker_Tests.TestBuilder.ServiceBuilders;
internal partial class PromouteRegistrationWorker_TestBuilder
{
    public class RegistrationServiceBuilder : BuilderBase<IRegistrationService>
    {
        public RegistrationServiceBuilder()
        {
            TryUpdateRegistration = new(Setup);
            TryGetReserveInfo = new(Setup);
            TryGetAvalableRegistration = new(Setup);
            TryInsertRegistration = new(Setup);
        }
        public TryUpdateRegistrationSetuper TryUpdateRegistration { get; }
        public class TryUpdateRegistrationSetuper
        {
            public static Result DefaultOutput { get; } = new(true);

            private readonly Action<Action<Mock<IRegistrationService>>> _settuper;

            public TryUpdateRegistrationSetuper(Action<Action<Mock<IRegistrationService>>> settuper) => _settuper = settuper;
          
            private readonly List<ReservInfo> _inputs = new();
            public IReadOnlyCollection<ReservInfo> Inputs => _inputs;
            public TryUpdateRegistrationSetuper Setup(Result? output = default)
            {
                var setup = (Mock<IRegistrationService> mock) =>
                {
                    var outputValue = output.HasValue ? output.Value : DefaultOutput;
                    Action<ReservInfo, CancellationToken> invokeCallback = (r, _) => _inputs.Add(r);
                    mock.Setup(s => s.TryUpdateRegistration(It.IsAny<ReservInfo>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(outputValue)
                        .Callback(invokeCallback);
                };
                _settuper(setup);
                return this;
            }
        }

        public TryInsertRegistrationSetuper TryInsertRegistration { get; }
        public class TryInsertRegistrationSetuper
        {
            public static Result DefaultOutput { get; } = new(true);

            private readonly Action<Action<Mock<IRegistrationService>>> _settuper;

            public TryInsertRegistrationSetuper(Action<Action<Mock<IRegistrationService>>> settuper) => _settuper = settuper;

            private readonly List<ReservInfo> _inputs = new();
            public IReadOnlyCollection<ReservInfo> Inputs => _inputs;
            public TryInsertRegistrationSetuper Setup(Result? output = default)
            {
                var setup = (Mock<IRegistrationService> mock) =>
                {
                    var outputValue = output.HasValue ? output.Value : DefaultOutput;
                    Action<ReservInfo, CancellationToken> invokeCallback = (r, _) => _inputs.Add(r);
                    mock.Setup(s => s.TryInsertRegistration(It.IsAny<ReservInfo>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(outputValue)
                        .Callback(invokeCallback);
                };
                _settuper(setup);
                return this;
            }
        }


        public TryGetReserveInfoSetuper TryGetReserveInfo { get; }
        public class TryGetReserveInfoSetuper
        {
            public static ValueResult<ReservInfo> DefaultOutput { get; } = new(new(new(12, 10, 2020), 1));

            private readonly Action<Action<Mock<IRegistrationService>>> _settuper;

            public TryGetReserveInfoSetuper(Action<Action<Mock<IRegistrationService>>> settuper) => _settuper = settuper;

            public TryGetReserveInfoSetuper Setup(ValueResult<ReservInfo>? output = default)
            {
                var setup = (Mock<IRegistrationService> mock) =>
                {
                    var outputValue = output.HasValue ? output.Value : DefaultOutput;
                    mock.Setup(s => s.TryGetReserveInfo(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(outputValue);
                };
                _settuper(setup);
                return this;
            }
        }
        public TryGetAvalableRegistrationSetuper TryGetAvalableRegistration { get; }
        public class TryGetAvalableRegistrationSetuper
        {
            public static ValueResult<ReservInfo> DefaultOutput { get; } = new(new(new(10, 10, 2020), 1));

            private readonly Action<Action<Mock<IRegistrationService>>> _settuper;

            public TryGetAvalableRegistrationSetuper(Action<Action<Mock<IRegistrationService>>> settuper) => _settuper = settuper;

            public TryGetAvalableRegistrationSetuper Setup(ValueResult<ReservInfo>? output = default)
            {
                var setup = (Mock<IRegistrationService> mock) =>
                {
                    var outputValue = output.HasValue ? output.Value : DefaultOutput;
                    mock.Setup(s => s.TryGetAvalableRegistration(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(outputValue);
                };
                _settuper(setup);
                return this;
            }
        }

        protected override void DefaultSetup(Mock<IRegistrationService> mock)
        {
            base.DefaultSetup(mock);

            TryGetReserveInfo.Setup();
            TryGetAvalableRegistration.Setup();
            TryUpdateRegistration.Setup();
            TryInsertRegistration.Setup();
        }
    }
}
