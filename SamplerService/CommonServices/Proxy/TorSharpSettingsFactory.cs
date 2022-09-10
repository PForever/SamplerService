using Knapcode.TorSharp;

namespace SamplerService.CommonServices.Proxy;

public interface ITorSharpSettingsFactory
{
    TorSharpSettings Settings { get; }
}
public class TorSharpSettingsFactory : ITorSharpSettingsFactory
{
    public TorSharpSettingsFactory()
    {

    }
    private readonly Lazy<TorSharpSettings> _settingsLazy = new Lazy<TorSharpSettings>(() => new TorSharpSettings
    {
        ZippedToolsDirectory = Path.Combine(Path.GetTempPath(), "TorZipped"),
        ExtractedToolsDirectory = Path.Combine(Path.GetTempPath(), "TorExtracted"),
        PrivoxySettings = { Port = 1337 },
        TorSettings =
        {
            SocksPort = 1338,
            ControlPort = 1339,
            ControlPassword = "foobar",
        }
    });
    public TorSharpSettings Settings => _settingsLazy.Value;
}
