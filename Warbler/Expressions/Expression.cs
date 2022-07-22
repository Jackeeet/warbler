namespace Warbler.Expressions;

public abstract class Expression
{
	public ExpressionType Type { get; set; }

	public int Line { get; init; }

	public abstract T Accept<T>(IExpressionVisitor<T> visitor);
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

	protected bool Equals(UnaryExpression other)
	{
		return Type.Equals(other.Type) && Line.Equals(other.Line) && Op.Equals(other.Op) && Expression.Equals(other.Expression);
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

	protected bool Equals(BinaryExpression other)
	{
		return Type.Equals(other.Type) && Line.Equals(other.Line) && Left.Equals(other.Left) && Op.Equals(other.Op) && Right.Equals(other.Right);
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

	protected bool Equals(TernaryExpression other)
	{
		return Type.Equals(other.Type) && Line.Equals(other.Line) && Condition.Equals(other.Condition) && ThenBranch.Equals(other.ThenBranch) && ElseBranch.Equals(other.ElseBranch);
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

	protected bool Equals(VariableDeclarationExpression other)
	{
		return Type.Equals(other.Type) && Line.Equals(other.Line) && VarType.Equals(other.VarType) && Name.Equals(other.Name) && Initializer.Equals(other.Initializer);
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

	protected bool Equals(AssignmentExpression other)
	{
		return Type.Equals(other.Type) && Line.Equals(other.Line) && Name.Equals(other.Name) && Value.Equals(other.Value);
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


