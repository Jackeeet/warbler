using Warbler.ErrorReporting;
using Warbler.Expressions;

namespace Tests.Mocks;

public class TestReporter : IErrorReporter
{
    public bool HadError { get; set; }

    public string? ErrorMessage { get; set; }

    public void ErrorAtLine(int line, string messageKey)
    {
        Report(line, "", messageKey);
    }

    public void ErrorAtToken(Token token, string messageKey)
    {
        Report(
            token.LineNumber,
            token.Kind == TokenKind.Eof ? " at the end of input" : $" at \"{token.Lexeme}\"",
            messageKey
        );
    }

    public void ErrorAtExpression(Expression expression, string messageKey)
    {
        Report(expression.Line, "", messageKey);
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