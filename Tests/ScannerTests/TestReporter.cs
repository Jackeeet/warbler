using Warbler.ErrorReporting;

namespace Tests.ScannerTests;

public class TestReporter : IErrorReporter
{
    public bool HadError { get; set; }

    public string? ErrorMessage { get; set; }

    public void ErrorAtLine(int line, string message)
    {
        HadError = true;
        ErrorMessage = $"[Line {line}] Error: {message}.";
    }

    public void Reset()
    {
        HadError = false;
        ErrorMessage = null;
    }
}