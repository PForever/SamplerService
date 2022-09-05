using SamplerService.Tests.RegistrationService_Tests.TestBuilders;
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
            .AddHttpClientFactory(GetTime, GetDate)
            ;
    }
    [Fact]
    public async Task TryGetReserveInfo()
    {

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
