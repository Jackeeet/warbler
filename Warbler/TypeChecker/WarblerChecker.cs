using Warbler.ErrorReporting;
using Warbler.Expressions;

namespace Warbler.TypeChecker;

public class WarblerChecker : IExpressionVisitor<object?>
{
    private readonly IErrorReporter _errorReporter;

    public WarblerChecker(IErrorReporter errorReporter)
    {
        _errorReporter = errorReporter;
    }

    public bool CheckTypes(Expression expression)
    {
        try
        {
            TypeExpession(expression);
            return true;
        }
        catch (TypeError te)
        {
            return false;
        }
        catch (SyntaxError se)
        {
            return false;
        }
    }

    private void TypeExpession(Expression expression)
    {
        expression.Accept(this);
    }

    private bool CheckNumericOperands(Expression left, Expression right)
    {
        if (!(left.Type == ExpressionType.Double || left.Type == ExpressionType.Integer))
            return false;
        if (!(right.Type == ExpressionType.Double || right.Type == ExpressionType.Integer))
            return false;

        return true;
    }

    private ExpressionType CoerceNumeric(Expression left, Expression right)
    {
        if (left.Type == ExpressionType.Double)
        {
            right.Type = ExpressionType.Double;
            return ExpressionType.Double;
        }

        if (right.Type == ExpressionType.Double)
        {
            left.Type = ExpressionType.Double;
            return ExpressionType.Double;
        }

        return ExpressionType.Integer;
    }

    private TypeError HandleTypeError(Expression expression, string message)
    {
        _errorReporter.ErrorAtExpression(expression, message);
        return new TypeError();
    }

    private SyntaxError HandleSyntaxError(Expression expression, string message)
    {
        _errorReporter.ErrorAtExpression(expression, message);
        return new SyntaxError();
    }

    public object? VisitUnaryExpression(UnaryExpression expression)
    {
        TypeExpession(expression.Expression);
        var innerType = expression.Expression.Type;
        switch (expression.Op.Kind)
        {
            case TokenKind.Not:
                if (innerType != ExpressionType.Boolean)
                {
                    throw HandleTypeError(expression, "The '!' operator can only be used with Boolean expressions");
                }

                break;
            case TokenKind.Minus:
                if (!(innerType == ExpressionType.Double || innerType == ExpressionType.Integer))
                {
                    throw HandleTypeError(expression,
                        "The '-' operator can only be used with Double or Integer expressions");
                }

                break;
            default:
                // unreachable
                throw new ArgumentException($"Unexpected operator {expression.Op.Lexeme}");
        }

        expression.Type = innerType;
        return null;
    }

    public object? VisitBinaryExpression(BinaryExpression expression)
    {
        var left = expression.Left;
        TypeExpession(left);
        var right = expression.Right;
        TypeExpession(right);

        switch (expression.Op.Kind)
        {
            case TokenKind.Hat:
            case TokenKind.Asterisk:
            case TokenKind.Plus:
            case TokenKind.Minus:
                if (!CheckNumericOperands(left, right))
                {
                    throw HandleTypeError(expression, "Both operands must be either Integer or Double");
                }

                expression.Type = CoerceNumeric(left, right);
                break;
            case TokenKind.Slash:
            case TokenKind.Modulo:
                if (!CheckNumericOperands(left, right))
                {
                    throw HandleTypeError(expression, "Both operands must be either Integer or Double");
                }

                // at this point only literal expressions have actual values
                // so it seems like checking division by 0 is possible here
                // but maybe it should be moved to the interpreter/compilation stage
                if (right is LiteralExpression literal)
                {
                    if (literal.Type == ExpressionType.Double && (double)literal.Value == 0 ||
                        literal.Type == ExpressionType.Integer && (int)literal.Value == 0)
                    {
                        throw HandleSyntaxError(expression, "Division by zero");
                    }
                }

                expression.Type = CoerceNumeric(left, right);
                break;
            case TokenKind.DoublePlus:
                if (left.Type != ExpressionType.String || right.Type != ExpressionType.String)
                    throw HandleTypeError(expression,
                        "The '++' operator can only be used with expressions of type String");
                expression.Type = ExpressionType.String;
                break;
            case TokenKind.GreaterThan:
            case TokenKind.LessThan:
            case TokenKind.GreaterEqual:
            case TokenKind.LessEqual:
            case TokenKind.NotEqual:
            case TokenKind.DoubleEqual:
                var numeric = CheckNumericOperands(left, right);
                if (!(left.Type == right.Type || numeric))
                {
                    throw HandleTypeError(expression,
                        $"Cannot compare expressions of types {left.Type} and {right.Type}");
                }

                if (numeric)
                {
                    CoerceNumeric(left, right);
                }

                expression.Type = ExpressionType.Boolean;
                break;
            default:
                // unreachable
                throw new ArgumentException($"Unexpected operator {expression.Op.Lexeme}");
        }

        return null;
    }

    public object? VisitTernaryExpression(TernaryExpression expression)
    {
        TypeExpession(expression.Condition);
        if (expression.Condition.Type != ExpressionType.Boolean)
        {
            throw HandleTypeError(expression, "A ternary condition must be Boolean");
        }

        var thenBranch = expression.ThenBranch;
        var elseBranch = expression.ElseBranch;
        TypeExpession(thenBranch);
        TypeExpession(elseBranch);
        var numeric = CheckNumericOperands(thenBranch, elseBranch);

        if (!(thenBranch.Type == elseBranch.Type || numeric))
        {
            throw HandleTypeError(expression, "Ternary branches must have the same type");
        }

        if (numeric)
        {
            CoerceNumeric(thenBranch, elseBranch);
        }

        expression.Type = thenBranch.Type;
        return null;
    }

    public object? VisitLiteralExpression(LiteralExpression expression)
    {
        if (expression.Type == ExpressionType.Undefined)
            throw HandleTypeError(expression, "Undefined literal type");

        return null;
    }
}