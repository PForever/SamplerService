namespace SamplerService.Workers;

interface IBusinessWorker
{
    Task DoWorkAsync(CancellationToken token);
    public TimeSpan Delay { get; }
}
