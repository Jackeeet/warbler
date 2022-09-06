using System.Globalization;
using CommandLine;
using Warbler.Resources.Common;

namespace Warbler;

internal static class Program
{
    private class Options
    {
        // todo translate helptexts
        [Option('v', "verbose", Required = false, HelpText = "Print all evaluation results.")]
        public bool Verbose { get; set; }

        [Option('l', "language", Required = false,
            HelpText = "Set message language. Supported options:\n" +
                       ":>    en (English, default option)\n" +
                       ":>    ru (Russian)")]
        public string? Language { get; set; }

        [Option('f', "file", Required = false, HelpText = "Run the specified file.")]
        public string? FilePath { get; set; }
    }

    private static void Main(string[] args)
    {
        CommandLine.Parser.Default.ParseArguments<Options>(args)
            .WithParsed(RunOptions)
            .WithNotParsed(HandleParseError);
    }

    private static void RunOptions(Options options)
    {
        SetCultureInfo(options);
        var warbler = new Warbler();

        if (string.IsNullOrEmpty(options.FilePath))
        {
            Console.WriteLine(Common.InterpreterInfo);
            Console.WriteLine(Common.InterpreterInfoExit);
            warbler.RunInteractive(options.Verbose);
        }
        else
        {
            warbler.RunFile(options.FilePath, options.Verbose);
        }
    }

    private static void SetCultureInfo(Options options)
    {
        var cultureInfo = new CultureInfo(options.Language ?? "en");
        Common.Culture = cultureInfo;
        Resources.Errors.Checker.Culture = cultureInfo;
        Resources.Errors.Syntax.Culture = cultureInfo;
        Resources.Errors.Runtime.Culture = cultureInfo;
    }

    private static void HandleParseError(IEnumerable<Error> errors)
    {
    }
}