using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplerService.SystemHelpers;
public static class RetryExtantions
{
    public static async Task<ValueResult<TResult>> RetryFor<TResult>(this Func<Task<ValueResult<TResult>>> processor, int count, TimeSpan delay) where TResult : struct
    {
        for (int i = 0; i < count; i++)
        {
            var result = await processor();
            if (result.IsOk) return result;
            await Task.Delay(delay);
        }
        return new();
    }
    public static async Task<Result<TResult>> RetryFor<TResult>(this Func<Task<Result<TResult>>> processor, int count, TimeSpan delay) where TResult : class
    {
        for (int i = 0; i < count; i++)
        {
            var result = await processor();
            if (result.IsOk) return result;
            await Task.Delay(delay);
        }
        return new();
    }
    public static async Task<Result> RetryFor(this Func<Task<Result>> processor, int count, TimeSpan delay)
    {
        for (int i = 0; i < count; i++)
        {
            var result = await processor();
            if (result.IsOk) return result;
            await Task.Delay(delay);
        }
        return new();
    }

    public static async Task<Result<HttpResponseMessage>> InokeRequest(this Func<Task<HttpResponseMessage>> processor, ILogger logger)
    {
        HttpResponseMessage request;
        try
        {
            request = await processor();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Http request failed");
            return new();
        }
        if (request.IsSuccessStatusCode is true) return new(request);
        
        logger.LogError($"[{request.StatusCode}]: {await request.Content.ReadAsStringAsync()}");
        return new();
    }
}
