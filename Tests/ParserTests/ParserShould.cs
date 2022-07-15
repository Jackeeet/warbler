using System.Collections.Generic;
using NUnit.Framework;
using Tests.Mocks;
using Warbler.Parser;
using Warbler.Scanner;
using BinaryExpression = Warbler.Parser.BinaryExpression;
using Expression = Warbler.Parser.Expression;
using UnaryExpression = Warbler.Parser.UnaryExpression;

namespace Tests.ParserTests;

public class ParserShould
{
    private TestReporter _errorReporter = null!;

    [OneTimeSetUp]
    public void BeforeFixture()
    {
        _errorReporter = new TestReporter();
        Assert.AreEqual(inputs.Count, outputs.Count);
        Assert.AreEqual(validInputNames.Count + invalidInputNames.Count, inputs.Count);
    }

    [SetUp]
    public void BeforeTest()
    {
        _errorReporter.Reset();
    }

    [Test]
    [TestCaseSource(nameof(validInputNames))]
    public void ParseValidInputs(string inputName)
    {
        var tokens = new WarblerScanner(inputs[inputName], _errorReporter).Scan();
        _errorReporter.Reset();
        var parser = new WarblerParser(tokens, _errorReporter);
        var expected = outputs[inputName];

        var actual = parser.Parse();

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(nameof(invalidInputNames))]
    public void ThrowErrorWhen(string inputName)
    {
        var tokens = new WarblerScanner(inputs[inputName], _errorReporter).Scan();
        _errorReporter.Reset();
        var parser = new WarblerParser(tokens, _errorReporter);

        var actual = parser.Parse();

        Assert.IsNull(actual);
        Assert.IsTrue(_errorReporter.HadError);
        // maybe check error msg as well
    }

    private static readonly List<string> validInputNames = new()
    {
        "intLiteral",
        "doubleLiteral",
        "stringLiteral",
        "charLiteral",
        "true",
        "false",
        "grouping",
        "raiseToPower",
        "raiseGroupingToPower",
        "raiseToGrouping",
        "chainPower",
        "unaryMinus",
        "unaryNot",
        "unaryGrouping",
        "multiplication",
        "chainMultiplication",
        "division",
        "chainDivision",
        "modulo",
        "chainModulo",
        "addition",
        "chainAddition",
        "subtraction",
        "chainSubtraction",
        "unarySubtraction",
        "concatenation",
        "chainConcatenation",
        "arithmeticPrecedence",
        "greaterThan",
        "lessThan",
        "greaterEqual",
        "lessEqual",
        "relationalPrecedence",
        "equality",
        "inequality",
        "equalityPrecedence",
        "ternary",
        "nestedTernary",
        "ternaryPrecedence"
    };

    private static readonly List<string> invalidInputNames = new()
    {
        "empty",
        "chainComparison",
        "chainEquality",
        "unsupportedUnary",
        "unsupportedBinary",
        "unsupportedTernaryCondition",
        "unsupportedTernaryBranch",
        "noBinaryArg",
        "noBinaryArgs",
        "noTernaryCondition",
        "noTernaryThen",
        "noTernaryElse",
        "noClosingBracket",
        "noOpeningBracket"
    };

