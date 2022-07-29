using Warbler.ErrorReporting;
using Warbler.Errors;
using Warbler.Expressions;

namespace Tests.Mocks;

public class TestReporter : IErrorReporter
{
    public bool HadError { get; set; }
    public bool HadRuntimeError { get; set; }

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

    public void ErrorAtExpression(Expression expression, string messageKey)
    {
        Report(expression.Line, "", messageKey);
    }

    public void ReportRuntimeError(RuntimeError error)
    {
        HadRuntimeError = true;
        ErrorMessage = error.Message;
    }

    public void Reset()
    {
        HadError = false;
        HadRuntimeError = false;
        ErrorMessage = null;
    }

    private void Report(int line, string atLocation, string message)
    {
        HadError = true;
        ErrorMessage = $"[Line {line}] Error{atLocation}: {message}.";
    }
}