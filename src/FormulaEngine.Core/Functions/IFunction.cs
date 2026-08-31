namespace FormulaEngine.Core.Functions;

/// <summary>
/// Implement this to add a new callable function to the engine
/// without modifying the parser or evaluator.
/// </summary>
public interface IFunction
{
    /// <summary>Case-insensitive name used to call this function from a formula.</summary>
    string Name { get; }

    /// <summary>Fewest arguments this function accepts.</summary>
    int MinArgs { get; }

    /// <summary>Most arguments this function accepts (use <see cref="int.MaxValue"/> for unbounded).</summary>
    int MaxArgs { get; }

    /// <summary>Computes the result. <paramref name="args"/> is already validated against
    /// <see cref="MinArgs"/>/<see cref="MaxArgs"/> by the evaluator before this is called.</summary>
    double Invoke(IReadOnlyList<double> args);
}
