using Microsoft.Extensions.Options;
using SamplerService.Workers.PromouteRegistration;
using SamplerService.Tests.CommonBuilders;

namespace SamplerService.Tests.PromouteRegistrationWorker_Tests.TestBuilder.ServiceBuilders;
internal partial class PromouteRegistrationWorker_TestBuilder
{
    public class PromouteRegistrationWorkerSettingsBuilder : IBuilder<IOptions<PromouteRegistrationWorkerSettings>>
    {
        private PromouteRegistrationWorkerSettings _settgings = new() { IsRegistred = true };
       
        public PromouteRegistrationWorkerSettingsBuilder SetIsRegistred(bool isRegistred)
        {
            _settgings.IsRegistred = isRegistred;
            return this;
        }
        public PromouteRegistrationWorkerSettingsBuilder SetDoRegisration(bool doRegisration)
        {
            _settgings.DoRegisration = doRegisration;
            return this;
        }

        public IOptions<PromouteRegistrationWorkerSettings> Build() => Options.Create(_settgings);
    }
}
