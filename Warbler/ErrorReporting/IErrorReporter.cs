using Warbler.Errors;
using Warbler.Expressions;
using Warbler.Utils.Token;

namespace Warbler.ErrorReporting;

public interface IErrorReporter
{
    public bool HadError { get; set; }
    public bool HadRuntimeError { get; set; }
    public void Reset();
    public void ErrorAtLine(int line, string message);
    public void ErrorAtToken(Token token, string message);
    public void ErrorAtExpression(Expression expression, string messageKey);
    public void ReportRuntimeError(RuntimeError error);
}