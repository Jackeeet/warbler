using System.Reflection;
using Newtonsoft.Json;
using Warbler.Expressions;
using Warbler.Localisation;

namespace Warbler.ErrorReporting;

public class ConsoleReporter : IErrorReporter
{
    public bool HadError { get; set; }

    public Language Language { get; set; }

    private Dictionary<string, Dictionary<string, string>> _errorMessages;

    public ConsoleReporter(Language language)
    {
        Language = language;
        var path = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(),
            $"Localisation\\{Language}.json"
        );

        using var reader = new StreamReader(path);
        var json = reader.ReadToEnd();
        var values =
            JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(json);
        if (values is null || !values.ContainsKey("errors"))
            throw new Exception("No localised error message data provided");
        _errorMessages = values["errors"];
    }

    public void ErrorAtLine(int line, string messageKey)
    {
        Report(line, "", messageKey);
    }

    public void ErrorAtToken(Token token, string messageKey)
    {
        Report(
            token.LineNumber,
            // todo translate location
            token.Kind == TokenKind.Eof ? " at the end of input" : $" at \"{token.Lexeme}\"",
            messageKey
        );
    }

    public void ErrorAtExpression(Expression expression, string messageKey)
    {
        Report(expression.Line, "", messageKey);
    }

    private void Report(int line, string atLocation, string messageKey)
    {
        var messageKeys = messageKey.Split('.');
        if (messageKeys.Length < 2)
            throw new Exception("expected at least 2 key parts");

        var errorType = messageKeys[0];
        if (!_errorMessages.ContainsKey(errorType))
            throw new Exception($"invalid error type {errorType}");

        var errorMessage = messageKeys[1];
        if (!_errorMessages[errorType].ContainsKey(errorMessage))
            throw new Exception($"unexpected message key {errorMessage}");
        var message = _errorMessages[errorType][errorMessage];

        HadError = true;
        // todo translate line + error
        Console.WriteLine($"[Line {line}] Error{atLocation}: {message}.");
    }
}