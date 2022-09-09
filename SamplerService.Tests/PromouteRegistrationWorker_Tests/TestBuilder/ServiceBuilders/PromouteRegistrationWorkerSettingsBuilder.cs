using Microsoft.Extensions.Options;
using SamplerService.Workers.PromouteRegistration;
using SamplerService.Tests.CommonBuilders;

namespace SamplerService.Tests.PromouteRegistrationWorker_Tests.TestBuilder.ServiceBuilders;
internal partial class PromouteRegistrationWorker_TestBuilder
{
    public class PromouteRegistrationWorkerSettingsBuilder : IBuilder<IOptions<PromouteRegistrationWorkerSettings>>
    {
        private PromouteRegistrationWorkerSettings _settgings = new() { IsRegistred = true, JobDelaySeconds = 5 };
        public PromouteRegistrationWorkerSettingsBuilder SetDelay(int delay)
        {
            _settgings.JobDelaySeconds = delay;
            return this;
        }
        public PromouteRegistrationWorkerSettingsBuilder SetIsRegistred(bool isRegistred)
        {
            _settgings.IsRegistred = isRegistred;
            return this;
        }

        public IOptions<PromouteRegistrationWorkerSettings> Build() => Options.Create(_settgings);
    }
}
