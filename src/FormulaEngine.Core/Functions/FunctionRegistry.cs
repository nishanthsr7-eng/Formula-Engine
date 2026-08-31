namespace FormulaEngine.Core.Functions;

/// <summary>Holds the set of functions callable from formulas. New functions are added by
/// registering an <see cref="IFunction"/> — the parser and evaluator never change.</summary>
public sealed class FunctionRegistry
{
    private readonly Dictionary<string, IFunction> _functions = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>All currently registered functions, e.g. for building a reference/help panel.</summary>
    public IReadOnlyCollection<IFunction> All => _functions.Values;

    public void Register(IFunction function) => _functions[function.Name] = function;

    public bool TryGet(string name, out IFunction function) => _functions.TryGetValue(name, out function!);

    /// <summary>A registry pre-populated with the engine's built-in functions.</summary>
    public static FunctionRegistry CreateDefault()
    {
        var registry = new FunctionRegistry();
        foreach (var fn in BuiltInFunctions.All)
            registry.Register(fn);
        return registry;
    }
}
