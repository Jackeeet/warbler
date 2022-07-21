using System.Text;
using Warbler.ErrorReporting;
using Warbler.Interpreter;
using Warbler.Localisation;
using Warbler.Parser;
using Warbler.Scanner;
using Warbler.TypeChecker;

namespace Warbler;

public class Warbler
{
    private readonly IErrorReporter _errorReporter = new ConsoleReporter(Language.En);

    public void RunFile(string path)
    {
        var bytes = File.ReadAllBytes(Path.GetFullPath(path));
        Run(Encoding.Default.GetString(bytes));

        if (_errorReporter.HadError)
        {
            Environment.Exit(1);
        }
    }

    public void RunInteractive()
    {
        while (true)
        {
            Console.Write(":> ");
            var input = Console.ReadLine();
            if (input is null || input == "exit")
            {
                break;
            }

            Run(input);
            _errorReporter.HadError = false;
        }
    }

    private void Run(string input)
    {
        var scanner = new WarblerScanner(input, _errorReporter);
        var tokens = scanner.Scan();

        var parser = new WarblerParser(tokens, _errorReporter);
        var expression = parser.Parse();
        if (expression is null || _errorReporter.HadError)
            return;

        var checker = new WarblerChecker(_errorReporter);
        checker.CheckTypes(expression);

        if (_errorReporter.HadError)
            return;

        var interpreter = new WarblerInterpreter(_errorReporter);
        interpreter.Interpret(expression);
    }
}