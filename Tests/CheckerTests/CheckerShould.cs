using NUnit.Framework;
using Tests.Mocks;
using Warbler.TypeChecker;

namespace Tests.CheckerTests;

public class CheckerShould
{
    private TestReporter _errorReporter = null!;

    [OneTimeSetUp]
    public void BeforeFixture()
    {
        _errorReporter = new TestReporter();
    }

    [SetUp]
    public void BeforeTest()
    {
        _errorReporter.Reset();
    }

    [Test]
    [TestCaseSource(typeof(UnaryExpressionsData), nameof(UnaryExpressionsData.ValidNames))]
    public void CheckValidUnaryExpressions(string inputName)
    {
        var checker = new WarblerChecker(_errorReporter);
        var expectedExpression = UnaryExpressionsData.Outputs[inputName];

        var actualExpression = UnaryExpressionsData.Inputs[inputName];
        var checkResult = checker.CheckTypes(actualExpression);

        Assert.IsTrue(checkResult);
        Assert.AreEqual(expectedExpression, actualExpression);
    }

    [Test]
    [TestCaseSource(typeof(BinaryExpressionsData), nameof(BinaryExpressionsData.ValidNames))]
    public void CheckValidBinaryExpressions(string inputName)
    {
        var checker = new WarblerChecker(_errorReporter);
        var expectedExpression = BinaryExpressionsData.Outputs[inputName];

        var actualExpression = BinaryExpressionsData.Inputs[inputName];
        var checkResult = checker.CheckTypes(actualExpression);

        Assert.IsTrue(checkResult);
        Assert.AreEqual(expectedExpression, actualExpression);
    }


    [Test]
    [TestCaseSource(typeof(TernaryExpressionsData), nameof(TernaryExpressionsData.ValidNames))]
    public void CheckValidTernaryExpressions(string inputName)
    {
        var checker = new WarblerChecker(_errorReporter);
        var expectedExpression = TernaryExpressionsData.Outputs[inputName];

        var actualExpression = TernaryExpressionsData.Inputs[inputName];
        var checkResult = checker.CheckTypes(actualExpression);

        Assert.IsTrue(checkResult);
        Assert.AreEqual(expectedExpression, actualExpression);
    }


    [Test]
    public void CheckInvalidLiteralExpression()
    {
    }

    [Test]
    public void CheckInvalidUnaryExpressions()
    {
    }

    [Test]
    public void CheckInvalidBinaryExpressions()
    {
    }

    [Test]
    [TestCaseSource(typeof(TernaryExpressionsData), nameof(TernaryExpressionsData.InvalidNames))]
    public void CheckInvalidTernaryExpressions(string inputName)
    {
        var checker = new WarblerChecker(_errorReporter);

        var expression = TernaryExpressionsData.Inputs[inputName];
        var checkResult = checker.CheckTypes(expression);

        Assert.IsFalse(checkResult);
        Assert.IsTrue(_errorReporter.HadError);
    }
}