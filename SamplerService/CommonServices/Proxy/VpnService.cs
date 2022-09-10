using Knapcode.TorSharp;
using SamplerService.SystemHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SamplerService.CommonServices.Proxy;
public interface IVpnService
{
    Task ChangeIp();
}
public class VpnService : IVpnService
{
    public VpnService(ITorSharpSettingsFactory settingsFactory, IHttpClientFactory httpClientFactory, ITorProxy proxy)
    {
        _settings = settingsFactory.Settings;
        _preparing = EnvPrepare(_settings, httpClientFactory);
        _proxy = proxy;
    }

    private readonly TorSharpSettings _settings;
    private readonly Task _preparing;
    private readonly ITorProxy _proxy;

    private async Task EnvPrepare(TorSharpSettings settings, IHttpClientFactory httpClientFactory)
    {
        using var httpClient = httpClientFactory.CreateClient();
        await new TorSharpToolFetcher(settings, httpClient).FetchAsync();
        await _proxy.Connect();
    }
    public async Task ChangeIp()
    {
        await _preparing;
        await _proxy.ChangeIp();
    }
}