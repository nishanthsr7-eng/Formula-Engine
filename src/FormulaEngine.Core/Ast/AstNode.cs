namespace FormulaEngine.Core.Ast;

/// <summary>Base of every parsed expression node. Dispatch happens through
/// <see cref="IAstVisitor{T}"/> (the interpreter/visitor pattern) so new consumers —
/// evaluation, a tree view, a formatter — can be added without touching the node types.</summary>
public abstract class AstNode
{
    public abstract T Accept<T>(IAstVisitor<T> visitor);
}
