using System.Collections.Generic;
using Tests.Mocks;
using Warbler.Expressions;

namespace Tests.ParserTests;

public static class WhileLoop
{
    public static readonly List<string> ValidNames = new()
    {
        "inequality",
        "relational",
        "ternary",
        "nested",
        "oneLine"
    };

    public static readonly List<string> InvalidNames = new()
    {
    };

    public static readonly Dictionary<string, string> Inputs = new()
    {
        {
            "inequality", "while i != 10 :>" +
                          "    i = i + 1" +
                          "<:"
        },
        {
            "relational", "while i < 10 :>" +
                          "    i = i + 1" +
                          "<:"
        },
        {
            "ternary", "while 1 == 2 ? true : false :> " +
                       "    bool x = true" +
                       "<:"
        },
        {
            "nested", "while true :>" +
                      "    while i < 10 :>" +
                      "        i = i + 1" +
                      "    <:" +
                      "<:"
        },
        {
            "oneLine", "while true\n" +
                       "    i = i + 1"
        }
    };

    public static readonly Dictionary<string, List<Expression?>> Outputs = new()
    {
        {
            "inequality", new List<Expression?>
            {
                new WhileLoopExpression(
                    new BinaryExpression(
                        new VariableExpression(new Token(TokenKind.Identifier, "i", null, 1)) { Line = 1 },
                        new Token(TokenKind.NotEqual, "!=", null, 1),
                        new LiteralExpression(10) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                    ) { Line = 1 },
                    new BlockExpression(new TestGuidProvider().Get(),
                        new List<Expression?>
                        {
                            new AssignmentExpression(
                                new Token(TokenKind.Identifier, "i", null, 1),
                                new BinaryExpression(
                                    new VariableExpression(
                                        new Token(TokenKind.Identifier, "i", null, 1)
                                    ) { Line = 1 },
                                    new Token(TokenKind.Plus, "+", null, 1),
                                    new LiteralExpression(1)
                                        { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                                ) { Line = 1 }
                            ) { Line = 1 }
                        }
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "relational", new List<Expression?>
            {
                new WhileLoopExpression(
                    new BinaryExpression(
                        new VariableExpression(new Token(TokenKind.Identifier, "i", null, 1)) { Line = 1 },
                        new Token(TokenKind.LessThan, "<", null, 1),
                        new LiteralExpression(10) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                    ) { Line = 1 },
                    new BlockExpression(new TestGuidProvider().Get(),
                        new List<Expression?>
                        {
                            new AssignmentExpression(
                                new Token(TokenKind.Identifier, "i", null, 1),
                                new BinaryExpression(
                                    new VariableExpression(
                                        new Token(TokenKind.Identifier, "i", null, 1)
                                    ) { Line = 1 },
                                    new Token(TokenKind.Plus, "+", null, 1),
                                    new LiteralExpression(1)
                                        { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                                ) { Line = 1 }
                            ) { Line = 1 }
                        }
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "ternary", new List<Expression?>
            {
                new WhileLoopExpression(
                    new TernaryExpression(
                        new BinaryExpression(
                            new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                            new Token(TokenKind.DoubleEqual, "==", null, 1),
                            new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                        ) { Line = 1 },
                        new LiteralExpression(true) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                        new LiteralExpression(false) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
                    ) { Line = 1 },
                    new BlockExpression(new TestGuidProvider().Get(),
                        new List<Expression?>
                        {
                            new VariableDeclarationExpression(
                                new Token(TokenKind.Bool, "bool", null, 1),
                                new Token(TokenKind.Identifier, "x", null, 1),
                                new LiteralExpression(true) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
                            ) { Line = 1 }
                        }
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "nested", new List<Expression?>
            {
                new WhileLoopExpression(
                    new LiteralExpression(true) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                    new BlockExpression(new TestGuidProvider().Get(),
                        new List<Expression?>
                        {
                            new WhileLoopExpression(
                                new BinaryExpression(
                                    new VariableExpression(new Token(TokenKind.Identifier, "i", null, 1)) { Line = 1 },
                                    new Token(TokenKind.LessThan, "<", null, 1),
                                    new LiteralExpression(10)
                                        { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                                ) { Line = 1 },
                                new BlockExpression(new TestGuidProvider().Get(),
                                    new List<Expression?>
                                    {
                                        new AssignmentExpression(
                                            new Token(TokenKind.Identifier, "i", null, 1),
                                            new BinaryExpression(
                                                new VariableExpression(
                                                    new Token(TokenKind.Identifier, "i", null, 1)
                                                ) { Line = 1 },
                                                new Token(TokenKind.Plus, "+", null, 1),
                                                new LiteralExpression(1)
                                                    { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                                            ) { Line = 1 }
                                        ) { Line = 1 }
                                    }
                                ) { Line = 1 }
                            ) { Line = 1 }
                        }
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "oneLine", new List<Expression?>
            {
                new WhileLoopExpression(
                    new LiteralExpression(true) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                    new AssignmentExpression(
                        new Token(TokenKind.Identifier, "i", null, 2),
                        new BinaryExpression(
                            new VariableExpression(
                                new Token(TokenKind.Identifier, "i", null, 2)
                            ) { Line = 2 },
                            new Token(TokenKind.Plus, "+", null, 2),
                            new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 2 }
                        ) { Line = 2 }
                    ) { Line = 2 }
                ) { Line = 1 }
            }
        },
    };
}