using System.Collections.Generic;
using Tests.Utils;
using Warbler.Expressions;
using Warbler.Utils.Token;
using Warbler.Utils.Type;

namespace Tests.ResolverTests;

public class Basic
{
    public static readonly List<string> ValidNames = new()
    {
        "literalUnary",
        "variableUnary",

        "literalBinary",
        "leftVariableBinary",
        "rightVariableBinary",
        "fullVariableBinary",

        "literalTernary",
        "variableConditionTernary",
        "variableThenTernary",
        "variableElseTernary",
        "fullVariableTernary",
    };

    public static readonly List<string> InvalidNames = new()
    {
    };

    public InputOutputData<List<Expression>, Dictionary<Expression, int?>> GetInputsOutputs()
    {
        var data = new InputOutputData<List<Expression>, Dictionary<Expression, int?>>();
        data.Add(
            "literalUnary",
            new List<Expression>
            {
                new UnaryExpression(
                    new Token(TokenKind.Minus, "-", null, 1),
                    new LiteralExpression(5) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            },
            new Dictionary<Expression, int?>()
        );
        data.Add(
            "variableUnary",
            new List<Expression>
            {
                new VariableDeclarationExpression(
                    new Token(TokenKind.Def, "def", null, 1),
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new LiteralExpression(true) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                new UnaryExpression(
                    new Token(TokenKind.Not, "!", null, 1),
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "a", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
            },
            new Dictionary<Expression, int?>()
            {
                {
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "a", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                    0
                }
            }
        );
        data.Add("literalBinary",
            new List<Expression>
            {
                new BinaryExpression(
                    new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    new Token(TokenKind.Plus, "+", null, 1),
                    new LiteralExpression(3) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            },
            new Dictionary<Expression, int?>()
        );
        data.Add(
            "leftVariableBinary",
            new List<Expression>
            {
                new VariableDeclarationExpression(
                    new Token(TokenKind.Def, "def", null, 1),
                    new Token(TokenKind.Identifier, "n", null, 1),
                    new LiteralExpression(10) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new BinaryExpression(
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "n", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    new Token(TokenKind.Plus, "+", null, 1),
                    new LiteralExpression(3) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            },
            new Dictionary<Expression, int?>()
            {
                {
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "n", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    0
                }
            }
        );
        data.Add(
            "rightVariableBinary",
            new List<Expression>
            {
                new VariableDeclarationExpression(
                    new Token(TokenKind.Def, "def", null, 1),
                    new Token(TokenKind.Identifier, "n", null, 1),
                    new LiteralExpression(10) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new BinaryExpression(
                    new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    new Token(TokenKind.Plus, "+", null, 1),
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "n", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            },
            new Dictionary<Expression, int?>()
            {
                {
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "n", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    0
                }
            }
        );
        data.Add(
            "fullVariableBinary",
            new List<Expression>
            {
                new VariableDeclarationExpression(
                    new Token(TokenKind.Def, "def", null, 1),
                    new Token(TokenKind.Identifier, "n", null, 1),
                    new LiteralExpression(10) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new VariableDeclarationExpression(
                    new Token(TokenKind.Def, "def", null, 1),
                    new Token(TokenKind.Identifier, "m", null, 1),
                    new LiteralExpression(15) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new BinaryExpression(
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "n", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    new Token(TokenKind.Plus, "+", null, 1),
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "m", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            },
            new Dictionary<Expression, int?>()
            {
                {
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "n", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    0
                },
                {
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "m", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    0
                }
            }
        );
        data.Add(
            "literalTernary",
            new List<Expression>
            {
                new TernaryExpression(
                    new LiteralExpression(true) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                    new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    new LiteralExpression(0) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            },
            new Dictionary<Expression, int?>()
        );
        data.Add(
            "variableConditionTernary",
            new List<Expression>
            {
                new VariableDeclarationExpression(
                    new Token(TokenKind.Def, "def", null, 1),
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new LiteralExpression(true) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new TernaryExpression(
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "a", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                    new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    new LiteralExpression(0) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            },
            new Dictionary<Expression, int?>()
            {
                {
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "a", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                    0
                }
            }
        );
        data.Add(
            "variableThenTernary",
            new List<Expression>
            {
                new VariableDeclarationExpression(
                    new Token(TokenKind.Def, "def", null, 1),
                    new Token(TokenKind.Identifier, "n", null, 1),
                    new LiteralExpression(10) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new TernaryExpression(
                    new LiteralExpression(true) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "n", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    new LiteralExpression(0) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            },
            new Dictionary<Expression, int?>()
            {
                {
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "n", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    0
                }
            }
        );
        data.Add(
            "variableElseTernary",
            new List<Expression>
            {
                new VariableDeclarationExpression(
                    new Token(TokenKind.Def, "def", null, 1),
                    new Token(TokenKind.Identifier, "n", null, 1),
                    new LiteralExpression(10) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new TernaryExpression(
                    new LiteralExpression(true) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                    new LiteralExpression(0) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "n", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            },
            new Dictionary<Expression, int?>()
            {
                {
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "n", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    0
                }
            }
        );
        data.Add(
            "fullVariableTernary",
            new List<Expression>
            {
                new VariableDeclarationExpression(
                    new Token(TokenKind.Def, "def", null, 1),
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new LiteralExpression(true) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                new VariableDeclarationExpression(
                    new Token(TokenKind.Def, "def", null, 1),
                    new Token(TokenKind.Identifier, "n", null, 1),
                    new LiteralExpression(10) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new VariableDeclarationExpression(
                    new Token(TokenKind.Def, "def", null, 1),
                    new Token(TokenKind.Identifier, "m", null, 1),
                    new LiteralExpression(15) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new TernaryExpression(
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "a", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "n", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "m", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            },
            new Dictionary<Expression, int?>()
            {
                {
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "a", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                    0
                },
                {
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "n", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    0
                },
                {
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "m", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    0
                }
            }
        );
        return data;
    }
}