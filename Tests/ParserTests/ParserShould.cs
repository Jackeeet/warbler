using System;
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
    [TestCaseSource(typeof(Basic), nameof(Basic.ValidNames))]
    public void ParseValidBasicExpressions(string inputName)
    {
        var tokens = new WarblerScanner(Basic.Inputs[inputName], _errorReporter).Scan();
        _errorReporter.Reset();
        var parser = new WarblerParser(tokens, _errorReporter, new TestGuidProvider());
        var expected = Basic.Outputs[inputName];

        var actual = parser.Parse();

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(Variable), nameof(Variable.ValidNames))]
    public void ParseValidVariableExpressions(string inputName)
    {
        var tokens = new WarblerScanner(Variable.Inputs[inputName], _errorReporter).Scan();
        _errorReporter.Reset();
        var parser = new WarblerParser(tokens, _errorReporter, new TestGuidProvider());
        var expected = Variable.Outputs[inputName];

        var actual = parser.Parse();

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(Block), nameof(Block.ValidNames))]
    public void ParseValidBlockExpressions(string inputName)
    {
        var tokens = new WarblerScanner(Block.Inputs[inputName], _errorReporter).Scan();
        _errorReporter.Reset();
        var parser = new WarblerParser(tokens, _errorReporter, new TestGuidProvider());
        var expected = Block.Outputs[inputName];

        var actual = parser.Parse();

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(Conditional), nameof(Conditional.ValidNames))]
    public void ParseValidConditionalExpressions(string inputName)
    {
        var tokens = new WarblerScanner(Conditional.Inputs[inputName], _errorReporter).Scan();
        _errorReporter.Reset();
        var parser = new WarblerParser(tokens, _errorReporter, new TestGuidProvider());
        var expected = Conditional.Outputs[inputName];

        var actual = parser.Parse();

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(WhileLoop), nameof(WhileLoop.ValidNames))]
    public void ParseValidWhileLoops(string inputName)
    {
        var tokens = new WarblerScanner(WhileLoop.Inputs[inputName], _errorReporter).Scan();
        _errorReporter.Reset();
        var parser = new WarblerParser(tokens, _errorReporter, new TestGuidProvider());
        var expected = WhileLoop.Outputs[inputName];

        var actual = parser.Parse();

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(Function), nameof(Function.ValidNames))]
    public void ParseValidFunctions(string inputName)
    {
        var tokens = new WarblerScanner(Function.Inputs[inputName], _errorReporter).Scan();
        _errorReporter.Reset();
        var parser = new WarblerParser(tokens, _errorReporter, new TestGuidProvider());
        var expected = Function.Outputs[inputName];

        var actual = parser.Parse();

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(Call), nameof(Call.ValidNames))]
    public void ParseValidCallExpressions(string inputName)
    {
        var tokens = new WarblerScanner(Call.Inputs[inputName], _errorReporter).Scan();
        _errorReporter.Reset();
        var parser = new WarblerParser(tokens, _errorReporter, new TestGuidProvider());
        var expected = Call.Outputs[inputName];

        var actual = parser.Parse();

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(Basic), nameof(Basic.InvalidNames))]
    public void ThrowErrorOnInvalidBasics(string inputName)
    {
        var tokens = new WarblerScanner(Basic.Inputs[inputName], _errorReporter).Scan();
        _errorReporter.Reset();
        var parser = new WarblerParser(tokens, _errorReporter, new TestGuidProvider());
        // the way the parser is used right now the only thing that matters
        // is that an error gets raised
        // and the actual list contents don't matter
        _ = parser.Parse();

        // Assert.Contains(null, actual);
        Assert.IsTrue(_errorReporter.HadError);
    }

    [Test]
    [TestCaseSource(typeof(Variable), nameof(Variable.InvalidNames))]
    public void ThrowErrorOnInvalidVarExpressions(string inputName)
    {
        var tokens = new WarblerScanner(Variable.Inputs[inputName], _errorReporter).Scan();
        _errorReporter.Reset();
        var parser = new WarblerParser(tokens, _errorReporter, new TestGuidProvider());

        _ = parser.Parse();

        Assert.IsTrue(_errorReporter.HadError);
    }

    [Test]
    [TestCaseSource(typeof(Block), nameof(Block.InvalidNames))]
    public void ThrowErrorOnInvalidBlocks(string inputName)
    {
        var tokens = new WarblerScanner(Block.Inputs[inputName], _errorReporter).Scan();
        _errorReporter.Reset();
        var parser = new WarblerParser(tokens, _errorReporter, new TestGuidProvider());

        _ = parser.Parse();

        Assert.IsTrue(_errorReporter.HadError);
    }
}