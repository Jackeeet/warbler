using Warbler.Scanner;
namespace Warbler.Parser;

public interface IExpressionVisitor<out T>
{
	T VisitUnaryExpression(UnaryExpression expression);
	T VisitBinaryExpression(BinaryExpression expression);
	T VisitTernaryExpression(TernaryExpression expression);
	T VisitLiteralExpression(LiteralExpression expression);
	T VisitGroupingExpression(GroupingExpression expression);
}

public abstract class Expression
{
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
}

public class LiteralExpression : Expression
{
	public readonly object? Value;

	public LiteralExpression(object? value)
	{
		Value = value;
	}

	public override T Accept<T>(IExpressionVisitor<T> visitor)
	{
		return visitor.VisitLiteralExpression(this);
	}
}

public class GroupingExpression : Expression
{
	public readonly Expression Expression;

	public GroupingExpression(Expression expression)
	{
		Expression = expression;
	}

	public override T Accept<T>(IExpressionVisitor<T> visitor)
	{
		return visitor.VisitGroupingExpression(this);
	}
}


