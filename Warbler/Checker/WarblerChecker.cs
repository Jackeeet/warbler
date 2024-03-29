﻿using System.Diagnostics;
using Warbler.Environment;
using Warbler.ErrorReporting;
using Warbler.Errors;
using Warbler.Expressions;
using Warbler.Utils.Exceptions;
using Warbler.Utils.Token;
using Warbler.Utils.Type;

namespace Warbler.Checker;

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
            AssignExpressionType(expression);
            return true;
        }
        catch (TypeError)
        {
            return false;
        }
        catch (RuntimeError error)
        {
            // todo reporting a RUNTIME error at CHECKING stage doesn't make sense
            _errorReporter.ReportRuntimeError(error);
            return false;
        }
    }

    private void AssignExpressionType(Expression expression)
    {
        expression.Accept(this);
    }

    private static bool IsNumeric(WarblerType innerType)
    {
        return innerType.BaseType is ExpressionType.Double or ExpressionType.Integer;
    }

    private static bool CheckNumericOperands(Expression left, Expression right)
    {
        return IsNumeric(left.Type) && IsNumeric(right.Type);
    }

    private TypeError HandleTypeError(Expression expression, string message)
    {
        _errorReporter.ErrorAtExpression(expression, message);
        return new TypeError();
    }

    public object? VisitUnaryExpression(UnaryExpression expression)
    {
        AssignExpressionType(expression.Expression);
        var innerType = expression.Expression.Type;
        TypeUnary(expression, innerType);
        return null;
    }

    private void TypeUnary(UnaryExpression expression, WarblerType innerType)
    {
        if (expression.Op.Kind != TokenKind.Not && expression.Op.Kind != TokenKind.Minus)
            // this should be handled at parsing stage, so this is a dev error
            throw new ArgumentException("Unexpected unary operator");

        if (expression.Op.Kind == TokenKind.Not && innerType.BaseType != ExpressionType.Boolean)
            throw HandleTypeError(expression, Resources.Errors.Checker.NegateNonBoolean);

        if (expression.Op.Kind == TokenKind.Minus && !IsNumeric(innerType))
            throw HandleTypeError(expression, Resources.Errors.Checker.NegateNonNumeric);

        expression.Type = innerType;
    }

    public object? VisitBinaryExpression(BinaryExpression expression)
    {
        var left = expression.Left;
        var right = expression.Right;
        AssignExpressionType(left);
        AssignExpressionType(right);
        if (left.Type != right.Type)
            throw HandleTypeError(expression, Resources.Errors.Checker.BinaryOperandsMismatch);

        var opKind = expression.Op.Kind;
        if (opKind == TokenKind.DoublePlus)
            TypeStringBinary(expression);
        else if (numericOperators.Contains(opKind))
            TypeNumericBinary(expression);
        else if (relationalOperators.Contains(opKind))
            expression.Type = new WarblerType(ExpressionType.Boolean);
        else
            throw new UnreachableException($"Unexpected operator {expression.Op.Lexeme}");

        return null;
    }

    private void TypeNumericBinary(BinaryExpression expression)
    {
        var left = expression.Left;
        var right = expression.Right;
        if (!CheckNumericOperands(left, right))
            throw HandleTypeError(expression, Resources.Errors.Checker.NonNumericBinaryOperands);

        expression.Type = expression.Left.Type;
    }

    private void TypeStringBinary(BinaryExpression expression)
    {
        if (expression.Left.Type.BaseType != ExpressionType.String ||
            expression.Right.Type.BaseType != ExpressionType.String)
            throw HandleTypeError(expression, Resources.Errors.Checker.NonStringConcatenation);
        expression.Type = new WarblerType(ExpressionType.String);
    }

    public object? VisitTernaryExpression(TernaryExpression expression)
    {
        AssignExpressionType(expression.Condition);
        if (expression.Condition.Type.BaseType != ExpressionType.Boolean)
            throw HandleTypeError(expression, Resources.Errors.Checker.NonBooleanCondition);

        var thenBranch = expression.ThenBranch;
        var elseBranch = expression.ElseBranch;
        AssignExpressionType(thenBranch);
        AssignExpressionType(elseBranch);

        if (thenBranch.Type != elseBranch.Type)
            throw HandleTypeError(expression, Resources.Errors.Checker.TernaryBranchesMismatch);

        expression.Type = thenBranch.Type;
        return null;
    }

    public object? VisitLiteralExpression(LiteralExpression expression)
    {
        // type of a literal expression is known at parsing stage
        // so at this point we just check that it's defined
        if (expression.Type.BaseType == ExpressionType.Untyped)
            throw new ArgumentException("Undefined literal type");

        return null;
    }

    public object? VisitVariableDeclarationExpression(VariableDeclarationExpression expression)
    {
        AssignExpressionType(expression.Initializer);
        if (!types.ContainsKey(expression.Initializer.Type.BaseType))
        {
            // todo add function support
            throw new ArgumentException("Unsupported expression type");
        }

        if (expression.VarType.Kind != TokenKind.Def &&
            expression.VarType.Kind != types[expression.Initializer.Type.BaseType])
        {
            throw HandleTypeError(expression,
                string.Format(Resources.Errors.Checker.VariableAssignmentMismatch, expression.VarType.Kind));
        }

        expression.Type = expression.Initializer.Type;
        _environment.Define(expression.Name.Lexeme, expression.Type);
        return null;
    }

    public object? VisitVariableExpression(VariableExpression expression)
    {
        // this may throw a RuntimeError
        var storedType = _environment.GetDefined(expression.Name).Item1;
        expression.Type = storedType;
        return null;
    }

    public object? VisitAssignmentExpression(AssignmentExpression expression)
    {
        AssignExpressionType(expression.Value);
        // this may throw a RuntimeError
        var storedType = _environment.GetDefined(expression.Name).Item1;
        if (expression.Value.Type != storedType)
        {
            throw HandleTypeError(expression,
                string.Format(Resources.Errors.Checker.VariableAssignmentMismatch, expression.Value.Type));
        }

        expression.Type = expression.Value.Type;
        return null;
    }

    public object? VisitBlockExpression(BlockExpression expression)
    {
        _environment.AddSubEnvironment(expression.EnvironmentId);
        TypeBlock(expression);
        return null;
    }

    public object? VisitConditionalExpression(ConditionalExpression expression)
    {
        AssignExpressionType(expression.Condition);

        if (expression.Condition.Type.BaseType != ExpressionType.Boolean)
            throw HandleTypeError(expression, Resources.Errors.Checker.NonBooleanCondition);

        // AssignExpressionType(expression.ThenBranch);
        TypeInnerBlock(expression.ThenBranch);
        if (expression.ElseBranch is not null)
        {
            // AssignExpressionType(expression.ElseBranch);
            TypeInnerBlock(expression.ElseBranch);
            if (expression.ThenBranch.Type != expression.ElseBranch.Type)
                throw HandleTypeError(expression, Resources.Errors.Checker.ConditionBranchesMismatch);
        }

        expression.Type = expression.ThenBranch.Type;
        return null;
    }

    public object? VisitWhileLoopExpression(WhileLoopExpression expression)
    {
        AssignExpressionType(expression.Condition);

        if (expression.Condition.Type.BaseType != ExpressionType.Boolean)
            throw HandleTypeError(expression, Resources.Errors.Checker.NonBooleanCondition);

        TypeInnerBlock(expression.Actions);

        expression.Type = new WarblerType(ExpressionType.Integer);
        return null;
    }

    private void TypeInnerBlock(Expression expression)
    {
        if (expression is BlockExpression blockExpression)
        {
            foreach (var expr in blockExpression.Expressions)
            {
                Debug.Assert(expr != null, nameof(expr) + " != null");
                AssignExpressionType(expr);
            }

            blockExpression.Type = blockExpression.Expressions[^1]!.Type;
        }
        else
        {
            AssignExpressionType(expression);
        }
    }

    public object? VisitFunctionDeclarationExpression(FunctionDeclarationExpression expression)
    {
        var functionType = GetFunctionType(expression);
        if (functionType.BaseType != ExpressionType.Function || functionType.Signature is null)
            throw new ArgumentException();

        _environment.Define(expression.Name.Lexeme, functionType);
        CheckParamsAndBody(expression, functionType);
        expression.Type = functionType;
        return null;
    }

    private void CheckParamsAndBody(FunctionDeclarationExpression expression, WarblerType functionType)
    {
        var previousEnvironment = _environment;
        _environment = _environment.AddFunctionEnvironment(expression.Name.Lexeme);
        foreach (var (typeData, name) in expression.Parameters)
            _environment.Define(name.Lexeme, WarblerTypeUtils.ToWarblerType(typeData));

        var bodyExpressions = expression.Body.Expressions;
        foreach (var expr in bodyExpressions)
        {
            Debug.Assert(expr != null, nameof(expr) + " != null");
            AssignExpressionType(expr);
        }

        expression.Body.Type = bodyExpressions[^1]!.Type;

        // signature is not null because the method calling this method
        // checks signature for null before the call
        if (expression.Body.Type != functionType.Signature!.ReturnType)
            throw HandleTypeError(expression, Resources.Errors.Checker.FunctionSignatureMismatch);

        _environment = previousEnvironment;
    }

    private static WarblerType GetFunctionType(FunctionDeclarationExpression expression)
    {
        var returnType = WarblerTypeUtils.ToWarblerType(expression.ReturnType);
        var parameterTypes = new List<WarblerType>();
        foreach (var (te, _) in expression.Parameters)
            parameterTypes.Add(WarblerTypeUtils.ToWarblerType(te));

        var typeSignature = new Signature(parameterTypes, returnType);
        var functionType = new WarblerType(ExpressionType.Function, typeSignature);
        return functionType;
    }

    public object? VisitCallExpression(CallExpression expression)
    {
        AssignExpressionType(expression.Called);
        var signature = expression.Called.Type.Signature;
        if (signature is null)
        {
            throw HandleTypeError(expression,
                string.Format(Resources.Errors.Checker.CallUncallable, expression.Called));
        }

        if (signature.Parameters.Count != expression.Args.Count)
        {
            throw HandleTypeError(expression,
                string.Format(Resources.Errors.Checker.ArgumentCountMismatch, signature.Parameters.Count));
        }

        for (int i = 0; i < signature.Parameters.Count; i++)
        {
            AssignExpressionType(expression.Args[i]);
            if (expression.Args[i].Type != signature.Parameters[i])
            {
                throw HandleTypeError(expression,
                    string.Format(Resources.Errors.Checker.ArgumentTypeMismatch, signature.Parameters[i]));
            }
        }

        expression.Type = signature.ReturnType;
        return null;
    }

    private void TypeBlock(BlockExpression expression)
    {
        var previousEnvironment = _environment;
        try
        {
            _environment = _environment.GetSubEnvironment(expression.EnvironmentId);
            var expressions = expression.Expressions;
            foreach (var expr in expressions)
            {
                Debug.Assert(expr != null, nameof(expr) + " != null");
                AssignExpressionType(expr);
            }

            expression.Type = expressions[^1]!.Type;
        }
        finally
        {
            _environment = previousEnvironment;
        }
    }
}