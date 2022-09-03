using Microsoft.Extensions.Options;
namespace SamplerService;
public static class Extensions
{
    public static T GetConfiguration<T>(this IServiceProvider serviceProvider)
        where T: class
    {
        var o = serviceProvider.GetService<IOptions<T>>();
        if (o is null)
            throw new ArgumentNullException(nameof(T));

        return o.Value;
    }
    public static IServiceCollection AddConfiguration<T>(this IServiceCollection services, IConfiguration configuration) where T : class
    {
        services.Configure<T>(configuration.GetSection(typeof(T).Name));
        return services;
    }
}