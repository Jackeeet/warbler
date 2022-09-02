using System.Diagnostics;
using System.Text;
using Warbler.Utils.General;
using Warbler.Utils.Id;
using Warbler.Utils.Token;
using Warbler.Utils.Type;

namespace Warbler.Expressions;

public abstract class Expression : IRepresentable
{
    public WarblerType Type { get; set; }

    public int Line { get; init; }

    public abstract T Accept<T>(IExpressionVisitor<T> visitor);

    public override string ToString() => $"Type: {Type}, Line: {Line}";

    public abstract string DefaultRepresentation();
}

public class UnaryExpression : Expression
{
    public readonly Token Op;
    public readonly Expression Expression;

    public UnaryExpression(Token op, Expression expression)
    {
        Op = op;
        Expression = expression;
    }

    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.VisitUnaryExpression(this);
    }

    public override string DefaultRepresentation()
    {
        return "new UnaryExpression(\n" +
               $"    {Op.DefaultRepresentation()},\n" +
               $"    {Expression.DefaultRepresentation()}\n" +
               $") {{ Type = new {Type}, Line = 1 }}";
    }

    protected bool Equals(UnaryExpression other)
    {
        return Type.Equals(other.Type) && Line.Equals(other.Line) && Op.Equals(other.Op) &&
               Expression.Equals(other.Expression);
    }

    public override bool Equals(object? obj)
    {
        return obj is UnaryExpression other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Op, Expression);
    }
}

public class BinaryExpression : Expression
{
    public readonly Expression Left;
    public readonly Token Op;
    public readonly Expression Right;

    public BinaryExpression(Expression left, Token op, Expression right)
    {
        Left = left;
        Op = op;
        Right = right;
    }

    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.VisitBinaryExpression(this);
    }

    public override string DefaultRepresentation()
    {
        return "new BinaryExpression(\n" +
               $"    {Left.DefaultRepresentation()},\n" +
               $"    {Op.DefaultRepresentation()},\n" +
               $"    {Right.DefaultRepresentation()}\n" +
               $") {{ Type = new {Type}, Line = 1 }}";
    }

    protected bool Equals(BinaryExpression other)
    {
        return Type.Equals(other.Type) && Line.Equals(other.Line) && Left.Equals(other.Left) && Op.Equals(other.Op) &&
               Right.Equals(other.Right);
    }

    public override bool Equals(object? obj)
    {
        return obj is BinaryExpression other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Left, Op, Right);
    }
}

public class TernaryExpression : Expression
{
    public readonly Expression Condition;
    public readonly Expression ThenBranch;
    public readonly Expression ElseBranch;

    public TernaryExpression(Expression condition, Expression thenbranch, Expression elsebranch)
    {
        Condition = condition;
        ThenBranch = thenbranch;
        ElseBranch = elsebranch;
    }

    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.VisitTernaryExpression(this);
    }

    public override string DefaultRepresentation()
    {
        return "new TernaryExpression(\n" +
               $"    {Condition.DefaultRepresentation()},\n" +
               $"    {ThenBranch.DefaultRepresentation()},\n" +
               $"    {ElseBranch.DefaultRepresentation()}\n" +
               $") {{ Type = new {Type}, Line = 1 }}";
    }

    protected bool Equals(TernaryExpression other)
    {
        return Type.Equals(other.Type) && Line.Equals(other.Line) && Condition.Equals(other.Condition) &&
               ThenBranch.Equals(other.ThenBranch) && ElseBranch.Equals(other.ElseBranch);
    }

    public override bool Equals(object? obj)
    {
        return obj is TernaryExpression other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Condition, ThenBranch, ElseBranch);
    }
}

public class LiteralExpression : Expression
{
    public readonly object Value;

