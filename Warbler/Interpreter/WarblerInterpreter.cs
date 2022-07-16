using System.Data;
using Warbler.Expressions;
using Warbler.Parser;
using Warbler.Scanner;

namespace Warbler.Interpreter;

public class WarblerInterpreter : IExpressionVisitor<object?>
{
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
                throw new NotImplementedException();
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

        switch (expression.Op.Kind)
        {
            case TokenKind.Hat:
                break;
            case TokenKind.Asterisk:
                break;
            case TokenKind.Slash:
                break;
            case TokenKind.Modulo:
                break;
            case TokenKind.Plus:
                break;
            case TokenKind.Minus:
                break;
            case TokenKind.DoublePlus:
                return (string)left + (string)right;
            case TokenKind.GreaterThan:
                break;
            case TokenKind.LessThan:
                break;
            case TokenKind.GreaterEqual:
                break;
            case TokenKind.LessEqual:
                break;
            case TokenKind.NotEqual:
                break;
            case TokenKind.DoubleEqual:
                break;
        }

        // unreachable
        return null;
    }

    public object VisitTernaryExpression(TernaryExpression expression)
    {
        throw new NotImplementedException();
    }

    public object? VisitLiteralExpression(LiteralExpression expression)
    {
        return expression.Value;
    }

    // public object VisitGroupingExpression(GroupingExpression expression)
    // {
    //     throw new NotImplementedException();
    // }
}