using Warbler.Errors;
using Warbler.Expressions;
using Warbler.Resources.Common;

namespace Warbler.ErrorReporting;

public class ConsoleReporter : IErrorReporter
{
    public bool HadError { get; set; }
    public bool HadRuntimeError { get; set; }

    public void Reset()
    {
        HadError = false;
        HadRuntimeError = false;
    }

    public void ErrorAtLine(int line, string message)
    {
        Report(line, "", message);
    }

    public void ErrorAtToken(Token token, string message)
    {
        Report(
            token.LineNumber,
            token.Kind == TokenKind.Eof ? Common.AtInputEnd : string.Format(Common.AtLocation, token.Lexeme),
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
        Console.WriteLine(Common.RuntimeError, error.Message, error.Token.LineNumber);
    }

    private void Report(int line, string atLocation, string message)
    {
        HadError = true;
        Console.WriteLine(Common.ErrorLine, line, atLocation, message);
    }
}