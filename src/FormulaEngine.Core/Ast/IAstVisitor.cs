namespace FormulaEngine.Core.Ast;

/// <summary>One method per <see cref="AstNode"/> subtype. <see cref="FormulaEngine.Core.Evaluation.Evaluator"/>
/// implements this to compute values; the Studio's AstTreeBuilder implements it to render
/// the same tree for display — one dispatch mechanism, independent consumers.</summary>
public interface IAstVisitor<T>
{
    T VisitNumber(NumberNode node);
    T VisitVariable(VariableNode node);
    T VisitUnaryOp(UnaryOpNode node);
    T VisitBinaryOp(BinaryOpNode node);
    T VisitFunctionCall(FunctionCallNode node);
}
