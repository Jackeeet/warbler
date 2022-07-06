namespace Warbler.ErrorReporting;

public interface IErrorReporter
{
    public bool HadError { get; set; }
    public void ErrorAtLine(int line, string message);
}