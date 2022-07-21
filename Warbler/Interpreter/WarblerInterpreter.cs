using Warbler.ErrorReporting;
using Warbler.Errors;
using Warbler.Expressions;
using Warbler.Utils;

namespace Warbler.Interpreter;

public class WarblerInterpreter : IExpressionVisitor<object?>
{
    private readonly HashSet<TokenKind> _numericOperators = new()
    {
        TokenKind.Hat, TokenKind.Minus, TokenKind.Plus, TokenKind.Asterisk, TokenKind.Slash, TokenKind.Modulo
    };

    private readonly HashSet<TokenKind> _relationalOperators = new()
    {
        TokenKind.LessEqual, TokenKind.LessThan, TokenKind.GreaterEqual, TokenKind.GreaterThan, TokenKind.DoubleEqual,
        TokenKind.NotEqual
    };

    private readonly IErrorReporter _errorReporter;

    public WarblerInterpreter(IErrorReporter errorReporter)
    {
        _errorReporter = errorReporter;
    }

    public void Interpret(Expression expression)
    {
        Console.WriteLine(Evaluate(expression));
    }


    private object? Evaluate(Expression expression)
    {
        return expression.Accept(this);
    }

    public object? VisitUnaryExpression(UnaryExpression expression)
    {
        var expr = Evaluate(expression.Expression);
        if (expr is null)
        {
            throw new ArgumentNullException();
        }

        switch (expression.Op.Kind)
        {
            case TokenKind.Minus:
                if (expr is int intExpr)
                    return -1 * intExpr;
                return -1 * (double)expr;
            case TokenKind.Not:
                return !(bool)expr;
        }

        // unreachable
        return null;
    }

    public object? VisitBinaryExpression(BinaryExpression expression)
    {
        var left = Evaluate(expression.Left);
        var right = Evaluate(expression.Right);
        if (left is null || right is null)
        {
            throw new ArgumentNullException();
        }

        var opKind = expression.Op.Kind;
        if (opKind == TokenKind.DoublePlus)
            return (string)left + (string)right;

        if (_numericOperators.Contains(opKind))
        {
            if (left is double dLeft)
            {
                var dRight = (double)right;
                try
                {
                    return EvaluateNumeric(opKind, dLeft, dRight, new DoubleMathProvider());
                }
                catch (DivideByZeroException)
                {
                    throw HandleRuntimeError(expression, "runtime.divideByZero");
                }
            }

            var iLeft = (int)left;
            var iRight = (int)right;
            try
            {
                return EvaluateNumeric(opKind, iLeft, iRight, new IntMathProvider());
            }
            catch (DivideByZeroException)
            {
                throw HandleRuntimeError(expression, "runtime.divideByZero");
            }
        }

        if (_relationalOperators.Contains(opKind))
        {
            var comparableLeft = (IComparable)left;
            var comparableRight = (IComparable)right;
            var comparisonResult = comparableLeft.CompareTo(comparableRight);
            return opKind switch
            {
                TokenKind.GreaterThan => comparisonResult > 0,
                TokenKind.LessThan => comparisonResult < 0,
                TokenKind.GreaterEqual => comparisonResult >= 0,
                TokenKind.LessEqual => comparisonResult <= 0,
                TokenKind.NotEqual => comparisonResult != 0,
                TokenKind.DoubleEqual => comparisonResult == 0,
                _ => throw new ArgumentException("Unexpected operator")
            };
        }

        // unreachable
        return null;
    }

    private static object? EvaluateNumeric<T>(TokenKind opKind, T left, T right, MathProvider<T> mathProvider)
        where T : struct
    {
        return opKind switch
        {
            TokenKind.Hat => mathProvider.RaiseToPower(left, right),
            TokenKind.Asterisk => mathProvider.Multiply(left, right),
            TokenKind.Slash => mathProvider.Divide(left, right),
            TokenKind.Modulo => mathProvider.Modulo(left, right),
            TokenKind.Plus => mathProvider.Add(left, right),
            TokenKind.Minus => mathProvider.Subtract(left, right),
            _ => throw new ArgumentException("Unexpected operator")
        };
    }

    public object VisitTernaryExpression(TernaryExpression expression)
    {
        var condition = Evaluate(expression.Condition);
        if (condition is not bool boolCondition)
            throw new ArgumentException();

        var result = boolCondition ? Evaluate(expression.ThenBranch) : Evaluate(expression.ElseBranch);
        if (result is null)
            throw new ArgumentNullException();

        return result;
    }

    public object? VisitLiteralExpression(LiteralExpression expression)
    {
        // an int expression might have had its type set to Double 
        // as a result of coercion
        // todo maybe this should be handled at type-checking stage
        if (expression.Type == ExpressionType.Double)
        {
            return (double)expression.Value;
        }

        return expression.Value;
    }

    private RuntimeError HandleRuntimeError(Expression expression, string message)
    {
        _errorReporter.ErrorAtExpression(expression, message);
        return new RuntimeError();
    }
}