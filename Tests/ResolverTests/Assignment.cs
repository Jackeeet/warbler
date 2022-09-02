using System.Collections.Generic;
using Tests.Utils;
using Warbler.Expressions;
using Warbler.Utils.Token;
using Warbler.Utils.Type;

namespace Tests.ResolverTests;

public class Assignment
{
    public static readonly List<string> ValidNames = new()
    {
        "assignLiteral",
        "assignVariable"
    };

    public static readonly List<string> InvalidNames = new()
    {
    };

    public InputOutputData<List<Expression>, Dictionary<Expression, int?>> GetInputsOutputs()
    {
        var data = new InputOutputData<List<Expression>, Dictionary<Expression, int?>>();
        data.Add(
            "assignLiteral",
            new List<Expression>()
            {
                new VariableDeclarationExpression(
                    new Token(TokenKind.Def, "def", null, 1),
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new LiteralExpression(5) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new AssignmentExpression(
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new LiteralExpression(10) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
            },
            new Dictionary<Expression, int?>()
        );
        data.Add(
            "assignVariable",
            new List<Expression>()
            {
                new VariableDeclarationExpression(
                    new Token(TokenKind.Def, "def", null, 1),
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new LiteralExpression(5) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new VariableDeclarationExpression(
                    new Token(TokenKind.Def, "def", null, 1),
                    new Token(TokenKind.Identifier, "b", null, 1),
                    new LiteralExpression(10) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new AssignmentExpression(
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "b", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            },
            new Dictionary<Expression, int?>()
            {
                {
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "b", null, 1)
                    ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    0
                }
            }
        );

        return data;
    }
}