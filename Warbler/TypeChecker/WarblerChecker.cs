using System.Diagnostics;
using Warbler.Environment;
using Warbler.ErrorReporting;
using Warbler.Errors;
using Warbler.Expressions;
using Type = Warbler.Resources.Errors.Type;

namespace Warbler.TypeChecker;

public class WarblerChecker : IExpressionVisitor<object?>
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

    private static readonly Dictionary<ExpressionType, TokenKind> types = new()
    {
        { ExpressionType.Integer, TokenKind.Int },
        { ExpressionType.Double, TokenKind.Double },
        { ExpressionType.Boolean, TokenKind.Bool },
        { ExpressionType.Char, TokenKind.Char },
        { ExpressionType.String, TokenKind.String }
    };

    private readonly IErrorReporter _errorReporter;
    private WarblerEnvironment _environment;

    public WarblerChecker(IErrorReporter errorReporter, WarblerEnvironment environment)
    {
        _errorReporter = errorReporter;
        _environment = environment;
    }

    public bool CheckTypes(Expression expression)
    {
        try
        {
            TypeExpression(expression);
            return true;
        }
        catch (TypeError)
        {
            return false;
        }
        catch (RuntimeError error)
        {
            _errorReporter.ReportRuntimeError(error);
            return false;
        }
    }

    private void TypeExpression(Expression expression)
    {
        expression.Accept(this);
    }

    private static bool IsNumeric(ExpressionType innerType)
    {
        return innerType == ExpressionType.Double || innerType == ExpressionType.Integer;
    }

    private bool CheckNumericOperands(Expression left, Expression right)
    {
        return IsNumeric(left.Type) && IsNumeric(right.Type);
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

    public object? VisitUnaryExpression(UnaryExpression expression)
    {
        TypeExpression(expression.Expression);
        var innerType = expression.Expression.Type;
        TypeUnary(expression, innerType);
        return null;
    }

    private void TypeUnary(UnaryExpression expression, ExpressionType innerType)
    {
        if (expression.Op.Kind != TokenKind.Not && expression.Op.Kind != TokenKind.Minus)
            // this should be handled at parsing stage
            throw new ArgumentException("Unexpected unary operator");
                
        if (expression.Op.Kind == TokenKind.Not && innerType != ExpressionType.Boolean)
            throw HandleTypeError(expression, Type.NegateNonBoolean);

        if (expression.Op.Kind == TokenKind.Minus && !IsNumeric(innerType))
            throw HandleTypeError(expression, Type.NegateNonNumeric);
        
        expression.Type = innerType;
    }

    public object? VisitBinaryExpression(BinaryExpression expression)
    {
        TypeExpression(expression.Left);
        TypeExpression(expression.Right);

        var opKind = expression.Op.Kind;
        if (opKind == TokenKind.DoublePlus)
            TypeStringBinary(expression);
        else if (numericOperators.Contains(opKind))
            TypeNumericBinary(expression);
        else if (relationalOperators.Contains(opKind))
            TypeRelationalBinary(expression);
        else
            // unreachable
            throw new ArgumentException($"Unexpected operator {expression.Op.Lexeme}");

        return null;
    }

    private void TypeRelationalBinary(BinaryExpression expression)
    {
        var left = expression.Left;
        var right = expression.Right;
        var numeric = CheckNumericOperands(left, right);
        if (!(left.Type == right.Type || numeric))
        {
            throw HandleTypeError(expression,
                string.Format(Type.ComparisonOperandsMismatch, left.Type, right.Type));
        }

        if (numeric)
            CoerceNumeric(left, right);

        expression.Type = ExpressionType.Boolean;
    }

    private void TypeNumericBinary(BinaryExpression expression)
    {
        var left = expression.Left;
        var right = expression.Right;
        if (!CheckNumericOperands(left, right))
            throw HandleTypeError(expression, Type.NonNumericBinaryOperands);

        expression.Type = CoerceNumeric(left, right);
    }

    private void TypeStringBinary(BinaryExpression expression)
    {
        if (expression.Left.Type != ExpressionType.String || expression.Right.Type != ExpressionType.String)
            throw HandleTypeError(expression, Type.NonStringConcatenation);
        expression.Type = ExpressionType.String;
    }

    public object? VisitTernaryExpression(TernaryExpression expression)
    {
        TypeExpression(expression.Condition);
        if (expression.Condition.Type != ExpressionType.Boolean)
            throw HandleTypeError(expression, Type.NonBooleanTernaryCondition);

        var thenBranch = expression.ThenBranch;
        var elseBranch = expression.ElseBranch;
        TypeExpression(thenBranch);
        TypeExpression(elseBranch);
        var numeric = CheckNumericOperands(thenBranch, elseBranch);

        if (!(thenBranch.Type == elseBranch.Type || numeric))
            throw HandleTypeError(expression, Type.TernaryBranchesMismatch);

        if (numeric)
            CoerceNumeric(thenBranch, elseBranch);

        expression.Type = thenBranch.Type;
        return null;
    }

    public object? VisitLiteralExpression(LiteralExpression expression)
    {
        // type of a literal expression is known at parsing stage
        // so at this point we just check that it's defined
        if (expression.Type == ExpressionType.Untyped)
            throw new ArgumentException("Undefined literal type");

        return null;
    }

    public object? VisitVariableDeclarationExpression(VariableDeclarationExpression expression)
    {
        TypeExpression(expression.Initializer);
        if (!types.ContainsKey(expression.Initializer.Type))
        {
            throw new ArgumentException("Unsupported expression type");
        }

        if (expression.VarType.Kind != TokenKind.Def && expression.VarType.Kind != types[expression.Initializer.Type])
        {
            throw HandleTypeError(expression,
                string.Format(Type.VariableAssignmentMismatch, expression.VarType.Kind));
        }
        
        expression.Type = expression.Initializer.Type;
        _environment.Define(expression.Name.Lexeme, expression.Type);
        return null;
    }

    public object? VisitVariableExpression(VariableExpression expression)
    {
        var storedType = _environment.Get(expression.Name, typeOnly: true).Item1;
        expression.Type = storedType;
        return null;
    }

    public object? VisitAssignmentExpression(AssignmentExpression expression)
    {
        TypeExpression(expression.Value);
        var storedType = _environment.Get(expression.Name, typeOnly: true).Item1;
        if (expression.Value.Type != storedType)
        {
            throw HandleTypeError(expression,
                string.Format(Type.VariableAssignmentMismatch, expression.Value.Type));
        }

        expression.Type = expression.Value.Type;
        return null;
    }

    public object? VisitBlockExpression(BlockExpression expression)
    {
        _environment.NewSubEnvironment(expression.BlockId.Value);
        TypeBlock(expression);
        return null;
    }

    private void TypeBlock(BlockExpression expression)
    {
        var previousEnvironment = _environment;
        try
        {
            _environment = _environment.GetSubEnvironment(expression.BlockId.Value);
            var expressions = expression.Expressions;
            foreach (var expr in expressions)
            {
                Debug.Assert(expr != null, nameof(expr) + " != null");
                TypeExpression(expr);
            }

            expression.Type = expressions[^1]!.Type;
        }
        finally
        {
            _environment = previousEnvironment;
        }
    }
}