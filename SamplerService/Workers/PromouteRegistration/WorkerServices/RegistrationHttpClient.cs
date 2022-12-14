using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SamplerService.CommonServices;
using Method = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace SamplerService.Workers.PromouteRegistration.WorkerServices;

public interface IRegistrationHttpClient : IBusinessHttpClient
{
	Task<HttpResponseMessage> GetRegistrationForm(CancellationToken token);
	Task<HttpResponseMessage> GetAvalableDate(string registrationToken, CancellationToken token);
	Task<HttpResponseMessage> GetAvalableTimeId(DateOnly avalableDate, DateOnly userTravalDate, string registrationToken, CancellationToken token);
	Task<HttpResponseMessage> GetReserveInfo(string userToken, CancellationToken token);
	Task<HttpResponseMessage> UpdateRegistration(string userToken, DateOnly avalableDate, int avalableTimeId, CancellationToken token);
	Task<HttpResponseMessage> InsertRegistration(string userToken, string userPhone, DateOnly avalableDate, int avalableTimeId, CancellationToken token);
}
public class RegistrationHttpClient : IRegistrationHttpClient
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<RegistrationHttpClient> _logger;

    public static string ClientName => "VisaTimetable";
    public static string BaseUrl => "https://italy-vms.ru/";

    public RegistrationHttpClient(IHttpClientFactory httpClientFactory, ILogger<RegistrationHttpClient> logger)
	{
		_httpClient = httpClientFactory.CreateClient(ClientName);
		_logger = logger;
	}

    public Task<HttpResponseMessage> GetRegistrationForm(CancellationToken token)
	{
        string url = "autoform/";
        TraceLog(Method.Get, url);
        return _httpClient.GetAsync(url, token);
    }


    public Task<HttpResponseMessage> GetAvalableDate(string registrationToken, CancellationToken token)
	{
		string url = $"vcs/get_nearest.htm?center=11&persons=&urgent=0&token={registrationToken}&lang=ru";
        TraceLog(Method.Get, url);
        return _httpClient.GetAsync(url, token);
    }

	public Task<HttpResponseMessage> GetAvalableTimeId(DateOnly avalableDate, DateOnly userTravalDate, string registrationToken, CancellationToken token)
	{
        var url = $"vcs/get_times.htm?vtype=6&center=11&persons=1&appdate={avalableDate:dd.MM.yyyy}&fdate={/*TODO Add GetTravelDate*/userTravalDate:dd.MM.yyyy}&token={registrationToken}&lang=ru";
        TraceLog(Method.Get, url);
        return _httpClient.GetAsync(url, token);
	}

	public Task<HttpResponseMessage> GetReserveInfo(string userToken, CancellationToken token)
	{
        var url = $"autoform/?t={userToken}&action=reschedule";
        TraceLog(Method.Get, url);
		return _httpClient.GetAsync(url, token);
    }

	public Task<HttpResponseMessage> InsertRegistration(string userToken, string userPhone, DateOnly avalableDate, int avalableTimeId, CancellationToken token)
	{
        var url = $"autoform/?t={userToken}&lang=ru";
        var content = new StringContent(
            $"action=forward" +
            $"&mobile_ver=0" +
            $"&mobile_app=0" +
            $"&biometric_data=0" +
            $"&last_error_return=" +
            $"&urgent_slots=0" +
            $"&finger=" +
            $"&sms={userPhone}" +
            $"&app_date={avalableDate:dd.MM.yyyy}" +
            $"&timeslot={avalableTimeId}");
        TraceLog(Method.Post, url, content);
        return _httpClient.PostAsync(url, content, token);
    }
	public Task<HttpResponseMessage> UpdateRegistration(string userToken, DateOnly avalableDate, int avalableTimeId, CancellationToken token)
	{
        var url = $"autoform/?t={userToken}&lang=ru";
        var content = new StringContent($"action=reschedule&appdata=&appdate={avalableDate:dd.MM.yyyy}&apptime={avalableTimeId}");
        TraceLog(Method.Post, url, content);
        return _httpClient.PostAsync(url, content, token);
    }

	private void TraceLog(Method method, string url) => _logger.LogTrace("{Method}: url: {Url}", method, url);
	private void TraceLog(Method method, string url, HttpContent content) => _logger.LogTrace("{Method}: url: {Url}, body: {Content}", method, url, new ContentReader(content));
}

class ContentReader
{
	private readonly HttpContent _content;

	public ContentReader(HttpContent content)
	{
		_content = content;
	}
	public override string ToString() => _content.ReadAsStringAsync().Result;
}
