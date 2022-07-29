using NUnit.Framework;
using Tests.Mocks;
using Warbler.Parser;
using Warbler.Scanner;

namespace Tests.ParserTests;

[TestFixture]
public class ParserShould
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
    [TestCaseSource(typeof(BasicExpressionsData), nameof(BasicExpressionsData.ValidNames))]
    public void ParseValidBasicExpressions(string inputName)
    {
        var tokens = new WarblerScanner(BasicExpressionsData.Inputs[inputName], _errorReporter).Scan();
        _errorReporter.Reset();
        var parser = new WarblerParser(tokens, _errorReporter);
        var expected = BasicExpressionsData.Outputs[inputName];

        var actual = parser.Parse();

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(VariableExpressionsData), nameof(VariableExpressionsData.ValidNames))]
    public void ParseValidVariableExpressions(string inputName)
    {
        var tokens = new WarblerScanner(VariableExpressionsData.Inputs[inputName], _errorReporter).Scan();
        _errorReporter.Reset();
        var parser = new WarblerParser(tokens, _errorReporter);
        var expected = VariableExpressionsData.Outputs[inputName];

        var actual = parser.Parse();

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(BlockExpressionsData), nameof(BlockExpressionsData.ValidNames))]
    public void ParseValidBlockExpressions(string inputName)
    {
        var tokens = new WarblerScanner(BlockExpressionsData.Inputs[inputName], _errorReporter).Scan();
        _errorReporter.Reset();
        var parser = new WarblerParser(tokens, _errorReporter);
        var expected = BlockExpressionsData.Outputs[inputName];

        var actual = parser.Parse();

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(BasicExpressionsData), nameof(BasicExpressionsData.InvalidNames))]
    public void ThrowErrorOnInvalidBasics(string inputName)
    {
        var tokens = new WarblerScanner(BasicExpressionsData.Inputs[inputName], _errorReporter).Scan();
        _errorReporter.Reset();
        var parser = new WarblerParser(tokens, _errorReporter);
        // the way the parser is used right now the only thing that matters
        // is that an error gets raised
        // and the actual list contents don't matter
        _ = parser.Parse();

        // Assert.Contains(null, actual);
        Assert.IsTrue(_errorReporter.HadError);
    }

    [Test]
    [TestCaseSource(typeof(VariableExpressionsData), nameof(VariableExpressionsData.InvalidNames))]
    public void ThrowErrorOnInvalidVarExpressions(string inputName)
    {
        var tokens = new WarblerScanner(VariableExpressionsData.Inputs[inputName], _errorReporter).Scan();
        _errorReporter.Reset();
        var parser = new WarblerParser(tokens, _errorReporter);

        _ = parser.Parse();

        Assert.IsTrue(_errorReporter.HadError);
    }

    [Test]
    [TestCaseSource(typeof(BlockExpressionsData), nameof(BlockExpressionsData.InvalidNames))]
    public void ThrowErrorOnInvalidBlocks(string inputName)
    {
        var tokens = new WarblerScanner(BlockExpressionsData.Inputs[inputName], _errorReporter).Scan();
        _errorReporter.Reset();
        var parser = new WarblerParser(tokens, _errorReporter);

        _ = parser.Parse();

        Assert.IsTrue(_errorReporter.HadError);
    }
}