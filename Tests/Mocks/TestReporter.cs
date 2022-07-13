using Warbler.ErrorReporting;
using Warbler.Scanner;

namespace Tests.Mocks;

public class TestReporter : IErrorReporter
{
    public bool HadError { get; set; }

    public string? ErrorMessage { get; set; }

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

    public void Reset()
    {
        HadError = false;
        ErrorMessage = null;
    }

    private void Report(int line, string atLocation, string message)
    {
        HadError = true;
        ErrorMessage = $"[Line {line}] Error{atLocation}: {message}.";
    }
}