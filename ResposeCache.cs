using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplerService;

public interface IResposeCache
{
    SampleResult CashedRespose { get; set; }
}

public class ResposeCache : IResposeCache
{
    public SampleResult CashedRespose { get; set; }
}
