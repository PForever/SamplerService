using SamplerService.SystemHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplerService.Workers.PromouteRegistration.WorkerServices;
public interface IVpnService
{
    Task<Result> Reconnect();
}
public class VpnService : IVpnService
{
    public Task<Result> Reconnect() => throw new NotImplementedException();
}
