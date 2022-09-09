using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplerService.Workers.PromouteRegistration.WorkerServices;

public interface IResposeCache
{
    ReservInfo? DateOfRegistration { get; set; }
}

public class ResposeCache : IResposeCache
{
    public ReservInfo? DateOfRegistration { get; set; }
}
