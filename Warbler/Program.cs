using System.Text;
using Warbler.ErrorReporting;
using Warbler.Parser;
using Warbler.Scanner;

namespace Warbler;

internal class Program
{
    private static IErrorReporter _errorReporter = new ConsoleReporter();

    private static void Main(string[] args)
    {
        switch (args.Length)
        {
            case 0:
                RunInteractive();
                break;
            case 1:
                RunFile(args[0]);
                break;
            default:
                Console.WriteLine("Usage: warbler [file path]");
                break;
        }
    }

    private static void RunFile(string path)
    {
        var bytes = File.ReadAllBytes(Path.GetFullPath(path));
        Run(Encoding.Default.GetString(bytes));

        if (_errorReporter.HadError)
        {
            Environment.Exit(1);
        }
    }

    private static void RunInteractive()
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

    private static void Run(string input)
    {
        var scanner = new WarblerScanner(input, _errorReporter);
        var tokens = scanner.Scan();

        var parser = new WarblerParser(tokens, _errorReporter);
        var expression = parser.Parse();

        if (_errorReporter.HadError)
            return;
        
        Console.WriteLine($":> Successfully parsed {tokens.Count} tokens");
    }
}