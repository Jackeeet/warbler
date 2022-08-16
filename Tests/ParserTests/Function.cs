using System;
using System.Collections.Generic;
using Tests.Mocks;
using Warbler.Expressions;
using Warbler.Utils.Token;
using Warbler.Utils.Type;

namespace Tests.ParserTests;

public class Function
{
    public static readonly List<string> ValidNames = new()
    {
        "empty",
        "noParams",
        "singleParam",
        "multipleParams",
        "conditionInBody",
        "whileLoopInBody",
        "blockInBody",
        "nested",
    };

    public static readonly List<string> InvalidNames = new()
    {
    };

    public static readonly Dictionary<string, string> Inputs = new()
    {
        { "empty", "\\ empty() int :> <:" },
        { "noParams", "\\ func() int :> 5 <:" },
        { "singleParam", "\\ increment(int n) int :> n + 1 <:" },
        {
            "multipleParams", "\\ concat(string a, string b, string c, string d) string :>\n" +
                              "    a ++ b ++ c ++ d\n" +
                              "<:"
        },
        {
            "conditionInBody", "\\ func(char ch) double :>\n" +
                               "    def num = 1.0 \n" +
                               "    if ch == 'a' then :>\n" +
                               "        num = 0.0\n" +
                               "    <:\n" +
                               "    num\n" +
                               "<:"
        },
        {
            "whileLoopInBody", "\\ iterCount(int n) int :>\n" +
                               "    while n < 8 :>\n" +
                               "        n = n + 7\n" +
                               "        n = n % 5\n" +
                               "    <:" +
                               "<:"
        },
        {
            "blockInBody", "\\  block(int x) bool :>\n" +
                           "    :> x + 1 <:\n" +
                           "    x\n" +
                           "<:"
        },
        {
            "nested", "\\ outer() int :>\n" +
                      "    \\ inner(int x) int :>\n" +
                      "        x + 1" +
                      "    <:\n" +
                      "<:"
        },
    };

