using SamplerService.Workers.PromouteRegistration.WorkerServices;
using SamplerService.Tests.CommonBuilders;
using static SamplerService.Tests.RegistrationService_Tests.TestBuilders.ServiceBuilders.RegistrationService_TestBuilder;

namespace SamplerService.Tests.RegistrationService_Tests.TestBuilders;
internal partial class RegistrationService_TestBuilder
{
    internal class ActResult<T>
    {
        public ActResult(T result, HttpClientFactoryBuilder httpClientFactoryBuilder, ResposeCacheBuilder resposeCacheBuilder, RegistrationServiceSettingsBuilder settingsBuilder, LoggerBuilder<RegistrationService> loggerBuilder)
        {
            Result = result;
            HttpClientFactoryBuilder = httpClientFactoryBuilder;
            ResposeCacheBuilder = resposeCacheBuilder;
            SettingsBuilder = settingsBuilder;
            LoggerBuilder = loggerBuilder;
        }

        public T Result { get; }
        public HttpClientFactoryBuilder HttpClientFactoryBuilder { get; }
        public ResposeCacheBuilder ResposeCacheBuilder { get; }
        public RegistrationServiceSettingsBuilder SettingsBuilder { get; }
        public LoggerBuilder<RegistrationService> LoggerBuilder { get; }
    }
}
