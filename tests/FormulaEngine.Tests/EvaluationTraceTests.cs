using FormulaEngine.Core;
using FormulaEngine.Core.Ast;
using Xunit;

namespace FormulaEngine.Tests;

public class EvaluationTraceTests
{
    [Fact]
    public void EvaluateWithTrace_SimpleAddition_RecordsStepsInEvaluationOrder()
    {
        var (result, _, steps) = Formula.EvaluateWithTrace("1 + 2");

        Assert.Equal(3, result);
        Assert.Equal(3, steps.Count);
        Assert.IsType<NumberNode>(steps[0].Node);
        Assert.Equal(1, steps[0].Value);
        Assert.IsType<NumberNode>(steps[1].Node);
        Assert.Equal(2, steps[1].Value);
        Assert.IsType<BinaryOpNode>(steps[2].Node);
        Assert.Equal(3, steps[2].Value);
    }

    [Fact]
    public void EvaluateWithTrace_FunctionCall_RecordsArgumentsBeforeCall()
    {
        var (result, _, steps) = Formula.EvaluateWithTrace("SUM(1, 2)");

        Assert.Equal(3, result);
        Assert.Equal(3, steps.Count);
        Assert.IsType<FunctionCallNode>(steps[^1].Node);
        Assert.Equal(3, steps[^1].Value);
    }

    [Fact]
    public void EvaluateWithTrace_LastStepValue_MatchesFinalResult()
    {
        var (result, _, steps) = Formula.EvaluateWithTrace("IF(2 > 1, 10, 20)");

        Assert.Equal(result, steps[^1].Value);
    }
}
