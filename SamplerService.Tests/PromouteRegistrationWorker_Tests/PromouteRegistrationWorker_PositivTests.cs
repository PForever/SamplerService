using SamplerService.Tests.PromouteRegistrationWorker_Tests.TestBuilder.ServiceBuilders;
using SamplerService.Workers.PromouteRegistration.WorkerServices;
using Xunit;

namespace SamplerService.Tests.PromouteRegistrationWorker_Tests;

public class PromouteRegistrationWorker_PositivTests
{
    [Fact]
    public async Task UpdateRegisterationWhenHasAvalableDate()
    {
        var currentReserv = new ReservInfo(Date: new(2022, 10, 13), TimeId: 1);
        var avalableReserv = new ReservInfo(Date: new(2022, 10, 11), TimeId: 1);

        var builder = new PromouteRegistrationWorker_TestBuilder()
            .AddRegistrationService(s =>
            {
                s.TryGetAvalableRegistration.Setup(new(avalableReserv));
                s.TryGetReserveInfo.Setup(new(currentReserv));
                s.TryUpdateRegistration.Setup();
                s.TryInsertRegistration.Setup();
            })
            .AddBotService(s => s.SendMessage.Setup())
            .AddPromouteRegistrationWorkerSettings(s => s.SetDelay(10).SetIsRegistred(true).SetDoRegisration(true))
            .AddMessageCache(s => s.LastMessage.Setup())
            .AddLogger(s => s.Log.Setup());

        var result = await builder.Act((s, c) => s.DoWorkAsync(c));

        var updateInputs = result.RegistrationService.TryUpdateRegistration.Inputs;
        var insertInputs = result.RegistrationService.TryInsertRegistration.Inputs;

        Assert.Contains(avalableReserv, updateInputs);
        Assert.True(updateInputs.Count == 1);
        Assert.True(insertInputs.Count == 0);

        Assert.Contains("Get reservation info..", result.Logger.Log.Info);
    }
    [Fact]
    public async Task InsertregisterationWhenHasAvalableDate()
    {
        var currentReserv = new ReservInfo(Date: new(2022, 10, 13), TimeId: 1);
        var avalableReserv = new ReservInfo(Date: new(2022, 10, 11), TimeId: 1);

        var builder = new PromouteRegistrationWorker_TestBuilder()
            .AddRegistrationService(s =>
            {
                s.TryGetAvalableRegistration.Setup(new(avalableReserv));
                s.TryGetReserveInfo.Setup(new(currentReserv));
                s.TryUpdateRegistration.Setup();
                s.TryInsertRegistration.Setup();
            })
            .AddBotService(s => s.SendMessage.Setup())
            .AddPromouteRegistrationWorkerSettings(s => s.SetDelay(10).SetIsRegistred(false).SetDoRegisration(true))
            .AddMessageCache(s => s.LastMessage.Setup())
            .AddLogger(s => s.Log.Setup());

        var result = await builder.Act((s, c) => s.DoWorkAsync(c));

        var updateInputs = result.RegistrationService.TryUpdateRegistration.Inputs;
        var insertInputs = result.RegistrationService.TryInsertRegistration.Inputs;

        Assert.Contains(avalableReserv, insertInputs);
        Assert.True(insertInputs.Count == 1);
        Assert.True(updateInputs.Count == 0);
    }
    [Fact]
    public async Task DoNotPostWhen_DoRegistration_IsDisabled()
    {
        var currentReserv = new ReservInfo(Date: new(2022, 10, 13), TimeId: 1);
        var avalableReserv = new ReservInfo(Date: new(2022, 10, 11), TimeId: 1);

        var builder = new PromouteRegistrationWorker_TestBuilder()
            .AddRegistrationService(s =>
            {
                s.TryGetAvalableRegistration.Setup(new(avalableReserv));
                s.TryGetReserveInfo.Setup(new(currentReserv));
                s.TryUpdateRegistration.Setup();
                s.TryInsertRegistration.Setup();
            })
            .AddBotService(s => s.SendMessage.Setup())
            .AddPromouteRegistrationWorkerSettings(s => s.SetDelay(10).SetIsRegistred(false).SetDoRegisration(false))
            .AddMessageCache(s => s.LastMessage.Setup())
            .AddLogger(s => s.Log.Setup());

        var result = await builder.Act((s, c) => s.DoWorkAsync(c));

        var updateInputs = result.RegistrationService.TryUpdateRegistration.Inputs;
        var insertInputs = result.RegistrationService.TryInsertRegistration.Inputs;

        Assert.True(insertInputs.Count == 0);
        Assert.True(updateInputs.Count == 0);
    }
}