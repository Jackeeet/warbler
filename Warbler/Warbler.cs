using System.Diagnostics;
using Warbler.Checker;
using Warbler.Environment;
using Warbler.ErrorReporting;
using Warbler.Expressions;
using Warbler.Interpreter;
using Warbler.Parser;
using Warbler.Resolver;
using Warbler.Scanner;
using Warbler.Utils.Id;
using Warbler.Utils.Token;

namespace Warbler;

public class Warbler
{
    private readonly IErrorReporter _errorReporter = new ConsoleReporter();
    private readonly WarblerEnvironment _globalEnvironment = new();

    public void RunFile(string path, bool verbose)
    {
        using (var reader = new StreamReader(path))
        {
            Run(reader.ReadToEnd(), verbose);
        }

        if (_errorReporter.HadError)
        {
            System.Environment.Exit(1);
        }
    }

    public void RunInteractive(bool verbose)
    {
        while (true)
        {
            Console.Write(@":> ");
            var input = Console.ReadLine();
            if (input is null or "exit")
            {
                break;
            }

            Run(input, verbose);
            _errorReporter.Reset();
        }
    }

    public void RunExpressionsGenerator(string inPath, string outPath)
    {
        string input;
        using (var reader = new StreamReader(inPath))
        {
            input = reader.ReadToEnd();
        }

        var tokens = Scan(input);
        var exprs = new WarblerParser(
            tokens, new ConsoleReporter(), new DefaultIdProvider()).Parse();

        using (var writer = new StreamWriter(outPath))
        {
            foreach (var expr in exprs)
            {
                writer.WriteLine(expr.DefaultRepresentation());
                writer.WriteLine();
            }
        }
    }

    private void Run(string input, bool verbose)
    {
        var tokens = Scan(input);
        if (!Parse(tokens, out var expressions))
            return;
        if (!Check(expressions))
            return;
        if (!Resolve(expressions, out var resolvedLocals))
            return;
        Interpret(resolvedLocals, expressions, verbose);
    }

    private void Interpret(Dictionary<Expression, int?> resolvedLocals, List<Expression> expressions, bool verbose)
    {
        var interpreter = new WarblerInterpreter(_errorReporter, _globalEnvironment, resolvedLocals);
        foreach (var expression in expressions)
        {
            Debug.Assert(expression != null, nameof(expression) + " != null");
            var value = interpreter.Interpret(expression);
            var canPrint = value is not null && (verbose || Printable(expression));
            if (canPrint)
                Console.WriteLine(value);
        }
    }

    private bool Printable(Expression expression)
    {
        return expression is not FunctionDeclarationExpression &&
               expression is not VariableDeclarationExpression &&
               expression is not AssignmentExpression;
    }

    private bool Resolve(List<Expression> expressions, out Dictionary<Expression, int?> resolvedLocals)
    {
        var resolver = new WarblerResolver(_errorReporter);
        resolvedLocals = resolver.Resolve(expressions);
        return !_errorReporter.HadError;
    }

    private bool Check(List<Expression> expressions)
    {
        var checker = new WarblerChecker(_errorReporter, _globalEnvironment);
        foreach (var expression in expressions)
        {
            Debug.Assert(expression != null, nameof(expression) + " != null");
            checker.CheckTypes(expression);
        }

        return !_errorReporter.HadError && !_errorReporter.HadRuntimeError;
    }

    private bool Parse(List<Token> tokens, out List<Expression> expressions)
    {
        var parser = new WarblerParser(tokens, _errorReporter, new DefaultIdProvider());
        expressions = parser.Parse();
        return !_errorReporter.HadError;
    }

    private List<Token> Scan(string input)
    {
        var scanner = new WarblerScanner(input, _errorReporter);
        var tokens = scanner.Scan();
        return tokens;
    }
}