using Knapcode.TorSharp;

namespace SamplerService.CommonServices.Proxy;
public interface ITorProxy
{
    Task ChangeIp();
    Task Connect();
}

public class TorProxy : ITorProxy
{
    private TorSharpProxy _proxy;
    private readonly ILogger<TorProxy> _logger;

    public TorProxy(ITorSharpSettingsFactory settingsFactory, ILogger<TorProxy> logger)
    {
        _proxy = new TorSharpProxy(settingsFactory.Settings);
        _logger = logger;
    }
    public async Task Connect()
    {
        await _proxy.ConfigureAndStartAsync();
        _logger.LogInformation("Proxy connected");
    }

    public async Task ChangeIp()
    {
        await _proxy.GetNewIdentityAsync();
        _logger.LogInformation("Ip changed");
    }
}