    public LiteralExpression(object value)
    {
        Value = value;
    }

    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.VisitLiteralExpression(this);
    }

    public override string DefaultRepresentation()
    {
        var constr = "new LiteralExpression(";
        var type = ") {{ Type = new {Type}, Line = 1 }}";
        if (Value is string)
            return constr + $"\"{Value}\"" + type;

        if (Value is char)
            return constr + $"\'{Value}\'" + type;

        return constr + $"{Value}" + type;
    }

    protected bool Equals(LiteralExpression other)
    {
        return Type.Equals(other.Type) && Line.Equals(other.Line) && Value.Equals(other.Value);
    }

    public override bool Equals(object? obj)
    {
        return obj is LiteralExpression other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

public class VariableDeclarationExpression : Expression
{
    public readonly Token VarType;
    public readonly Token Name;
    public readonly Expression Initializer;

    public VariableDeclarationExpression(Token vartype, Token name, Expression initializer)
    {
        VarType = vartype;
        Name = name;
        Initializer = initializer;
    }

    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.VisitVariableDeclarationExpression(this);
    }

    public override string DefaultRepresentation()
    {
        return "new VariableDeclarationExpression(\n" +
               $"    {VarType.DefaultRepresentation()},\n" +
               $"    {Name.DefaultRepresentation()},\n" +
               $"    {Initializer.DefaultRepresentation()}\n" +
               $") {{ Type = new {Type}, Line = 1 }}";
    }

    protected bool Equals(VariableDeclarationExpression other)
    {
        return Type.Equals(other.Type) && Line.Equals(other.Line) && VarType.Equals(other.VarType) &&
               Name.Equals(other.Name) && Initializer.Equals(other.Initializer);
    }

    public override bool Equals(object? obj)
    {
        return obj is VariableDeclarationExpression other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(VarType, Name, Initializer);
    }
}

public class VariableExpression : Expression
{
    public readonly Token Name;

    public VariableExpression(Token name)
    {
        Name = name;
    }

    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.VisitVariableExpression(this);
    }

    public override string DefaultRepresentation()
    {
        return "new VariableExpression(\n" +
               $"    {Name.DefaultRepresentation()}\n" +
               $") {{ Type = new {Type}, Line = 1 }}";
    }

    protected bool Equals(VariableExpression other)
    {
        return Type.Equals(other.Type) && Line.Equals(other.Line) && Name.Equals(other.Name);
    }

    public override bool Equals(object? obj)
    {
        return obj is VariableExpression other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}

public class AssignmentExpression : Expression
{
    public readonly Token Name;
    public readonly Expression Value;

    public AssignmentExpression(Token name, Expression value)
    {
        Name = name;
        Value = value;
    }

    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.VisitAssignmentExpression(this);
    }

    public override string DefaultRepresentation()
    {
        return "new VariableDeclarationExpression(\n" +
               $"    {Name.DefaultRepresentation()},\n" +
               $"    {Value.DefaultRepresentation()}\n" +
               $") {{ Type = new {Type}, Line = 1 }}";
    }

    protected bool Equals(AssignmentExpression other)
    {
        return Type.Equals(other.Type) && Line.Equals(other.Line) && Name.Equals(other.Name) &&
               Value.Equals(other.Value);
    }

    public override bool Equals(object? obj)
    {
        return obj is AssignmentExpression other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Value);
    }
}

public class BlockExpression : Expression
{
    public readonly EnvId EnvironmentId;
    public readonly List<Expression?> Expressions;

    public BlockExpression(EnvId environmentid, List<Expression?> expressions)
    {
        EnvironmentId = environmentid;
        Expressions = expressions;
    }

    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.VisitBlockExpression(this);
    }

    public override string DefaultRepresentation()
    {
        var builder = new StringBuilder("new BlockExpression(\n" +
                                        "    new EnvId(new Guid()),\n" +
                                        "    new List<Expression?> {\n");
        foreach (var expr in Expressions)
        {
            Debug.Assert(expr != null, nameof(expr) + " != null");
            builder.Append(expr.DefaultRepresentation());
            builder.Append(",\n");
        }

        builder.Append("    }\n" +
                       $") {{ Type = new {Type}, Line = 1 }} ");

        return builder.ToString();
    }

    protected bool Equals(BlockExpression other)
    {
        return Type.Equals(other.Type) && Line.Equals(other.Line) && EnvironmentId.Equals(other.EnvironmentId) &&
               Expressions.AllEquals(other.Expressions);
    }

    public override bool Equals(object? obj)
    {
        return obj is BlockExpression other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(EnvironmentId, Expressions);
    }
}

public class ConditionalExpression : Expression
{
    public readonly Expression Condition;
    public readonly Expression ThenBranch;
    public readonly Expression? ElseBranch;

