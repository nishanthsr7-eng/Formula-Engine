using FormulaEngine.Core;
using FormulaEngine.Core.Evaluation;
using FormulaEngine.Core.Functions;
using Xunit;

namespace FormulaEngine.Tests;

public class FunctionRegistryTests
{
    private sealed class DoubleFunction : IFunction
    {
        public string Name => "DOUBLEIT";
        public int MinArgs => 1;
        public int MaxArgs => 1;
        public double Invoke(IReadOnlyList<double> args) => args[0] * 2;
    }

    [Fact]
    public void CustomFunction_RegisteredWithoutModifyingCore_IsInvocable()
    {
        var registry = FunctionRegistry.CreateDefault();
        registry.Register(new DoubleFunction());

        var context = new EvaluationContext(new Dictionary<string, double>(), registry);

        Assert.Equal(10, Formula.Evaluate("DOUBLEIT(5)", context));
    }

    [Fact]
    public void CreateDefault_All_ListsEveryBuiltInFunction()
    {
        var registry = FunctionRegistry.CreateDefault();

        Assert.Contains(registry.All, f => f.Name == "SUM");
        Assert.Contains(registry.All, f => f.Name == "IF");
        Assert.Contains(registry.All, f => f.Name == "PI");
    }
}
