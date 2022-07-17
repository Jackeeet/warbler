using System;
using NuGet.Frameworks;
using NUnit.Framework;
using Tests.Mocks;
using Warbler.Expressions;
using Warbler.TypeChecker;

namespace Tests.CheckerTests;

public class CheckerShould
{
    private TestReporter _errorReporter = null!;
    private WarblerChecker _checker;

    [OneTimeSetUp]
    public void BeforeFixture()
    {
        _errorReporter = new TestReporter();
    }

    [SetUp]
    public void BeforeTest()
    {
        _errorReporter.Reset();
        _checker = new WarblerChecker(_errorReporter);
    }

    [Test]
    [TestCaseSource(typeof(UnaryExpressionsData), nameof(UnaryExpressionsData.ValidNames))]
    public void CheckValidUnaryExpressions(string inputName)
    {
        var expectedExpression = UnaryExpressionsData.Outputs[inputName];

        var actualExpression = UnaryExpressionsData.Inputs[inputName];
        var checkResult = _checker.CheckTypes(actualExpression);

        Assert.IsTrue(checkResult);
        Assert.AreEqual(expectedExpression, actualExpression);
    }

    [Test]
    [TestCaseSource(typeof(BinaryExpressionsData), nameof(BinaryExpressionsData.ValidNames))]
    public void CheckValidBinaryExpressions(string inputName)
    {
        var expectedExpression = BinaryExpressionsData.Outputs[inputName];

        var actualExpression = BinaryExpressionsData.Inputs[inputName];
        var checkResult = _checker.CheckTypes(actualExpression);

        Assert.IsTrue(checkResult);
        Assert.AreEqual(expectedExpression, actualExpression);
    }


    [Test]
    [TestCaseSource(typeof(TernaryExpressionsData), nameof(TernaryExpressionsData.ValidNames))]
    public void CheckValidTernaryExpressions(string inputName)
    {
        var expectedExpression = TernaryExpressionsData.Outputs[inputName];

        var actualExpression = TernaryExpressionsData.Inputs[inputName];
        var checkResult = _checker.CheckTypes(actualExpression);

        Assert.IsTrue(checkResult);
        Assert.AreEqual(expectedExpression, actualExpression);
    }


    [Test]
    public void CheckInvalidLiteralExpression()
    {
        var checkResult = _checker.CheckTypes(new LiteralExpression(2) { Line = 1 });

        Assert.IsFalse(checkResult);
        Assert.IsTrue(_errorReporter.HadError);
    }

    [Test]
    [TestCaseSource(typeof(UnaryExpressionsData), nameof(UnaryExpressionsData.InvalidNames))]
    public void CheckInvalidUnaryExpressions(string inputName)
    {
        var expression = UnaryExpressionsData.Inputs[inputName];
        var checkResult = _checker.CheckTypes(expression);

        Assert.IsFalse(checkResult);
        Assert.IsTrue(_errorReporter.HadError);
    }

    [Test]
    [TestCaseSource(typeof(BinaryExpressionsData), nameof(BinaryExpressionsData.InvalidNames))]
    public void CheckInvalidBinaryExpressions(string inputName)
    {
        var expression = BinaryExpressionsData.Inputs[inputName];
        var checkResult = _checker.CheckTypes(expression);

        Assert.IsFalse(checkResult);
        Assert.IsTrue(_errorReporter.HadError);
    }

    [Test]
    [TestCaseSource(typeof(TernaryExpressionsData), nameof(TernaryExpressionsData.InvalidNames))]
    public void CheckInvalidTernaryExpressions(string inputName)
    {
        var expression = TernaryExpressionsData.Inputs[inputName];
        var checkResult = _checker.CheckTypes(expression);

        Assert.IsFalse(checkResult);
        Assert.IsTrue(_errorReporter.HadError);
    }

    [Test]
    public void ThrowOnUnexpectedUnaryOp()
    {
        Assert.Throws<ArgumentException>(() => _checker.CheckTypes(
                new UnaryExpression(
                    new Token(TokenKind.Plus, "+", null, 1),
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            )
        );
    }

    [Test]
    public void ThrowOnUnexpectedBinaryOp()
    {
        Assert.Throws<ArgumentException>(() => _checker.CheckTypes(
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.Question, "?", null, 1),
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            )
        );
    }
}