    public ConditionalExpression(Expression condition, Expression thenbranch, Expression? elsebranch)
    {
        Condition = condition;
        ThenBranch = thenbranch;
        ElseBranch = elsebranch;
    }

    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.VisitConditionalExpression(this);
    }

    public override string DefaultRepresentation()
    {
        var elseBranch = ElseBranch is null ? "null" : ElseBranch.DefaultRepresentation();
        return "new ConditionalExpression(\n" +
               $"    {Condition.DefaultRepresentation()},\n" +
               $"    {ThenBranch.DefaultRepresentation()},\n" +
               $"    {elseBranch}\n" +
               $") {{ Type = new {Type}, Line = 1 }}";
    }

    protected bool Equals(ConditionalExpression other)
    {
        return Type.Equals(other.Type) && Line.Equals(other.Line) && Condition.Equals(other.Condition) &&
               ThenBranch.Equals(other.ThenBranch) &&
               (ElseBranch is null && other.ElseBranch is null ||
                ElseBranch is not null && ElseBranch.Equals(other.ElseBranch));
    }

    public override bool Equals(object? obj)
    {
        return obj is ConditionalExpression other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Condition, ThenBranch, ElseBranch);
    }
}

public class WhileLoopExpression : Expression
{
    public readonly Expression Condition;
    public readonly Expression Actions;

    public WhileLoopExpression(Expression condition, Expression actions)
    {
        Condition = condition;
        Actions = actions;
    }

    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.VisitWhileLoopExpression(this);
    }

    public override string DefaultRepresentation()
    {
        return "new WhileLoopExpression(\n" +
               $"    {Condition.DefaultRepresentation()},\n" +
               $"    {Actions.DefaultRepresentation()}\n" +
               $") {{ Type = new {Type}, Line = 1 }}";
    }

    protected bool Equals(WhileLoopExpression other)
    {
        return Type.Equals(other.Type) && Line.Equals(other.Line) && Condition.Equals(other.Condition) &&
               Actions.Equals(other.Actions);
    }

    public override bool Equals(object? obj)
    {
        return obj is WhileLoopExpression other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Condition, Actions);
    }
}

public class FunctionDeclarationExpression : Expression
{
    public readonly EnvId EnvironmentId;
    public readonly Token Name;
    public readonly List<Tuple<TypeData, Token>> Parameters;
    public readonly TypeData ReturnType;
    public readonly BlockExpression Body;

    public FunctionDeclarationExpression(EnvId environmentid, Token name, List<Tuple<TypeData, Token>> parameters,
        TypeData returntype, BlockExpression body)
    {
        EnvironmentId = environmentid;
        Name = name;
        Parameters = parameters;
        ReturnType = returntype;
        Body = body;
    }

    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.VisitFunctionDeclarationExpression(this);
    }

    public override string DefaultRepresentation()
    {
        var builder = new StringBuilder("new FunctionDeclarationExpression(\n" +
                                        "    new EnvId(new Guid()),\n" +
                                        $"    {Name.DefaultRepresentation()},\n" +
                                        "    new List<Tuple<TypeData, Token>> {\n");
        foreach (var (type, token) in Parameters)
        {
            builder.Append("    Tuple.Create(\n" +
                           $"        new {type},\n" +
                           $"        {token.DefaultRepresentation()}\n" +
                           "    ), \n},");
        }

        builder.Append($"new {ReturnType},\n");
        builder.Append(Body.DefaultRepresentation());
        builder.Append($"\n) {{ Type = {Type}, Line = 1 }}");
        return builder.ToString();
    }

    protected bool Equals(FunctionDeclarationExpression other)
    {
        return Type.Equals(other.Type) && Line.Equals(other.Line) && EnvironmentId.Equals(other.EnvironmentId) &&
               Name.Equals(other.Name) &&
               Parameters.AllEquals(other.Parameters)
               && ReturnType.Equals(other.ReturnType) && Body.Equals(other.Body);
    }

    public override bool Equals(object? obj)
    {
        return obj is FunctionDeclarationExpression other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(EnvironmentId, Name, Parameters, ReturnType, Body);
    }
}

public class CallExpression : Expression
{
    public readonly Expression Called;
    public readonly List<Expression> Args;

    public CallExpression(Expression called, List<Expression> args)
    {
        Called = called;
        Args = args;
    }

    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.VisitCallExpression(this);
    }

    public override string DefaultRepresentation()
    {
        var builder = new StringBuilder("new CallExpression(\n" +
                                        $"    {Called.DefaultRepresentation()},\n" +
                                        "    new List<Expression> {\n");

        foreach (var arg in Args)
        {
            builder.Append($"        {arg.DefaultRepresentation()},\n");
        }

        builder.Append($"}}\n) {{ Type = {Type}, Line = 1 }} ");

        return builder.ToString();
    }

    protected bool Equals(CallExpression other)
    {
        return Type.Equals(other.Type) && Line.Equals(other.Line) && Called.Equals(other.Called) &&
               Args.AllEquals(other.Args);
    }

    public override bool Equals(object? obj)
    {
        return obj is CallExpression other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Called, Args);
    }
}