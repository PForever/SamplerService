using SamplerService.SystemHelpers;
using SamplerService.Tests.RegistrationService_Tests.TestBuilders;
using SamplerService.Workers.PromouteRegistration.WorkerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SamplerService.Tests.RegistrationService_Tests;
public class RegistrationService_PositivTest
{
    [Fact]
    public async Task TryGetAvalableRegistration_WithoutCache()
    {
        string userPhone = "79005554433";
        string userToken = "#myToken#";
        DateTime travalDate = new DateTime(2022, 12, 30);

        var builder = new RegistrationService_TestBuilder()
            .AddResposeCache(s => s.DateOfRegistration.Setup(null))
            .AddSettings(s => s.SetUserPhone(userPhone).SetUserToken(userToken).SetTryCont(1).SetUserTrevalDate(travalDate))
            .AddRegistrationHttpClient(s =>
            {
                s.GetRegistrationForm.Setup(@"RegistrationService_Tests\TestBuilder\Static\GetRegistrationForm_OK.html");
                s.GetAvalableDate.Setup(@"RegistrationService_Tests\TestBuilder\Static\GetAvalableDate_OK.html");
                s.GetAvalableTimeId.Setup(@"RegistrationService_Tests\TestBuilder\Static\GetAvalableTimeId_OK.html");
                s.GetReserveInfo.Setup(@"RegistrationService_Tests\TestBuilder\Static\GetReserveInfo_OK.html");
                s.UpdateRegistration.Setup();
                s.InsertRegistration.Setup();
            })
            .AddLogger(s => s.Log.Setup());


        var result = await builder.Act((s, c) => s.TryGetAvalableRegistration(c));
        Assert.Equal(new(new(Date: new(2022, 10, 05), TimeId: 1272)), result.Result);
    }
    [Fact]
    public async Task TryGetReserveInfo()
    {
        string userPhone = "79005554433";
        string userToken = "#myToken#";
        DateTime travalDate = new DateTime(2022, 12, 30);

        var builder = new RegistrationService_TestBuilder()
            .AddResposeCache(s => s.DateOfRegistration.Setup(null))
            .AddSettings(s => s.SetUserPhone(userPhone).SetUserToken(userToken).SetTryCont(1).SetUserTrevalDate(travalDate))
            .AddRegistrationHttpClient(s =>
            {
                s.GetAvalableDate.Setup(@"RegistrationService_Tests\TestBuilder\Static\GetAvalableDate_OK.html");
                s.GetAvalableTimeId.Setup(@"RegistrationService_Tests\TestBuilder\Static\GetAvalableTimeId_OK.html");
                s.GetReserveInfo.Setup(@"RegistrationService_Tests\TestBuilder\Static\GetReserveInfo_OK.html");
                s.UpdateRegistration.Setup();
                s.InsertRegistration.Setup();
            })
            .AddLogger(s => s.Log.Setup());


        var result = await builder.Act((s, c) => s.TryGetReserveInfo(c));
        Assert.Equal(new(new(Date: new(2022, 10, 05), TimeId: -1)), result.Result);
    }
    [Fact]
    public async Task TryUpdateRegistration()
    {

    }
    [Fact]
    public async Task TryInsertRegistration()
    {

    }
}
