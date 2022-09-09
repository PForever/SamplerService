using SamplerService.Workers.PromouteRegistration.WorkerServices;
using SamplerService.Tests.CommonBuilders;
using Microsoft.Extensions.Options;
using SamplerService.Workers.PromouteRegistration;
using static SamplerService.Tests.PromouteRegistrationWorker_Tests.TestBuilder.ServiceBuilders.PromouteRegistrationWorker_TestBuilder;

namespace SamplerService.Tests.RegistrationService_Tests.TestBuilders.ServiceBuilders;
internal partial class RegistrationService_TestBuilder
{
    internal class RegistrationServiceSettingsBuilder : IBuilder<IOptions<RegistrationServiceSettings>>
    {
        private RegistrationServiceSettings _settgings = new() { TryCont = 1, UserPhone = "555", UserToken = "token", UserTrevalDate = new(2022, 02, 02)};
        public RegistrationServiceSettingsBuilder SetTryCont(int tryCount)
        {
            _settgings.TryCont = tryCount;
            return this;
        }
        public RegistrationServiceSettingsBuilder SetUserTrevalDate(DateTime userTrevalDate)
        {
            _settgings.UserTrevalDate = userTrevalDate;
            return this;
        }
        public RegistrationServiceSettingsBuilder SetUserToken(string userToken)
        {
            _settgings.UserToken = userToken;
            return this;
        }
        public RegistrationServiceSettingsBuilder SetUserPhone(string userPhone)
        {
            _settgings.UserPhone = userPhone;
            return this;
        }

        public IOptions<RegistrationServiceSettings> Build() => Options.Create(_settgings);
    }

    
}
