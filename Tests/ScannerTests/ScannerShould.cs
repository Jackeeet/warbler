using System.Collections.Generic;
using NUnit.Framework;
using Tests.Mocks;
using Warbler.Expressions;
using Warbler.Scanner;

namespace Tests.ScannerTests;

[TestFixture]
public class ScannerShould
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
    public void ScanValidInputs(string inputName)
    {
        var scanner = new WarblerScanner(inputs[inputName], _errorReporter);
        var expected = outputs[inputName];

        var actual = scanner.Scan();

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(nameof(invalidInputNames))]
    public void ScanInvalidInputs(string inputName)
    {
        var scanner = new WarblerScanner(inputs[inputName], _errorReporter);
        var expected = outputs[inputName];

        var actual = scanner.Scan();

        Assert.AreEqual(expected, actual);
        Assert.IsTrue(_errorReporter.HadError);
    }

    private static readonly List<string> validInputNames = new()
    {
        "empty",
        "int",
        "double",
        "string",
        "escapedCharString",
        "leadingPointNumber",
        "trailingPointNumber",
        "escapedQuoteString",
        "stringWithUnsupportedChar",
        "keyword",
        "cStyleComment",
        "haskellStyleComment",
        "multilineString",
        "functionParams",
        "controlFlow",
        "simpleSum",
        "simpleDifference",
        "simpleProduct",
        "simpleDivision",
        "negativeNum",
        "forloop",
        "stringConcatenation",
        "import",
        "logicExpression",
        "rangeCheck",
        "escapedCharLiteral",
        "plusEqual",
        "minusEqual",
        "asteriskEqual",
        "slashEqual",
        "leadingWhiteSpace",
        "trailingWhiteSpace",
        "true",
        "false"
    };

    private static readonly List<string> invalidInputNames = new()
    {
        "unterminatedString",
        "unknownEscapedString",
        "unsupportedInputChar",
        "inputEndApostrophe",
        "inputEndQuote",
        "emptyCharLiteral",
        "unknownEscapedChar",
        "unterminatedChar",
    };

    private static readonly Dictionary<string, string> inputs = new()
    {
        { "empty", "" },
        { "int", "1" },
        { "double", "1.245" },
        { "leadingPointNumber", ".245" },
        { "trailingPointNumber", "245." },
        { "string", "\"text\"" },
        { "unterminatedString", "\"text" },
        { "escapedCharString", "\"\\text\"" },
        { "unknownEscapedString", "\"t\\dxt\"" },
        { "escapedQuoteString", "\"t\\\"xt\"" },
        { "keyword", "ret \"ret\"" },
        { "cStyleComment", "\"before\" // comment\n \"after\"" },
        { "haskellStyleComment", "\"before\" -- comment\n \"after\"" },
        { "stringWithUnsupportedChar", "\"abcdf#2234\"" },
        { "unsupportedInputChar", "#" },
        { "multilineString", "\"first line\nsecond line\"" },
        { "functionParams", "\\ Run(int? first, bool second)" },
        { "controlFlow", "if 10 % 2 == 0 then :> 2 <: else :> 1 <:" },
        { "simpleSum", "2 + 3" },
        { "simpleDifference", "3 - 1" },
        { "negativeNum", "-1" },
        { "simpleProduct", "7*7" },
        { "simpleDivision", "10 / 2" },
        { "forloop", "for int i = 0; i < 5; i += 1" },
        { "stringConcatenation", "\"hello \" ++ \"birds\"" },
        { "import", "-> wb.IO" },
        { "logicExpression", "!(2 ^ 4 != 7)" },
        { "rangeCheck", "x >= 'a' and x <= 'z'" },
        { "inputEndApostrophe", "'" },
        { "inputEndQuote", "\"" },
        { "emptyCharLiteral", "''" },
        { "unknownEscapedChar", "'\\p'" },
        { "unterminatedChar", "'dd" },
        { "escapedCharLiteral", "'\\\\'" },
        { "plusEqual", "x += 1" },
        { "minusEqual", "x -= 1 " },
        { "asteriskEqual", "x *= 1" },
        { "slashEqual", "x /= 1" },
        { "leadingWhiteSpace", "    \t    def x" },
        { "trailingWhiteSpace", "def x     \t    " },
        { "true", "true" },
        { "false", "false" },
    };

    private static readonly Dictionary<string, List<Token>> outputs = new()
    {
        {
            "false", new List<Token>
            {
                new(TokenKind.False, "false", null, 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "true", new List<Token>
            {
                new(TokenKind.True, "true", null, 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "trailingWhiteSpace", new List<Token>
            {
                new(TokenKind.Def, "def", null, 1),
                new(TokenKind.Identifier, "x", null, 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "leadingWhiteSpace", new List<Token>
            {
                new(TokenKind.Def, "def", null, 1),
                new(TokenKind.Identifier, "x", null, 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "slashEqual", new List<Token>
            {
                new(TokenKind.Identifier, "x", null, 1),
                new(TokenKind.SlashEqual, "/=", null, 1),
                new(TokenKind.IntLiteral, "1", 1, 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "asteriskEqual", new List<Token>
            {
                new(TokenKind.Identifier, "x", null, 1),
                new(TokenKind.AsteriskEqual, "*=", null, 1),
                new(TokenKind.IntLiteral, "1", 1, 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "minusEqual", new List<Token>
            {
                new(TokenKind.Identifier, "x", null, 1),
                new(TokenKind.MinusEqual, "-=", null, 1),
                new(TokenKind.IntLiteral, "1", 1, 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "plusEqual", new List<Token>
            {
                new(TokenKind.Identifier, "x", null, 1),
                new(TokenKind.PlusEqual, "+=", null, 1),
                new(TokenKind.IntLiteral, "1", 1, 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "escapedCharLiteral", new List<Token>
            {
                new(TokenKind.CharLiteral, "'\\\\'", '\\', 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "unterminatedChar", new List<Token>
            {
                new(TokenKind.Identifier, "d", null, 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "unknownEscapedChar", new List<Token>
            {
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "emptyCharLiteral", new List<Token>
            {
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "inputEndApostrophe", new List<Token>
            {
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "inputEndQuote", new List<Token>
            {
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "rangeCheck", new List<Token>
            {
                new(TokenKind.Identifier, "x", null, 1),
                new(TokenKind.GreaterEqual, ">=", null, 1),
                new(TokenKind.CharLiteral, "'a'", 'a', 1),
                new(TokenKind.And, "and", null, 1),
                new(TokenKind.Identifier, "x", null, 1),
                new(TokenKind.LessEqual, "<=", null, 1),
                new(TokenKind.CharLiteral, "'z'", 'z', 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "logicExpression", new List<Token>
            {
                new(TokenKind.Not, "!", null, 1),
                new(TokenKind.LeftBracket, "(", null, 1),
                new(TokenKind.IntLiteral, "2", 2, 1),
                new(TokenKind.Hat, "^", null, 1),
                new(TokenKind.IntLiteral, "4", 4, 1),
                new(TokenKind.NotEqual, "!=", null, 1),
                new(TokenKind.IntLiteral, "7", 7, 1),
                new(TokenKind.RightBracket, ")", null, 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "import", new List<Token>
            {
                new(TokenKind.RightArrow, "->", null, 1),
                new(TokenKind.Identifier, "wb", null, 1),
                new(TokenKind.Dot, ".", null, 1),
                new(TokenKind.Identifier, "IO", null, 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "stringConcatenation", new List<Token>
            {
                new(TokenKind.StringLiteral, "\"hello \"", "hello ", 1),
                new(TokenKind.DoublePlus, "++", null, 1),
                new(TokenKind.StringLiteral, "\"birds\"", "birds", 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "forloop", new List<Token>
            {
                new(TokenKind.For, "for", null, 1),
                new(TokenKind.Int, "int", null, 1),
                new(TokenKind.Identifier, "i", null, 1),
                new(TokenKind.Equal, "=", null, 1),
                new(TokenKind.IntLiteral, "0", 0, 1),
                new(TokenKind.Semicolon, ";", null, 1),
                new(TokenKind.Identifier, "i", null, 1),
                new(TokenKind.LessThan, "<", null, 1),
                new(TokenKind.IntLiteral, "5", 5, 1),
                new(TokenKind.Semicolon, ";", null, 1),
                new(TokenKind.Identifier, "i", null, 1),
                new(TokenKind.PlusEqual, "+=", null, 1),
                new(TokenKind.IntLiteral, "1", 1, 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "negativeNum", new List<Token>
            {
                new(TokenKind.Minus, "-", null, 1),
                new(TokenKind.IntLiteral, "1", 1, 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "simpleDivision", new List<Token>
            {
                new(TokenKind.IntLiteral, "10", 10, 1),
                new(TokenKind.Slash, "/", null, 1),
                new(TokenKind.IntLiteral, "2", 2, 1),
                new(TokenKind.Eof, "", null, 1),
            }
        },
        {
            "simpleProduct", new List<Token>
            {
                new(TokenKind.IntLiteral, "7", 7, 1),
                new(TokenKind.Asterisk, "*", null, 1),
                new(TokenKind.IntLiteral, "7", 7, 1),
                new(TokenKind.Eof, "", null, 1),
            }
        },
        {
            "simpleDifference", new List<Token>
            {
                new(TokenKind.IntLiteral, "3", 3, 1),
                new(TokenKind.Minus, "-", null, 1),
                new(TokenKind.IntLiteral, "1", 1, 1),
                new(TokenKind.Eof, "", null, 1),
            }
        },
        {
            "simpleSum", new List<Token>
            {
                new(TokenKind.IntLiteral, "2", 2, 1),
                new(TokenKind.Plus, "+", null, 1),
                new(TokenKind.IntLiteral, "3", 3, 1),
                new(TokenKind.Eof, "", null, 1),
            }
        },
        {
            "controlFlow", new List<Token>
            {
                new(TokenKind.If, "if", null, 1),
                new(TokenKind.IntLiteral, "10", 10, 1),
                new(TokenKind.Percent, "%", null, 1),
                new(TokenKind.IntLiteral, "2", 2, 1),
                new(TokenKind.DoubleEqual, "==", null, 1),
                new(TokenKind.IntLiteral, "0", 0, 1),
                new(TokenKind.Then, "then", null, 1),
                new(TokenKind.RightBird, ":>", null, 1),
                new(TokenKind.IntLiteral, "2", 2, 1),
                new(TokenKind.LeftBird, "<:", null, 1),
                new(TokenKind.Else, "else", null, 1),
                new(TokenKind.RightBird, ":>", null, 1),
                new(TokenKind.IntLiteral, "1", 1, 1),
                new(TokenKind.LeftBird, "<:", null, 1),
                new(TokenKind.Eof, "", null, 1),
            }
        },
        {
            "functionParams", new List<Token>
            {
                new(TokenKind.Func, "\\", null, 1),
                new(TokenKind.Identifier, "Run", null, 1),
                new(TokenKind.LeftBracket, "(", null, 1),
                new(TokenKind.Int, "int", null, 1),
                new(TokenKind.Question, "?", null, 1),
                new(TokenKind.Identifier, "first", null, 1),
                new(TokenKind.Comma, ",", null, 1),
                new(TokenKind.Bool, "bool", null, 1),
                new(TokenKind.Identifier, "second", null, 1),
                new(TokenKind.RightBracket, ")", null, 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        { "empty", new List<Token> { new(TokenKind.Eof, "", null, 1) } },
        {
            "int", new List<Token>
            {
                new(TokenKind.IntLiteral, "1", 1, 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "double", new List<Token>()
            {
                new(TokenKind.DoubleLiteral, "1.245", 1.245d, 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "leadingPointNumber", new List<Token>
            {
                new(TokenKind.Dot, ".", null, 1),
                new(TokenKind.IntLiteral, "245", 245, 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "trailingPointNumber", new List<Token>
            {
                new(TokenKind.IntLiteral, "245", 245, 1),
                new(TokenKind.Dot, ".", null, 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "string", new List<Token>
            {
                new(TokenKind.StringLiteral, "\"text\"", "text", 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "unterminatedString", new List<Token>
            {
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "escapedCharString", new List<Token>
            {
                new(TokenKind.StringLiteral, "\"\\text\"", "\t" + "ext", 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "unknownEscapedString", new List<Token>
            {
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "keyword", new List<Token>
            {
                new(TokenKind.Ret, "ret", null, 1),
                new(TokenKind.StringLiteral, "\"ret\"", "ret", 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "cStyleComment", new List<Token>
            {
                new(TokenKind.StringLiteral, "\"before\"", "before", 1),
                new(TokenKind.StringLiteral, "\"after\"", "after", 2),
                new(TokenKind.Eof, "", null, 2)
            }
        },
        {
            "haskellStyleComment", new List<Token>
            {
                new(TokenKind.StringLiteral, "\"before\"", "before", 1),
                new(TokenKind.StringLiteral, "\"after\"", "after", 2),
                new(TokenKind.Eof, "", null, 2)
            }
        },
        {
            "stringWithUnsupportedChar", new List<Token>
            {
                new(TokenKind.StringLiteral, "\"abcdf#2234\"", "abcdf#2234", 1),
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "unsupportedInputChar", new List<Token>
            {
                new(TokenKind.Eof, "", null, 1)
            }
        },
        {
            "multilineString", new List<Token>
            {
                new(TokenKind.StringLiteral, "\"first line\nsecond line\"", "first line\nsecond line", 2),
                new(TokenKind.Eof, "", null, 2)
            }
        },
        {
            "escapedQuoteString", new List<Token>
            {
                new(TokenKind.StringLiteral, "\"t\\\"xt\"", "t\"xt", 1),
                new(TokenKind.Eof, "", null, 1)
            }
        }
    };
}