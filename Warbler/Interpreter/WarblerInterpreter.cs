using System.Diagnostics;
using Warbler.Environment;
using Warbler.ErrorReporting;
using Warbler.Errors;
using Warbler.Expressions;
using Warbler.Utils;
using Warbler.Resources.Errors;

namespace Warbler.Interpreter;

public class WarblerInterpreter : IExpressionVisitor<object>
{
    private static readonly HashSet<TokenKind> numericOperators = new()
    {
        TokenKind.Hat, TokenKind.Minus, TokenKind.Plus, TokenKind.Asterisk, TokenKind.Slash, TokenKind.Percent
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

    public object? Interpret(Expression expression)
    {
        try
        {
            return Evaluate(expression);
        }
        catch (RuntimeError error)
        {
            _errorReporter.ReportRuntimeError(error);
            return null;
        }
    }

    private object Evaluate(Expression expression)
    {
        Debug.Assert(expression != null, nameof(expression) + " != null");
        return expression.Accept(this);
    }

    public object VisitUnaryExpression(UnaryExpression expression)
    {
        if (expression.Expression is null)
            throw new ArgumentException();

        var expr = Evaluate(expression.Expression);
        switch (expression.Op.Kind)
        {
            case TokenKind.Minus:
                if (expr is int intExpr)
                    return -1 * intExpr;
                if (expr is double doubleExpr)
                    return -1 * doubleExpr;
                throw new ArgumentException();
            case TokenKind.Not:
                if (expr is not bool boolExpr)
                    throw new ArgumentException();
                return !boolExpr;
        }

        // unreachable
        throw new ArgumentException();
    }

    public object VisitBinaryExpression(BinaryExpression expression)
    {
        if (expression.Left is null || expression.Right is null)
            throw new ArgumentException();

        var left = Evaluate(expression.Left);
        var right = Evaluate(expression.Right);
        Debug.Assert(left != null, nameof(left) + " != null");
        Debug.Assert(right != null, nameof(right) + " != null");

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
        throw new ArgumentException("Unexpected operator");
    }

    private static object EvaluateRelationalBinary(object left, object right, TokenKind opKind)
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
            TokenKind.Percent => mathProvider.Modulo(left, right),
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
            throw new ArgumentException();

        return result;
    }

    public object VisitLiteralExpression(LiteralExpression expression)
    {
        // an int expression might have had its type set to Double as a result of coercion
        if (expression.Value is int intValue && expression.Type == ExpressionType.Double)
            return Convert.ToDouble(intValue);

        return expression.Value;
    }

    public object VisitVariableDeclarationExpression(VariableDeclarationExpression expression)
    {
        if (!_environment.Defined(expression.Name.Lexeme))
            throw new ArgumentException(
                $"Variable name {expression.Name.Lexeme} should be defined before the interpreting stage");

        var initializerValue = Evaluate(expression.Initializer);
        if (initializerValue is null)
            throw new ArgumentException();

        _environment.Define(expression.Name.Lexeme, expression.Initializer.Type, initializerValue);
        return initializerValue;
    }

    public object VisitVariableExpression(VariableExpression expression)
    {
        if (!_environment.Assigned(expression.Name.Lexeme))
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

    public object VisitAssignmentExpression(AssignmentExpression expression)
    {
        if (!_environment.Defined(expression.Name.Lexeme))
            throw new ArgumentException();

        var value = Evaluate(expression.Value);
        if (value is null)
            throw new ArgumentException();

        _environment.Assign(expression.Name, value);
        return value;
    }

    public object VisitBlockExpression(BlockExpression expression)
    {
        var previousEnvironment = _environment;
        try
        {
            _environment = _environment.GetSubEnvironment(expression.BlockId.Value);
            var expressions = expression.Expressions;
            Debug.Assert(expressions != null, nameof(expressions) + " != null");

            for (var i = 0; i < expressions.Count - 1; i++)
            {
                var expr = expressions[i];
                Debug.Assert(expr != null, nameof(expr) + " != null");
                _ = Evaluate(expr);
            }

            return Evaluate(expressions[^1]!);
        }
        catch (ArgumentException ex)
        {
            // this should catch a custom "NoBlockException" or something like that
            // because the message is block-specific and because right now it catches
            // exceptions not related to blocks as well
            throw new ArgumentException("Expected a block to be declared at type-checking stage", ex);
        }
        finally
        {
            _environment = previousEnvironment;
        }
    }

    public object VisitConditionalExpression(ConditionalExpression expression)
    {
        if (Evaluate(expression.Condition) is not bool boolCondition)
            throw new ArgumentException();

        if (boolCondition)
        {
            Evaluate(expression.ThenBranch);
        }
        else if (expression.ElseBranch is not null)
        {
            Evaluate(expression.ElseBranch);
        }

        return boolCondition;
    }

    public object VisitWhileLoopExpression(WhileLoopExpression expression)
    {
        if (Evaluate(expression.Condition) is not bool)
            throw new ArgumentException();

        var loopCount = 0;
        // this has to be explicitly reevaluated on every iteration
        // otherwise the loop condition will be a constant value
        Debug.Assert(expression.Condition != null, "expression.Condition != null");
        while ((bool)Evaluate(expression.Condition))
        {
            Evaluate(expression.Actions);
            loopCount++;
        }

        return loopCount;
    }

    private RuntimeError HandleRuntimeError(Expression expression, string message)
    {
        _errorReporter.ErrorAtExpression(expression, message);
        return new RuntimeError();
    }
}