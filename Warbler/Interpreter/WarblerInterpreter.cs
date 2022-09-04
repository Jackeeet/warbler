using System.Diagnostics;
using Warbler.Environment;
using Warbler.ErrorReporting;
using Warbler.Errors;
using Warbler.Expressions;
using Warbler.Resources.Errors;
using Warbler.Utils.Exceptions;
using Warbler.Utils.General;
using Warbler.Utils.Token;
using Warbler.Utils.Type;

namespace Warbler.Interpreter;

public class WarblerInterpreter : IExpressionVisitor<object?>
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
    public readonly WarblerEnvironment GlobalEnvironment;
    private WarblerEnvironment _environment;
    private readonly Dictionary<Expression, int?> _locals;

    public WarblerInterpreter(IErrorReporter errorReporter, WarblerEnvironment globalEnvironment,
        Dictionary<Expression, int?> locals)
    {
        GlobalEnvironment = globalEnvironment;
        _locals = locals;
        _errorReporter = errorReporter;
        _environment = GlobalEnvironment;
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

    private object? Evaluate(Expression expression)
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
            case TokenKind.Minus when expr is int intExpr:
                return -1 * intExpr;
            case TokenKind.Minus when expr is double doubleExpr:
                return -1 * doubleExpr;
            case TokenKind.Not when expr is bool boolExpr:
                return !boolExpr;
            case TokenKind.Minus:
            case TokenKind.Not:
                throw new ArgumentException();
        }

        throw new UnreachableException();
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

        throw new UnreachableException("Unexpected operator");
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

    private static object EvaluateNumericBinary(object left, object right, TokenKind opKind)
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
        if (expression.Value is int intValue && expression.Type.BaseType == ExpressionType.Double)
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
        var (storedType, storedValue) = LookupVariable(expression.Name, expression);
        Debug.Assert(storedValue != null, nameof(storedValue) + " != null");

        return storedType.BaseType switch
        {
            ExpressionType.Integer => (int)storedValue,
            ExpressionType.Double => (double)storedValue,
            ExpressionType.Boolean => (bool)storedValue,
            ExpressionType.Char => (char)storedValue,
            ExpressionType.String => (string)storedValue,
            ExpressionType.Function => (WarblerFunction)storedValue,
            _ => throw new ArgumentException()
        };
    }

    private Tuple<WarblerType, object?> LookupVariable(Token name, Expression expression)
    {
        var level = _locals[expression];
        var variable = level is not null
            ? _environment.GetAt(level.Value, name.Lexeme)
            : GlobalEnvironment.GetAssigned(name);
        return variable;
    }

    public object VisitAssignmentExpression(AssignmentExpression expression)
    {
        if (!_environment.Defined(expression.Name.Lexeme))
            throw new ArgumentException();

        var value = Evaluate(expression.Value);
        if (value is null)
            throw new ArgumentException();

        var level = _locals[expression];
        if (level is null)
        {
            GlobalEnvironment.Assign(expression.Name, value);
        }
        else
        {
            _environment.AssignAt(level.Value, expression.Name, value);
        }

        _environment.Assign(expression.Name, value);
        return value;
    }

    public object? InterpretBlock(BlockExpression block, WarblerEnvironment environment)
    {
        var previousEnvironment = _environment;
        try
        {
            _environment = environment;
            var expressions = block.Expressions;
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
            // todo this should catch a custom "NoBlockException" or something like that
            // because the message is block-specific and because right now it catches
            // exceptions not related to blocks as well
            throw new ArgumentException("Expected a block to be declared at type-checking stage", ex);
        }
        finally
        {
            _environment = previousEnvironment;
        }
    }

    public object? VisitBlockExpression(BlockExpression expression)
    {
        // if a block is in a function, it does not get defined until the interpreting stage
        var blockEnvironment = _environment.HasSubEnvironment(expression.EnvironmentId)
            ? _environment.GetSubEnvironment(expression.EnvironmentId)
            : _environment.AddSubEnvironment(expression.EnvironmentId);

        return InterpretBlock(expression, blockEnvironment);
    }

    public object? VisitConditionalExpression(ConditionalExpression expression)
    {
        if (Evaluate(expression.Condition) is not bool boolCondition)
            throw new ArgumentException();

        // condition is true -> "then" branch gets evaluated
        if (boolCondition)
            return EvaluateBlock(expression.ThenBranch);
        // return Evaluate(expression.ThenBranch);

        // condition is false -> "else" branch exists and gets evaluated
        if (expression.ElseBranch is not null)
            return EvaluateBlock(expression.ElseBranch);
        // return Evaluate(expression.ElseBranch);

        // condition is false and there is no else branch
        return null;
    }

    public object VisitWhileLoopExpression(WhileLoopExpression expression)
    {
        if (Evaluate(expression.Condition) is not bool)
            throw new ArgumentException();

        var loopCount = 0;

        Debug.Assert(expression.Condition != null, "expression.Condition != null");

        // expression.Condition has to be explicitly reevaluated on every iteration,
        // otherwise the loop condition will be a constant value
        // it is always a basic expression (never null) so evaluated condition is never null as well
        while ((bool)Evaluate(expression.Condition)!)
        {
            _ = EvaluateBlock(expression.Actions);
            loopCount++;
        }

        return loopCount;
    }

    private object EvaluateBlock(Expression expression)
    {
        if (expression is BlockExpression blockExpression)
        {
            for (var i = 0; i < blockExpression.Expressions.Count - 1; i++)
            {
                var expr = blockExpression.Expressions[i];
                Debug.Assert(expr != null, nameof(expr) + " != null");
                Evaluate(expr);
            }

            return Evaluate(blockExpression.Expressions[^1]!) ?? throw new ArgumentException();
        }

        return Evaluate(expression) ?? throw new ArgumentException();
    }

    public object VisitFunctionDeclarationExpression(FunctionDeclarationExpression expression)
    {
        var func = new WarblerFunction(expression);
        _environment.Define(expression.Name.Lexeme, expression.Type, func); // todo this should be Assign
        return func;
    }

    public object VisitCallExpression(CallExpression expression)
    {
        var called = Evaluate(expression.Called);

        // todo maybe this error should be shown to the user
        if (called is not ICallable callable)
            throw new ArgumentException();

        var arguments = new List<object>();
        foreach (var arg in expression.Args)
            arguments.Add(Evaluate(arg) ?? throw new ArgumentException());

        return callable.Call(this, _environment, arguments);
    }

    private RuntimeError HandleRuntimeError(Expression expression, string message)
    {
        _errorReporter.ErrorAtExpression(expression, message);
        return new RuntimeError();
    }
}