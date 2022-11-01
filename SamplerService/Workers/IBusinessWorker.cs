namespace SamplerService.Workers;

interface IBusinessWorker
{
    Task DoWorkAsync(CancellationToken token);
}
