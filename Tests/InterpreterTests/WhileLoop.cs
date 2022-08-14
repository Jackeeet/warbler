using System;
using System.Collections.Generic;
using Tests.Mocks;
using Warbler.Expressions;

namespace Tests.InterpreterTests;

public static class WhileLoop
{
    public static readonly List<string> ValidNames = new()
    {
        "inequality",
        "relational",
        "nested",
        "oneLine"
    };

    public static readonly List<string> InvalidNames = new()
    {
    };

    public static readonly Dictionary<string, Expression> Inputs = new()
    {
        {
            "inequality",
            new WhileLoopExpression(
                new BinaryExpression(
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "i", null, 1)) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.NotEqual, "!=", null, 1),
                    new LiteralExpression(10) { Type = ExpressionType.Integer, Line = 1 }
                ) { Type = ExpressionType.Boolean, Line = 1 },
                new BlockExpression(new TestGuidProvider().Get(),
                    new List<Expression?>
                    {
                        new AssignmentExpression(
                            new Token(TokenKind.Identifier, "i", null, 1),
                            new BinaryExpression(
                                new VariableExpression(
                                    new Token(TokenKind.Identifier, "i", null, 1)
                                ) { Type = ExpressionType.Integer, Line = 1 },
                                new Token(TokenKind.Plus, "+", null, 1),
                                new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 }
                            ) { Type = ExpressionType.Integer, Line = 1 }
                        ) { Type = ExpressionType.Integer, Line = 1 }
                    }
                ) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "relational",
            new WhileLoopExpression(
                new BinaryExpression(
                    new VariableExpression(new Token(TokenKind.Identifier, "i", null, 1)) { Line = 1 },
                    new Token(TokenKind.LessThan, "<", null, 1),
                    new LiteralExpression(10) { Type = ExpressionType.Integer, Line = 1 }
                ) { Type = ExpressionType.Boolean, Line = 1 },
                new BlockExpression(new TestGuidProvider().Get(),
                    new List<Expression?>
                    {
                        new AssignmentExpression(
                            new Token(TokenKind.Identifier, "i", null, 1),
                            new BinaryExpression(
                                new VariableExpression(
                                    new Token(TokenKind.Identifier, "i", null, 1)
                                ) { Type = ExpressionType.Integer, Line = 1 },
                                new Token(TokenKind.Plus, "+", null, 1),
                                new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 }
                            ) { Type = ExpressionType.Integer, Line = 1 }
                        ) { Type = ExpressionType.Integer, Line = 1 }
                    }
                ) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "nested",
            new WhileLoopExpression(
                new BinaryExpression(
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "i", null, 1)
                    ) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.LessThan, "<", null, 1),
                    new LiteralExpression(10) { Type = ExpressionType.Integer, Line = 1 }
                ) { Type = ExpressionType.Boolean, Line = 1 },
                new BlockExpression(new TestGuidProvider().Get(),
                    new List<Expression?>
                    {
                        new WhileLoopExpression(
                            new BinaryExpression(
                                new VariableExpression(new Token(TokenKind.Identifier, "j", null, 1)
                                ) { Type = ExpressionType.Integer, Line = 1 },
                                new Token(TokenKind.LessThan, "<", null, 1),
                                new LiteralExpression(10) { Type = ExpressionType.Integer, Line = 1 }
                            ) { Type = ExpressionType.Boolean, Line = 1 },
                            new BlockExpression(new Guid("00000000-0000-0000-0000-000000000001"),
                                new List<Expression?>
                                {
                                    new AssignmentExpression(
                                        new Token(TokenKind.Identifier, "j", null, 1),
                                        new BinaryExpression(
                                            new VariableExpression(
                                                new Token(TokenKind.Identifier, "j", null, 1)
                                            ) { Type = ExpressionType.Integer, Line = 1 },
                                            new Token(TokenKind.Plus, "+", null, 1),
                                            new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 }
                                        ) { Type = ExpressionType.Integer, Line = 1 }
                                    ) { Type = ExpressionType.Integer, Line = 1 }
                                }
                            ) { Type = ExpressionType.Integer, Line = 1 }
                        ) { Type = ExpressionType.Integer, Line = 1 },
                        new AssignmentExpression(
                            new Token(TokenKind.Identifier, "i", null, 1),
                            new BinaryExpression(
                                new VariableExpression(
                                    new Token(TokenKind.Identifier, "i", null, 1)
                                ) { Type = ExpressionType.Integer, Line = 1 },
                                new Token(TokenKind.Plus, "+", null, 1),
                                new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 }
                            ) { Type = ExpressionType.Integer, Line = 1 }
                        ) { Type = ExpressionType.Integer, Line = 1 }
                    }
                ) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "oneLine",
            new WhileLoopExpression(
                new BinaryExpression(
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "i", null, 1)
                    ) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.LessThan, "<", null, 1),
                    new LiteralExpression(5) { Type = ExpressionType.Integer, Line = 1 }
                ) { Type = ExpressionType.Boolean, Line = 1 },
                new AssignmentExpression(
                    new Token(TokenKind.Identifier, "i", null, 2),
                    new BinaryExpression(
                        new VariableExpression(
                            new Token(TokenKind.Identifier, "i", null, 2)
                        ) { Type = ExpressionType.Integer, Line = 2 },
                        new Token(TokenKind.Plus, "+", null, 2),
                        new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 2 }
                    ) { Type = ExpressionType.Integer, Line = 2 }
                ) { Type = ExpressionType.Integer, Line = 2 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
    };

    public static Dictionary<string, object?> Outputs = new()
    {
        { "inequality", 10 },
        { "relational", 10 },
        { "nested", 10 },
        { "oneLine", 5 },
    };
}