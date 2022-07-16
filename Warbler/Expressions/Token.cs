using Warbler.Scanner;

namespace Warbler.Expressions;

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
        if (Literal is null)
            return $"{Kind} \"{Lexeme}\" (line {LineNumber})";

        return $"{Kind} \"{Lexeme}\" {Literal} (line {LineNumber})";
    }

    public override bool Equals(object? obj)
    {
        return obj is Token other && Equals(other);
    }

    protected bool Equals(Token other)
    {
        return Kind == other.Kind &&
               Lexeme == other.Lexeme &&
               Equals(Literal, other.Literal) &&
               LineNumber == other.LineNumber;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)Kind, Lexeme, Literal, LineNumber);
    }
}