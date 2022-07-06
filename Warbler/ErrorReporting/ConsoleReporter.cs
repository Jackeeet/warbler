namespace Warbler.ErrorReporting;

public class ConsoleReporter : IErrorReporter
{
    public bool HadError { get; set; }

    public void ErrorAtLine(int line, string message)
    {
        Report(line, "", message);
    }

    private void Report(int line, string atLocation, string message)
    {
        HadError = true;
        Console.WriteLine($"[Line {line}] Error {atLocation}: {message}");
    }
}