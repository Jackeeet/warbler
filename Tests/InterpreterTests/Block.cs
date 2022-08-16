using System;
using System.Collections.Generic;
using Warbler.Expressions;
using Warbler.Utils.Id;
using Warbler.Utils.Token;
using Warbler.Utils.Type;

namespace Tests.InterpreterTests;

public static class Block
{
    public static readonly EnvId OuterBlockId = new(new Guid());
    public static readonly EnvId InnerBlockId = new(new Guid("00000000-0000-0000-0000-000000000001"));

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
                        new LiteralExpression(10) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
        {
            "multiExpressionBlock",
            new BlockExpression(OuterBlockId, new List<Expression?>
                {
                    new VariableDeclarationExpression(
                        new Token(TokenKind.Int, "int", null, 1),
                        new Token(TokenKind.Identifier, "block", null, 1),
                        new LiteralExpression(10) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    new AssignmentExpression(
                        new Token(TokenKind.Identifier, "block", null, 1),
                        new LiteralExpression(15) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
        {
            "nestedBlock",
            new BlockExpression(OuterBlockId, new List<Expression?>
                {
                    new VariableDeclarationExpression(
                        new Token(TokenKind.Int, "int", null, 1),
                        new Token(TokenKind.Identifier, "block", null, 1),
                        new LiteralExpression(10) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    new BlockExpression(InnerBlockId, new List<Expression?>()
                        {
                            new VariableDeclarationExpression(
                                new Token(TokenKind.Int, "int", null, 1),
                                new Token(TokenKind.Identifier, "block", null, 1),
                                new LiteralExpression(20) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                        }
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    new VariableDeclarationExpression(
                        new Token(TokenKind.Int, "int", null, 1),
                        new Token(TokenKind.Identifier, "block2", null, 1),
                        new VariableExpression(
                            new Token(TokenKind.Identifier, "block", null, 1)
                        ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
    };

    public static readonly Dictionary<string, object?> Outputs = new()
    {
        { "simpleBlock", 10 },
        { "multiExpressionBlock", 15 },
        { "nestedBlock", 10 },
    };
}