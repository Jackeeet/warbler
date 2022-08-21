using System.Diagnostics;
using Warbler.Environment;
using Warbler.ErrorReporting;
using Warbler.Interpreter;
using Warbler.Parser;
using Warbler.Scanner;
using Warbler.TypeChecker;
using Warbler.Utils.Id;

namespace Warbler;

public class Warbler
{
    private readonly IErrorReporter _errorReporter = new ConsoleReporter();
    private readonly WarblerEnvironment _globalEnvironment = new();

    public void RunFile(string path)
    {
        using (var reader = new StreamReader(path))
        {
            Run(reader.ReadToEnd());
        }

        if (_errorReporter.HadError)
        {
            System.Environment.Exit(1);
        }
    }

    public void RunInteractive()
    {
        while (true)
        {
            Console.Write(@":> ");
            var input = Console.ReadLine();
            if (input is null or "exit")
            {
                break;
            }

            Run(input);
            _errorReporter.Reset();
        }
    }

    private void Run(string input)
    {
        var scanner = new WarblerScanner(input, _errorReporter);
        var tokens = scanner.Scan();

        var parser = new WarblerParser(tokens, _errorReporter, new DefaultIdProvider());
        var expressions = parser.Parse();
        if (_errorReporter.HadError)
            return;

        var checker = new WarblerChecker(_errorReporter, _globalEnvironment);
        foreach (var expression in expressions)
        {
            Debug.Assert(expression != null, nameof(expression) + " != null");
            checker.CheckTypes(expression);
        }

        if (_errorReporter.HadError || _errorReporter.HadRuntimeError)
            return;

        var interpreter = new WarblerInterpreter(_errorReporter, _globalEnvironment);
        foreach (var expression in expressions)
        {
            Debug.Assert(expression != null, nameof(expression) + " != null");
            var value = interpreter.Interpret(expression);
            if (value is not null)
                Console.WriteLine(value);
        }
    }
}