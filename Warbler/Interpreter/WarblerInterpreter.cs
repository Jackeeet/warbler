using Warbler.Environment;
using Warbler.ErrorReporting;
using Warbler.Errors;
using Warbler.Expressions;
using Warbler.Utils;
using Warbler.Resources.Errors;

namespace Warbler.Interpreter;

public class WarblerInterpreter : IExpressionVisitor<object?>
{
    private static readonly HashSet<TokenKind> numericOperators = new()
    {
        TokenKind.Hat, TokenKind.Minus, TokenKind.Plus, TokenKind.Asterisk, TokenKind.Slash, TokenKind.Modulo
    };

    private static readonly HashSet<TokenKind> relationalOperators = new()
    {
        TokenKind.LessEqual, TokenKind.LessThan, TokenKind.GreaterEqual, TokenKind.GreaterThan, TokenKind.DoubleEqual,
        TokenKind.NotEqual
    };

    private readonly IErrorReporter _errorReporter;
    private WarblerEnvironment _environment;

    public WarblerInterpreter(IErrorReporter errorReporter, WarblerEnvironment environment)
    {
        _errorReporter = errorReporter;
        _environment = environment;
    }

    public void Interpret(Expression expression)
    {
        try
        {
            Console.WriteLine(Evaluate(expression));
        }
        catch (RuntimeError error)
        {
            _errorReporter.ReportRuntimeError(error);
        }
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
        {
            return EvaluateStringBinary(left, right);
        }

        if (numericOperators.Contains(opKind))
        {
            try
            {
                return EvaluateNumericBinary(left, right, opKind);
            }
            catch (DivideByZeroException)
            {
                throw HandleRuntimeError(expression, Runtime.DivideByZero);
            }
        }

        if (relationalOperators.Contains(opKind))
        {
            return EvaluateRelationalBinary(left, right, opKind);
        }

        // unreachable
        return null;
    }

    private static object? EvaluateRelationalBinary(object left, object right, TokenKind opKind)
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

    private static object EvaluateStringBinary(object left, object right)
    {
        return (string)left + (string)right;
    }

    private object EvaluateNumericBinary(object left, object right, TokenKind opKind)
    {
        if (left is double dLeft)
            return EvaluateNumeric(opKind, dLeft, (double)right, new DoubleMathProvider());

        return EvaluateNumeric(opKind, (int)left, (int)right, new IntMathProvider());
    }

    private static object EvaluateNumeric<T>(TokenKind opKind, T left, T right, MathProvider<T> mathProvider)
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

    public object VisitLiteralExpression(LiteralExpression expression)
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

    public object VisitVariableDeclarationExpression(VariableDeclarationExpression expression)
    {
        if (!_environment.Defined(expression.Name.Lexeme))
            throw new Exception(
                $"Variable name {expression.Name.Lexeme} should be defined before the interpreting stage");

        var initializerValue = Evaluate(expression.Initializer);
        if (initializerValue is null)
            throw new ArgumentNullException();

        _environment.Define(expression.Name.Lexeme, expression.Initializer.Type, initializerValue);
        return initializerValue;
    }

    public object VisitVariableExpression(VariableExpression expression)
    {
        if (!_environment.DefinedValue(expression.Name.Lexeme))
            throw new ArgumentException();

        var stored = _environment.Get(expression.Name);
        var storedType = stored.Item1;
        var storedValue = stored.Item2!;

        return storedType switch
        {
            ExpressionType.Integer => (int)storedValue,
            ExpressionType.Double => (double)storedValue,
            ExpressionType.Boolean => (bool)storedValue,
            ExpressionType.Char => (char)storedValue,
            ExpressionType.String => (string)storedValue,
            _ => throw new ArgumentException()
        };
    }

    public object? VisitAssignmentExpression(AssignmentExpression expression)
    {
        if (!_environment.Defined(expression.Name.Lexeme))
            throw new ArgumentException();

        var value = Evaluate(expression.Value);
        _environment.Assign(expression.Name, value);
        return value;
    }

    private RuntimeError HandleRuntimeError(Expression expression, string message)
    {
        _errorReporter.ErrorAtExpression(expression, message);
        return new RuntimeError();
    }
}