using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace SamplerService.SystemHelpers;
public static class SettingsExtensions
{
    public static T GetConfiguration<T>(this IServiceProvider serviceProvider)
        where T : class
    {
        var o = serviceProvider.GetService<IOptions<T>>();
        if (o is null)
            throw new ArgumentNullException(nameof(T));

        return o.Value;
    }
    private static Regex settingsPattern = new Regex("(.+)Settings$");
    public static IServiceCollection AddConfiguration<T>(this IServiceCollection services, IConfiguration configuration) where T : class
    {
        static string RemoveSuffix(string name) => settingsPattern.IsMatch(name) ? settingsPattern.Replace(name, "$1") : name;
        services.Configure<T>(configuration.GetSection(RemoveSuffix(typeof(T).Name)));
        return services;
    }
}