    private static readonly Dictionary<string, string> inputs = new()
    {
        #region validInputs

        { "intLiteral", "2" },
        { "doubleLiteral", "3.4" },
        { "stringLiteral", "\"hello birds\"" },
        { "charLiteral", "'a'" },
        { "true", "true" },
        { "false", "false" },
        { "grouping", "(2 + 2)" },
        { "raiseToPower", "2 ^ 2" },
        { "raiseGroupingToPower", "(2 + 2) ^ 2" },
        { "raiseToGrouping", "2 ^ (2 + 2)" },
        { "chainPower", "2 ^ 2 ^ 2 ^ 2" },
        { "unaryMinus", "-123" },
        { "unaryNot", "!false" },
        { "unaryGrouping", "-(2 + 2)" },
        { "multiplication", "2 * 2" },
        { "chainMultiplication", "1 * 2 * 3" },
        { "division", "2 / 2" },
        { "chainDivision", "1 / 2 / 3" },
        { "modulo", "2 % 2" },
        { "chainModulo", "1 % 2 % 3" },
        { "addition", "2 + 2" },
        { "chainAddition", "1 + 2 + 3" },
        { "subtraction", "2 - 2" },
        { "chainSubtraction", "1 - 2 - 3" },
        { "unarySubtraction", "-1 - 2" },
        { "concatenation", "\"a\" ++ \"b\"" },
        { "chainConcatenation", "\"a\" ++ \"b\" ++ \"c\"" },
        { "arithmeticPrecedence", "-5 + (1 + 2) ^ 3 * 4" },
        { "greaterThan", "3 > 2" },
        { "lessThan", "2 < 3 " },
        { "greaterEqual", "3 >=2" },
        { "lessEqual", "\"a\" <= \"b\"" },
        { "relationalPrecedence", "2 + 3 <= 5" },
        { "equality", "2 == 2" },
        { "inequality", "2 != 3" },
        { "equalityPrecedence", "2 < 3 == \"a\" < \"b\"" },
        { "ternary", "true ? 1 : 0" },
        { "nestedTernary", "true ? false ? 1 : 2 : true ? 3 : 4" },
        { "ternaryPrecedence", "2 != 3 ? 2 + 3 : 2 - 3" },

        #endregion

        { "empty", "" },
        { "chainComparison", "x < y <= z" },
        { "chainEquality", "x == y != z" },
        { "unsupportedUnary", "/2" },
        { "unsupportedBinary", "2 _ 2" },
        { "unsupportedTernaryCondition", "x # y : z" },
        { "unsupportedTernaryBranch", "x ? y < z" },
        { "noBinaryArg", "+" },
        { "noBinaryArgs", "2 +" },
        { "noTernaryCondition", "? y : z" },
        { "noTernaryThen", "x ? : z" },
        { "noTernaryElse", "x ? y :" },
        { "noClosingBracket", "(2 + 3" },
        { "noOpeningBracket", "2 + 3)" },
    };

