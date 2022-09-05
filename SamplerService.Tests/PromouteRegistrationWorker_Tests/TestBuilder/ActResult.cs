using SamplerService.Tests.CommonBuilders;
using SamplerService.Workers.PromouteRegistration;
using static SamplerService.Tests.PromouteRegistrationWorker_Tests.TestBuilder.ServiceBuilders.PromouteRegistrationWorker_TestBuilder;

namespace SamplerService.Tests.PromouteRegistrationWorker_Tests.TestBuilder;
internal partial class PromouteRegistrationWorker_TestBuilder
{
    public class ActResult
    {
        public RegistrationServiceBuilder RegistrationService;
        public BotServiceBuilder BotService;
        public MessageCacheBuilder MessageCache;
        public PromouteRegistrationWorkerSettingsBuilder Settings;
        public LoggerBuilder<PromouteRegistrationWorker> Logger;
        public ActResult(RegistrationServiceBuilder registrationServiceBuilder, BotServiceBuilder botServiceBuilder, MessageCacheBuilder messageCacheBuilder, PromouteRegistrationWorkerSettingsBuilder settingsBuilder, LoggerBuilder<PromouteRegistrationWorker> loggerBuilder)
        {
            RegistrationService = registrationServiceBuilder;
            BotService = botServiceBuilder;
            MessageCache = messageCacheBuilder;
            Settings = settingsBuilder;
            Logger = loggerBuilder;
        }
    }
}
