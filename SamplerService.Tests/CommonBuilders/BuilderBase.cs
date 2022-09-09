using Moq;

namespace SamplerService.Tests.CommonBuilders;

class BuilderBase<T> : IBuilder<T> where T : class
{
    private Mock<T> _mock { get; } = new();

    private bool _isSettedUp;
    protected virtual void DefaultSetup(Mock<T> mock) => _isSettedUp = true;

    protected void Setup(Action<Mock<T>> setupper)
    {
        if (setupper is null) throw new ArgumentNullException(nameof(setupper));
        _isSettedUp = true;
        setupper(_mock);
    }

    public T Build()
    {
        if (!_isSettedUp) DefaultSetup(_mock);
        return _mock.Object;
    }
}