    private static readonly Dictionary<string, Expression?> outputs = new()
    {
        { "empty", null },
        { "chainComparison", null },
        { "chainEquality", null },
        { "unsupportedUnary", null },
        { "unsupportedBinary", null },
        { "unsupportedTernaryCondition", null },
        { "unsupportedTernaryBranch", null },
        { "noBinaryArg", null },
        { "noBinaryArgs", null },
        { "noTernaryCondition", null },
        { "noTernaryThen", null },
        { "noTernaryElse", null },
        { "noClosingBracket", null },
        { "noOpeningBracket", null },

        #region validOutputs

        {
            "ternaryPrecedence",
            new TernaryExpression(
                new BinaryExpression(
                    new LiteralExpression(2),
                    new Token(TokenKind.NotEqual, "!=", null, 1),
                    new LiteralExpression(3)
                ),
                new BinaryExpression(
                    new LiteralExpression(2),
                    new Token(TokenKind.Plus, "+", null, 1),
                    new LiteralExpression(3)
                ),
                new BinaryExpression(
                    new LiteralExpression(2),
                    new Token(TokenKind.Minus, "-", null, 1),
                    new LiteralExpression(3)
                )
            )
        },

        {
            "nestedTernary",
            new TernaryExpression(
                new LiteralExpression(true),
                new TernaryExpression(
                    new LiteralExpression(false),
                    new LiteralExpression(1),
                    new LiteralExpression(2)
                ),
                new TernaryExpression(
                    new LiteralExpression(true),
                    new LiteralExpression(3),
                    new LiteralExpression(4)
                )
            )
        },
        {
            "ternary",
            new TernaryExpression(
                new LiteralExpression(true),
                new LiteralExpression(1),
                new LiteralExpression(0)
            )
        },
        {
            "equalityPrecedence",
            new BinaryExpression(
                new BinaryExpression(
                    new LiteralExpression(2),
                    new Token(TokenKind.LessThan, "<", null, 1),
                    new LiteralExpression(3)
                ),
                new Token(TokenKind.DoubleEqual, "==", null, 1),
                new BinaryExpression(
                    new LiteralExpression("a"),
                    new Token(TokenKind.LessThan, "<", null, 1),
                    new LiteralExpression("b")
                )
            )
        },
        {
            "inequality",
            new BinaryExpression(
                new LiteralExpression(2),
                new Token(TokenKind.NotEqual, "!=", null, 1),
                new LiteralExpression(3)
            )
        },
        {
            "equality",
            new BinaryExpression(
                new LiteralExpression(2),
                new Token(TokenKind.DoubleEqual, "==", null, 1),
                new LiteralExpression(2)
            )
        },
        {
            "relationalPrecedence",
            new BinaryExpression(
                new BinaryExpression(
                    new LiteralExpression(2),
                    new Token(TokenKind.Plus, "+", null, 1),
                    new LiteralExpression(3)
                ),
                new Token(TokenKind.LessEqual, "<=", null, 1),
                new LiteralExpression(5)
            )
        },
        {
            "lessEqual",
            new BinaryExpression(
                new LiteralExpression("a"),
                new Token(TokenKind.LessEqual, "<=", null, 1),
                new LiteralExpression("b")
            )
        },
        {
            "greaterEqual",
            new BinaryExpression(
                new LiteralExpression(3),
                new Token(TokenKind.GreaterEqual, ">=", null, 1),
                new LiteralExpression(2)
            )
        },
        {
            "lessThan",
            new BinaryExpression(
                new LiteralExpression(2),
                new Token(TokenKind.LessThan, "<", null, 1),
                new LiteralExpression(3)
            )
        },
        {
            "greaterThan",
            new BinaryExpression(
                new LiteralExpression(3),
                new Token(TokenKind.GreaterThan, ">", null, 1),
                new LiteralExpression(2)
            )
        },
        {
            "arithmeticPrecedence",
            new BinaryExpression(
                new UnaryExpression(
                    new Token(TokenKind.Minus, "-", null, 1),
                    new LiteralExpression(5)
                ),
                new Token(TokenKind.Plus, "+", null, 1),
                new BinaryExpression(
                    new BinaryExpression(
                        new GroupingExpression(
                            new BinaryExpression(
                                new LiteralExpression(1),
                                new Token(TokenKind.Plus, "+", null, 1),
                                new LiteralExpression(2)
                            )
                        ),
                        new Token(TokenKind.Hat, "^", null, 1),
                        new LiteralExpression(3)
                    ),
                    new Token(TokenKind.Asterisk, "*", null, 1),
                    new LiteralExpression(4)
                )
            )
        },
        {
            "chainConcatenation",
            new BinaryExpression(
                new BinaryExpression(
                    new LiteralExpression("a"),
                    new Token(TokenKind.DoublePlus, "++", null, 1),
                    new LiteralExpression("b")
                ),
                new Token(TokenKind.DoublePlus, "++", null, 1),
                new LiteralExpression("c")
            )
        },
        {
            "concatenation",
            new BinaryExpression(
                new LiteralExpression("a"),
                new Token(TokenKind.DoublePlus, "++", null, 1),
                new LiteralExpression("b")
            )
        },
        {
            "unarySubtraction",
            new BinaryExpression(
                new UnaryExpression(
                    new Token(TokenKind.Minus, "-", null, 1),
                    new LiteralExpression(1)
                ),
                new Token(TokenKind.Minus, "-", null, 1),
                new LiteralExpression(2)
            )
        },
        {
            "chainSubtraction",
            new BinaryExpression(
                new BinaryExpression(
                    new LiteralExpression(1),
                    new Token(TokenKind.Minus, "-", null, 1),
                    new LiteralExpression(2)
                ),
                new Token(TokenKind.Minus, "-", null, 1),
                new LiteralExpression(3)
            )
        },
        {
            "subtraction",
            new BinaryExpression(
                new LiteralExpression(2),
                new Token(TokenKind.Minus, "-", null, 1),
                new LiteralExpression(2)
            )
        },
        {
            "chainAddition",
            new BinaryExpression(
                new BinaryExpression(
                    new LiteralExpression(1),
                    new Token(TokenKind.Plus, "+", null, 1),
                    new LiteralExpression(2)
                ),
                new Token(TokenKind.Plus, "+", null, 1),
                new LiteralExpression(3)
            )
        },
        {
            "addition",
            new BinaryExpression(
                new LiteralExpression(2),
                new Token(TokenKind.Plus, "+", null, 1),
                new LiteralExpression(2)
            )
        },
        {
            "chainModulo",
            new BinaryExpression(
                new BinaryExpression(
                    new LiteralExpression(1),
                    new Token(TokenKind.Modulo, "%", null, 1),
                    new LiteralExpression(2)
                ),
                new Token(TokenKind.Modulo, "%", null, 1),
                new LiteralExpression(3)
            )
        },
        {
            "modulo",
            new BinaryExpression(
                new LiteralExpression(2),
                new Token(TokenKind.Modulo, "%", null, 1),
                new LiteralExpression(2)
            )
        },
        {
            "chainDivision",
            new BinaryExpression(
                new BinaryExpression(
                    new LiteralExpression(1),
                    new Token(TokenKind.Slash, "/", null, 1),
                    new LiteralExpression(2)
                ),
                new Token(TokenKind.Slash, "/", null, 1),
                new LiteralExpression(3)
            )
        },
        {
            "division",
            new BinaryExpression(
                new LiteralExpression(2),
                new Token(TokenKind.Slash, "/", null, 1),
                new LiteralExpression(2)
            )
        },
        {
            "chainMultiplication",
            new BinaryExpression(
                new BinaryExpression(
                    new LiteralExpression(1),
                    new Token(TokenKind.Asterisk, "*", null, 1),
                    new LiteralExpression(2)
                ),
                new Token(TokenKind.Asterisk, "*", null, 1),
                new LiteralExpression(3)
            )
        },
        {
            "multiplication",
            new BinaryExpression(
                new LiteralExpression(2),
                new Token(TokenKind.Asterisk, "*", null, 1),
                new LiteralExpression(2)
            )
        },
        /* ------------------ */
        {
            "intLiteral", new LiteralExpression(2)
        },
        {
            "doubleLiteral", new LiteralExpression(3.4d)
        },
        {
            "stringLiteral", new LiteralExpression("hello birds")
        },
        {
            "charLiteral", new LiteralExpression('a')
        },
        {
            "true", new LiteralExpression(true)
        },
        {
            "false", new LiteralExpression(false)
        },
        {
            "grouping",
            new GroupingExpression(
                new BinaryExpression(
                    new LiteralExpression(2),
                    new Token(TokenKind.Plus, "+", null, 1),
                    new LiteralExpression(2)
                )
            )
        },
        {
            "raiseToPower", new BinaryExpression(
                new LiteralExpression(2),
                new Token(TokenKind.Hat, "^", null, 1),
                new LiteralExpression(2)
            )
        },
        {
            "raiseGroupingToPower",
            new BinaryExpression(
                new GroupingExpression(
                    new BinaryExpression(
                        new LiteralExpression(2),
                        new Token(TokenKind.Plus, "+", null, 1),
                        new LiteralExpression(2)
                    )
                ),
                new Token(TokenKind.Hat, "^", null, 1),
                new LiteralExpression(2)
            )
        },
        {
            "raiseToGrouping",
            new BinaryExpression(
                new LiteralExpression(2),
                new Token(TokenKind.Hat, "^", null, 1),
                new GroupingExpression(
                    new BinaryExpression(
                        new LiteralExpression(2),
                        new Token(TokenKind.Plus, "+", null, 1),
                        new LiteralExpression(2)
                    )
                )
            )
        },
        {
            "chainPower",
            new BinaryExpression(
                new BinaryExpression(
                    new BinaryExpression(
                        new LiteralExpression(2),
                        new Token(TokenKind.Hat, "^", null, 1),
                        new LiteralExpression(2)
                    ),
                    new Token(TokenKind.Hat, "^", null, 1),
                    new LiteralExpression(2)
                ),
                new Token(TokenKind.Hat, "^", null, 1),
                new LiteralExpression(2)
            )
        },
        {
            "unaryMinus",
            new UnaryExpression(
                new Token(TokenKind.Minus, "-", null, 1),
                new LiteralExpression(123)
            )
        },
        {
            "unaryNot",
            new UnaryExpression(
                new Token(TokenKind.Not, "!", null, 1),
                new LiteralExpression(false)
            )
        },
        {
            "unaryGrouping",
            new UnaryExpression(
                new Token(TokenKind.Minus, "-", null, 1),
                new GroupingExpression(
                    new BinaryExpression(
                        new LiteralExpression(2),
                        new Token(TokenKind.Plus, "+", null, 1),
                        new LiteralExpression(2)
                    )
                )
            )
        }

        #endregion
    };
}