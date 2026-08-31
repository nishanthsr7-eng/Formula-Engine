namespace FormulaEngine.Core.Functions;

internal sealed class DelegateFunction : IFunction
{
    private readonly Func<IReadOnlyList<double>, double> _impl;

    public string Name { get; }
    public int MinArgs { get; }
    public int MaxArgs { get; }

    public DelegateFunction(string name, int minArgs, int maxArgs, Func<IReadOnlyList<double>, double> impl)
    {
        Name = name;
        MinArgs = minArgs;
        MaxArgs = maxArgs;
        _impl = impl;
    }

    public double Invoke(IReadOnlyList<double> args) => _impl(args);
}
