namespace SamplerService;

public interface IMessageCache
{
    string? LastMessage { get; set; }
}

public class MessageCache : IMessageCache
{
    public string? LastMessage { get; set; }
}