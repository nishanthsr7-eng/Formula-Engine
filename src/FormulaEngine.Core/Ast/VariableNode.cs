namespace FormulaEngine.Core.Ast;

public sealed class VariableNode : AstNode
{
    public string Name { get; }

    public VariableNode(string name)
    {
        Name = name;
    }

    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitVariable(this);
}