    public static readonly Dictionary<string, List<Expression?>> Outputs = new()
    {
        {
            "empty", new List<Expression?>
            {
                new FunctionDeclarationExpression(
                    new TestIdProvider().GetEnvironmentId(),
                    new Token(TokenKind.Identifier, "empty", null, 1),
                    new List<Tuple<TypeData, Token>>(),
                    new TypeData(ExpressionType.Integer, null, null),
                    new BlockExpression(new TestIdProvider().GetEnvironmentId(), new List<Expression?>()) { Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "noParams", new List<Expression?>
            {
                new FunctionDeclarationExpression(new TestIdProvider().GetEnvironmentId(),
                    new Token(TokenKind.Identifier, "func", null, 1),
                    new List<Tuple<TypeData, Token>>(),
                    new TypeData(ExpressionType.Integer, null, null),
                    new BlockExpression(new TestIdProvider().GetEnvironmentId(), new List<Expression?>
                        {
                            new LiteralExpression(5) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                        }
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "singleParam", new List<Expression?>
            {
                new FunctionDeclarationExpression(
                    new TestIdProvider().GetEnvironmentId(),
                    new Token(TokenKind.Identifier, "increment", null, 1),
                    new List<Tuple<TypeData, Token>>
                    {
                        Tuple.Create(
                            new TypeData(ExpressionType.Integer, null, null),
                            new Token(TokenKind.Identifier, "n", null, 1)
                        )
                    },
                    new TypeData(ExpressionType.Integer, null, null),
                    new BlockExpression(new TestIdProvider().GetEnvironmentId(), new List<Expression?>
                        {
                            new BinaryExpression(
                                new VariableExpression(
                                    new Token(TokenKind.Identifier, "n", null, 1)
                                ) { Line = 1 },
                                new Token(TokenKind.Plus, "+", null, 1),
                                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                            ) { Line = 1 }
                        }
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "multipleParams", new List<Expression?>
            {
                new FunctionDeclarationExpression(new TestIdProvider().GetEnvironmentId(),
                    new Token(TokenKind.Identifier, "concat", null, 1),
                    new List<Tuple<TypeData, Token>>
                    {
                        Tuple.Create(
                            new TypeData(ExpressionType.String, null, null),
                            new Token(TokenKind.Identifier, "a", null, 1)
                        ),
                        Tuple.Create(
                            new TypeData(ExpressionType.String, null, null),
                            new Token(TokenKind.Identifier, "b", null, 1)
                        ),
                        Tuple.Create(
                            new TypeData(ExpressionType.String, null, null),
                            new Token(TokenKind.Identifier, "c", null, 1)
                        ),
                        Tuple.Create(
                            new TypeData(ExpressionType.String, null, null),
                            new Token(TokenKind.Identifier, "d", null, 1)
                        ),
                    },
                    new TypeData(ExpressionType.String, null, null),
                    new BlockExpression(new TestIdProvider().GetEnvironmentId(), new List<Expression?>
                        {
                            new BinaryExpression(
                                new BinaryExpression(
                                    new BinaryExpression(new VariableExpression(
                                            new Token(TokenKind.Identifier, "a", null, 2)
                                        ) { Line = 2 },
                                        new Token(TokenKind.DoublePlus, "++", null, 2),
                                        new VariableExpression(
                                            new Token(TokenKind.Identifier, "b", null, 2)
                                        ) { Line = 2 }) { Line = 2 },
                                    new Token(TokenKind.DoublePlus, "++", null, 2),
                                    new VariableExpression(
                                        new Token(TokenKind.Identifier, "c", null, 2)
                                    ) { Line = 2 }
                                ) { Line = 2 },
                                new Token(TokenKind.DoublePlus, "++", null, 2),
                                new VariableExpression(
                                    new Token(TokenKind.Identifier, "d", null, 2)
                                ) { Line = 2 }
                            ) { Line = 2 }
                        }
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "conditionInBody", new List<Expression?>
            {
                new FunctionDeclarationExpression(new TestIdProvider().GetEnvironmentId(),
                    new Token(TokenKind.Identifier, "func", null, 1),
                    new List<Tuple<TypeData, Token>>
                    {
                        Tuple.Create(
                            new TypeData(ExpressionType.Char, null, null),
                            new Token(TokenKind.Identifier, "ch", null, 1)
                        )
                    },
                    new TypeData(ExpressionType.Double, null, null),
                    new BlockExpression(new TestIdProvider().GetEnvironmentId(), new List<Expression?>
                        {
                            new VariableDeclarationExpression(
                                new Token(TokenKind.Def, "def", null, 2),
                                new Token(TokenKind.Identifier, "num", null, 2),
                                new LiteralExpression(1.0) { Type = new WarblerType(ExpressionType.Double), Line = 2 }
                            ) { Line = 2 },
                            new ConditionalExpression(
                                new BinaryExpression(
                                    new VariableExpression(
                                        new Token(TokenKind.Identifier, "ch", null, 3)
                                    ) { Line = 3 },
                                    new Token(TokenKind.DoubleEqual, "==", null, 3),
                                    new LiteralExpression('a') { Type = new WarblerType(ExpressionType.Char), Line = 3 }
                                ) { Line = 3 },
                                new BlockExpression(new TestIdProvider().GetEnvironmentId(), new List<Expression?>
                                    {
                                        new AssignmentExpression(
                                            new Token(TokenKind.Identifier, "num", null, 4),
                                            new LiteralExpression(0.0)
                                                { Type = new WarblerType(ExpressionType.Double), Line = 4 }
                                        ) { Line = 4 }
                                    }
                                ) { Line = 3 },
                                null
                            ) { Line = 3 },
                            new VariableExpression(
                                new Token(TokenKind.Identifier, "num", null, 6)
                            ) { Line = 6 }
                        }
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "whileLoopInBody", new List<Expression?>
            {
                new FunctionDeclarationExpression(new TestIdProvider().GetEnvironmentId(),
                    new Token(TokenKind.Identifier, "iterCount", null, 1),
                    new List<Tuple<TypeData, Token>>
                    {
                        Tuple.Create(
                            new TypeData(ExpressionType.Integer, null, null),
                            new Token(TokenKind.Identifier, "n", null, 1)
                        )
                    },
                    new TypeData(ExpressionType.Integer, null, null),
                    new BlockExpression(new TestIdProvider().GetEnvironmentId(), new List<Expression?>
                        {
                            new WhileLoopExpression(
                                new BinaryExpression(
                                    new VariableExpression(
                                        new Token(TokenKind.Identifier, "n", null, 2)
                                    ) { Line = 2 },
                                    new Token(TokenKind.LessThan, "<", null, 2),
                                    new LiteralExpression(8)
                                        { Type = new WarblerType(ExpressionType.Integer), Line = 2 }
                                ) { Line = 2 },
                                new BlockExpression(new TestIdProvider().GetEnvironmentId(),
                                    new List<Expression?>
                                    {
                                        new AssignmentExpression(
                                            new Token(TokenKind.Identifier, "n", null, 3),
                                            new BinaryExpression(
                                                new VariableExpression(
                                                    new Token(TokenKind.Identifier, "n", null, 3)
                                                ) { Line = 3 },
                                                new Token(TokenKind.Plus, "+", null, 3),
                                                new LiteralExpression(7)
                                                    { Type = new WarblerType(ExpressionType.Integer), Line = 3 }
                                            ) { Line = 3 }
                                        ) { Line = 3 },
                                        new AssignmentExpression(
                                            new Token(TokenKind.Identifier, "n", null, 4),
                                            new BinaryExpression(
                                                new VariableExpression(
                                                    new Token(TokenKind.Identifier, "n", null, 4)
                                                ) { Line = 4 },
                                                new Token(TokenKind.Percent, "%", null, 4),
                                                new LiteralExpression(5)
                                                    { Type = new WarblerType(ExpressionType.Integer), Line = 4 }
                                            ) { Line = 4 }
                                        ) { Line = 4 }
                                    }
                                ) { Line = 2 }
                            ) { Line = 2 }
                        }
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "blockInBody", new List<Expression?>
            {
                new FunctionDeclarationExpression(new TestIdProvider().GetEnvironmentId(),
                    new Token(TokenKind.Identifier, "block", null, 1),
                    new List<Tuple<TypeData, Token>>
                    {
                        Tuple.Create(
                            new TypeData(ExpressionType.Integer, null, null),
                            new Token(TokenKind.Identifier, "x", null, 1)
                        )
                    },
                    new TypeData(ExpressionType.Boolean, null, null),
                    new BlockExpression(new TestIdProvider().GetEnvironmentId(), new List<Expression?>
                        {
                            new BlockExpression(new TestIdProvider().GetEnvironmentId(), new List<Expression?>
                                {
                                    new BinaryExpression(
                                        new VariableExpression(
                                            new Token(TokenKind.Identifier, "x", null, 2)
                                        ) { Line = 2 },
                                        new Token(TokenKind.Plus, "+", null, 2),
                                        new LiteralExpression(1)
                                            { Type = new WarblerType(ExpressionType.Integer), Line = 2 }
                                    ) { Line = 2 }
                                }
                            ) { Line = 2 },
                            new VariableExpression(
                                new Token(TokenKind.Identifier, "x", null, 3)
                            ) { Line = 3 }
                        }
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "nested", new List<Expression?>
            {
                new FunctionDeclarationExpression(new TestIdProvider().GetEnvironmentId(),
                    new Token(TokenKind.Identifier, "outer", null, 1),
                    new List<Tuple<TypeData, Token>>(),
                    new TypeData(ExpressionType.Integer, null, null),
                    new BlockExpression(new TestIdProvider().GetEnvironmentId(), new List<Expression?>
                        {
                            new FunctionDeclarationExpression(new TestIdProvider().GetEnvironmentId(),
                                new Token(TokenKind.Identifier, "inner", null, 2),
                                new List<Tuple<TypeData, Token>>
                                {
                                    Tuple.Create(
                                        new TypeData(ExpressionType.Integer, null, null),
                                        new Token(TokenKind.Identifier, "x", null, 2)
                                    )
                                },
                                new TypeData(ExpressionType.Integer, null, null),
                                new BlockExpression(new TestIdProvider().GetEnvironmentId(), new List<Expression?>
                                    {
                                        new BinaryExpression(
                                            new VariableExpression(
                                                new Token(TokenKind.Identifier, "x", null, 3)
                                            ) { Line = 3 },
                                            new Token(TokenKind.Plus, "+", null, 3),
                                            new LiteralExpression(1)
                                                { Type = new WarblerType(ExpressionType.Integer), Line = 3 }
                                        ) { Line = 3 }
                                    }
                                ) { Line = 2 }
                            ) { Line = 2 }
                        }
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        },
    };
}