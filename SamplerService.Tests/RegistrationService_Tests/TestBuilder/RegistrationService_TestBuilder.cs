using Moq;
using SamplerService.SystemHelpers;
using SamplerService.Tests.PromouteRegistrationWorker_Tests.TestBuilder.ServiceBuilders;
using SamplerService.Workers.PromouteRegistration.WorkerServices;
using SamplerService.Workers.PromouteRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamplerService.Tests.CommonBuilders;
using static SamplerService.Tests.RegistrationService_Tests.TestBuilders.ServiceBuilders.RegistrationService_TestBuilder;
using SamplerService.Tests.RegistrationService_Tests.TestBuilder.ServiceBuilders;

namespace SamplerService.Tests.RegistrationService_Tests.TestBuilders;
internal partial class RegistrationService_TestBuilder
{
    internal async Task<ActResult<TResult>> Act<TResult>(Func<RegistrationService, CancellationToken, Task<TResult>> act, CancellationToken cancellationToken = default)
    {
        var worker = new RegistrationService(_resposeCacheBuilder.Build(), _settingsBuilder.Build(), _registrationhttpClientBuilder.Build(), _loggerBuilder.Build());
        var result = await act(worker, cancellationToken).ConfigureAwait(false);

        return new ActResult<TResult>(result, _registrationhttpClientBuilder, _resposeCacheBuilder, _settingsBuilder, _loggerBuilder);
    }

    private readonly RegistrationHttpClientBuilder _registrationhttpClientBuilder = new();
    internal RegistrationService_TestBuilder AddRegistrationHttpClient(Action<RegistrationHttpClientBuilder> setuper)
    {
        setuper(_registrationhttpClientBuilder);
        return this;
    }

    private readonly ResposeCacheBuilder _resposeCacheBuilder = new();
    internal RegistrationService_TestBuilder AddResposeCache(Action<ResposeCacheBuilder> setuper)
    {
        setuper(_resposeCacheBuilder);
        return this;
    }

    private readonly RegistrationServiceSettingsBuilder _settingsBuilder = new();
    internal RegistrationService_TestBuilder AddSettings(Action<RegistrationServiceSettingsBuilder> setuper)
    {
        setuper(_settingsBuilder);
        return this;
    }

    private readonly LoggerBuilder<RegistrationService> _loggerBuilder = new();
    internal RegistrationService_TestBuilder AddLogger(Action<LoggerBuilder<RegistrationService>> setuper)
    {
        setuper(_loggerBuilder);
        return this;
    }

    
}
