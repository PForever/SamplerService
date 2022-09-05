namespace SamplerService.SystemHelpers;

public readonly record struct Result
{
    public bool IsOk { get; }
    public Result(bool isOk)
    {
        IsOk = isOk;
    }
}
public readonly record struct Result<T> where T : class
{
    public T Value => _value != null ? _value : throw new NotInitializedException(nameof(Value));
    private readonly T? _value;
    public bool IsOk { get; }
    public Result(T value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
        IsOk = true;
    }
    public Result()
    {
        _value = null;
        IsOk = false;
    }
}
public readonly record struct ValueResult<T> where T : struct
{
    public T Value => _value.HasValue ? _value.Value : throw new NotInitializedException(nameof(Value));
    private readonly T? _value;
    public bool IsOk { get; }
    public ValueResult(T value)
    {
        _value = value;
        IsOk = true;
    }
    public ValueResult()
    {
        _value = null;
        IsOk = false;
    }
}


[Serializable]
public class NotInitializedException : Exception
{
    public NotInitializedException() { }
    public NotInitializedException(string message) : base(message) { }
    public NotInitializedException(string message, Exception inner) : base(message, inner) { }
    protected NotInitializedException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}