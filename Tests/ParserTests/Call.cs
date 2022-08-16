using System.Collections.Generic;
using Warbler.Expressions;
using Warbler.Utils.Token;
using Warbler.Utils.Type;

namespace Tests.ParserTests;

public class Call
{
    public static readonly List<string> ValidNames = new()
    {
        "functionCall",
        "callAssignment",
        "callWithArguments",
        "callCallback"
    };

    public static readonly List<string> InvalidNames = new()
    {
    };

    public static readonly Dictionary<string, string> Inputs = new()
    {
        { "functionCall", "func()" },
        { "callAssignment", "x = func()" },
        { "callWithArguments", "func(5, a + 7)" },
        { "callCallback", "getCallback()()" }
    };

    public static readonly Dictionary<string, List<Expression?>> Outputs = new()
    {
        {
            "functionCall", new List<Expression?>
            {
                new CallExpression(
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "func", null, 1)
                    ) { Line = 1 },
                    new List<Expression>()
                ) { Line = 1 }
            }
        },
        {
            "callAssignment", new List<Expression?>
            {
                new AssignmentExpression(
                    new Token(TokenKind.Identifier, "x", null, 1),
                    new CallExpression(new VariableExpression(
                            new Token(TokenKind.Identifier, "func", null, 1)
                        ) { Line = 1 },
                        new List<Expression>()
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "callWithArguments", new List<Expression?>
            {
                new CallExpression(new VariableExpression(
                        new Token(TokenKind.Identifier, "func", null, 1)
                    ) { Line = 1 },
                    new List<Expression>
                    {
                        new LiteralExpression(5) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                        new BinaryExpression(
                            new VariableExpression(
                                new Token(TokenKind.Identifier, "a", null, 1)
                            ) { Line = 1 },
                            new Token(TokenKind.Plus, "+", null, 1),
                            new LiteralExpression(7) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                        ) { Line = 1 }
                    }
                ) { Line = 1 }
            }
        },
        {
            "callCallback", new List<Expression?>
            {
                new CallExpression(
                    new CallExpression(
                        new VariableExpression(
                            new Token(TokenKind.Identifier, "getCallback", null, 1)
                        ) { Line = 1 },
                        new List<Expression>()
                    ) { Line = 1 },
                    new List<Expression>()
                ) { Line = 1 }
            }
        }
    };
}