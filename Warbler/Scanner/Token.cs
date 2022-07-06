namespace Warbler.Scanner;

public class Token
{
    public TokenKind Kind { get; }
    public string Lexeme { get; }
    public object? Literal { get; }
    public int LineNumber { get; }

    public Token(TokenKind kind, string lexeme, object? literal, int lineNumber)
    {
        Kind = kind;
        Lexeme = lexeme;
        Literal = literal;
        LineNumber = lineNumber;
    }

    public override string ToString()
    {
        return $"[{LineNumber}] {Kind} {Lexeme} {Literal}";
    }
}