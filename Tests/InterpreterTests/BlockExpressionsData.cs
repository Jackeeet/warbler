using System;
using System.Collections.Generic;
using Warbler.Expressions;

namespace Tests.InterpreterTests;

public static class BlockExpressionsData
{
    public static Guid OuterBlockId = new();
    public static Guid InnerBlockId = new("00000000-0000-0000-0000-000000000001");

    public static readonly List<string> ValidNames = new()
    {
        "simpleBlock",
        "multiExpressionBlock",
        "nestedBlock"
    };

    public static readonly List<string> InvalidNames = new() { };

    public static readonly Dictionary<string, Expression> Inputs = new()
    {
        {
            "simpleBlock",
            new BlockExpression(OuterBlockId, new List<Expression?>
                {
                    new VariableDeclarationExpression(
                        new Token(TokenKind.Int, "int", null, 1),
                        new Token(TokenKind.Identifier, "block", null, 1),
                        new LiteralExpression(10) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Type = ExpressionType.Integer, Line = 1 }
                }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "multiExpressionBlock",
            new BlockExpression(OuterBlockId, new List<Expression?>
                {
                    new VariableDeclarationExpression(
                        new Token(TokenKind.Int, "int", null, 1),
                        new Token(TokenKind.Identifier, "block", null, 1),
                        new LiteralExpression(10) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Type = ExpressionType.Integer, Line = 1 },
                    new AssignmentExpression(
                        new Token(TokenKind.Identifier, "block", null, 1),
                        new LiteralExpression(15) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Type = ExpressionType.Integer, Line = 1 }
                }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "nestedBlock",
            new BlockExpression(OuterBlockId, new List<Expression?>
                {
                    new VariableDeclarationExpression(
                        new Token(TokenKind.Int, "int", null, 1),
                        new Token(TokenKind.Identifier, "block", null, 1),
                        new LiteralExpression(10) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Type = ExpressionType.Integer, Line = 1 },
                    new BlockExpression(InnerBlockId, new List<Expression?>()
                        {
                            new VariableDeclarationExpression(
                                new Token(TokenKind.Int, "int", null, 1),
                                new Token(TokenKind.Identifier, "block", null, 1),
                                new LiteralExpression(20) { Type = ExpressionType.Integer, Line = 1 }
                            ) { Type = ExpressionType.Integer, Line = 1 },
                        }
                    ) { Type = ExpressionType.Integer, Line = 1 },
                    new VariableDeclarationExpression(
                        new Token(TokenKind.Int, "int", null, 1),
                        new Token(TokenKind.Identifier, "block2", null, 1),
                        new VariableExpression(
                            new Token(TokenKind.Identifier, "block", null, 1)
                        ) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Type = ExpressionType.Integer, Line = 1 }
                }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
    };

    public static readonly Dictionary<string, object?> Outputs = new()
    {
        { "simpleBlock", 10 },
        { "multiExpressionBlock", 15 },
        { "nestedBlock", 10 },
    };
}