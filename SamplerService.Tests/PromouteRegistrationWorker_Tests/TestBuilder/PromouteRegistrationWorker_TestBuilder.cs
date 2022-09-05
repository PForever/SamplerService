using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamplerService.Workers.PromouteRegistration;
using System.Threading.Tasks.Dataflow;
using SamplerService.Tests.CommonBuilders;
using static SamplerService.Tests.PromouteRegistrationWorker_Tests.TestBuilder.PromouteRegistrationWorker_TestBuilder;

namespace SamplerService.Tests.PromouteRegistrationWorker_Tests.TestBuilder.ServiceBuilders;
internal partial class PromouteRegistrationWorker_TestBuilder
{
    public async Task<ActResult> Act(Func<PromouteRegistrationWorker, CancellationToken, Task> act, CancellationToken cancellationToken = default)
    {
        var worker = new PromouteRegistrationWorker(_registrationServiceBuilder.Build(),
                                                    _botServiceBuilder.Build(),
                                                    _messageCacheBuilder.Build(),
                                                    _settingsBuilder.Build(),
                                                    _loggerBuilder.Build());

        await act(worker, cancellationToken).ConfigureAwait(false);

        return new ActResult(_registrationServiceBuilder, _botServiceBuilder, _messageCacheBuilder, _settingsBuilder, _loggerBuilder);
    }

    private readonly RegistrationServiceBuilder _registrationServiceBuilder = new();
    public PromouteRegistrationWorker_TestBuilder AddRegistrationService(Action<RegistrationServiceBuilder> setuper)
    {
        setuper(_registrationServiceBuilder);
        return this;
    }


    private readonly BotServiceBuilder _botServiceBuilder = new();
    public PromouteRegistrationWorker_TestBuilder AddBotService(Action<BotServiceBuilder> setuper)
    {
        setuper(_botServiceBuilder);
        return this;
    }


    private readonly MessageCacheBuilder _messageCacheBuilder = new();
    public PromouteRegistrationWorker_TestBuilder AddMessageCache(Action<MessageCacheBuilder> setuper)
    {
        setuper(_messageCacheBuilder);
        return this;
    }

    private readonly PromouteRegistrationWorkerSettingsBuilder _settingsBuilder = new();
    public PromouteRegistrationWorker_TestBuilder AddPromouteRegistrationWorkerSettings(Action<PromouteRegistrationWorkerSettingsBuilder> setuper)
    {
        setuper(_settingsBuilder);
        return this;
    }

    private readonly LoggerBuilder<PromouteRegistrationWorker> _loggerBuilder = new();
    public PromouteRegistrationWorker_TestBuilder AddLogger(Action<LoggerBuilder<PromouteRegistrationWorker>> setuper)
    {
        setuper(_loggerBuilder);
        return this;
    }
}
