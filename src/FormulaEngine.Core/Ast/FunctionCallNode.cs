namespace FormulaEngine.Core.Ast;

public sealed class FunctionCallNode : AstNode
{
    public string Name { get; }
    public IReadOnlyList<AstNode> Arguments { get; }

    public FunctionCallNode(string name, IReadOnlyList<AstNode> arguments)
    {
        Name = name;
        Arguments = arguments;
    }

    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitFunctionCall(this);
}
