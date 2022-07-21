using Warbler.Expressions;
using Warbler.Parser;
using Warbler.Scanner;

namespace Warbler.ErrorReporting;

public interface IErrorReporter
{
    public bool HadError { get; set; }
    public void ErrorAtLine(int line, string messageKey);
    public void ErrorAtToken(Token token, string messageKey);
    public void ErrorAtExpression(Expression expression, string messageKey);
}