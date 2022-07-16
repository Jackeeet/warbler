using Warbler.Expressions;
using Warbler.Parser;
using Warbler.Scanner;

namespace Warbler.ErrorReporting;

public class ConsoleReporter : IErrorReporter
{
    public bool HadError { get; set; }

    public void ErrorAtLine(int line, string message)
    {
        Report(line, "", message);
    }

    public void ErrorAtToken(Token token, string message)
    {
        Report(
            token.LineNumber,
            token.Kind == TokenKind.Eof ? " at the end of input" : $" at \"{token.Lexeme}\"",
            message
        );
    }

    public void ErrorAtExpression(Expression expression, string message)
    {
        Report(expression.Line, "", message);
    }

    private void Report(int line, string atLocation, string message)
    {
        HadError = true;
        Console.WriteLine($"[Line {line}] Error{atLocation}: {message}.");
    }
}