using System.Globalization;
using System.Text;
using Warbler.ErrorReporting;
using Warbler.Expressions;

namespace Warbler.Scanner;

public class WarblerScanner
{
    private static readonly Dictionary<char, char> escapedChars = new()
    {
        { 'a', '\a' },
        { 'b', '\b' },
        { 't', '\t' },
        { 'r', '\r' },
        { 'v', '\v' },
        { 'f', '\f' },
        { 'n', '\n' },
        { '0', '\0' },
        { '\'', '\'' },
        { '"', '\"' },
        { '\\', '\\' }
    };

    private static readonly Dictionary<string, TokenKind> keywords = new()
    {
        { "if", TokenKind.If },
        { "then", TokenKind.Then },
        { "else", TokenKind.Else },
        { "case", TokenKind.Case },
        { "of", TokenKind.Of },
        { "while", TokenKind.While },
        { "for", TokenKind.For },
        { "foreach", TokenKind.ForEach },
        { "in", TokenKind.In },
        { "fun", TokenKind.Fun },
        { "def", TokenKind.Def },
        { "type", TokenKind.Type },
        { "inst", TokenKind.Inst },
        { "ret", TokenKind.Ret },
        { "print", TokenKind.Print },
        { "and", TokenKind.And },
        { "or", TokenKind.Or },
        { "true", TokenKind.True },
        { "false", TokenKind.False },
        { "bool", TokenKind.Bool },
        { "int", TokenKind.Int },
        { "double", TokenKind.Double },
        { "char", TokenKind.Char },
        { "string", TokenKind.String }
    };

    private readonly string _input;
    private readonly IErrorReporter _errorReporter;

    private readonly List<Token> _tokens;

    private int _tokenStart;
    private int _currentChar;
    private int _currentLine;
    private int _currentLinePos;

    public WarblerScanner(string input, IErrorReporter errorReporter)
    {
        _input = input;
        _errorReporter = errorReporter;
        _tokens = new List<Token>();
        _tokenStart = 0;
        _currentChar = 0;
        _currentLinePos = 0;
        _currentLine = 1;
    }

    public List<Token> Scan()
    {
        while (!InputEnded())
        {
            _tokenStart = _currentChar;
            NextToken();
        }

        _tokens.Add(new Token(TokenKind.Eof, "", null, _currentLine));
        return _tokens;
    }

    private void NextToken()
    {
        TokenKind tk;
        var ch = ReadNextChar();
        switch (ch)
        {
            case '%':
                AddToken(TokenKind.Modulo);
                break;
            case '^':
                AddToken(TokenKind.Hat);
                break;
            case ',':
                AddToken(TokenKind.Comma);
                break;
            case '.':
                AddToken(TokenKind.Dot);
                break;
            case ';':
                AddToken(TokenKind.Semicolon);
                break;
            case '?':
                AddToken(TokenKind.Question);
                break;
            case '(':
                AddToken(TokenKind.LeftBracket);
                break;
            case ')':
                AddToken(TokenKind.RightBracket);
                break;
            case '!':
                AddToken(NextCharMatching('=') ? TokenKind.NotEqual : TokenKind.Not);
                break;
            case '=':
                AddToken(NextCharMatching('=') ? TokenKind.DoubleEqual : TokenKind.Equal);
                break;
            case '*':
                AddToken(NextCharMatching('=') ? TokenKind.AsteriskEqual : TokenKind.Asterisk);
                break;
            case '+':
                tk = TokenKind.Plus;
                if (NextCharMatching('='))
                    tk = TokenKind.PlusEqual;
                else if (NextCharMatching('+'))
                    tk = TokenKind.DoublePlus;

                AddToken(tk);
                break;
            case '-':
                if (NextCharMatching('-'))
                {
                    SkipUntil('\n');
                    break;
                }

                tk = TokenKind.Minus;
                if (NextCharMatching('='))
                    tk = TokenKind.MinusEqual;
                else if (NextCharMatching('>'))
                    tk = TokenKind.RightArrow;
                AddToken(tk);
                break;
            case '/':
                if (NextCharMatching('/'))
                {
                    SkipUntil('\n');
                    break;
                }

                AddToken(NextCharMatching('=') ? TokenKind.SlashEqual : TokenKind.Slash);
                break;
            case '<':
                tk = TokenKind.LessThan;
                if (NextCharMatching(':'))
                    tk = TokenKind.LeftBird;
                else if (NextCharMatching('='))
                    tk = TokenKind.LessEqual;
                AddToken(tk);
                break;
            case '>':
                AddToken(NextCharMatching('=') ? TokenKind.GreaterEqual : TokenKind.GreaterThan);
                break;
            case ':':
                AddToken(NextCharMatching('>') ? TokenKind.RightBird : TokenKind.Colon);
                break;
            case '\'':
                AddCharToken();
                break;
            case '"':
                AddStringToken();
                break;
            case ' ':
            case '\r':
            case '\t':
                break;
            case '\n':
                StartNewLine();
                break;
            default:
                if (IsDigit(ch))
                {
                    AddNumberToken();
                }
                else if (IsAlpha(ch))
                {
                    AddIdentifierToken();
                }
                else
                {
                    _errorReporter.ErrorAtLine(
                        _currentLine,
                        $"Unexpected character: {_input[_currentChar - 1]} at position {_currentLinePos - 1}"
                    );
                }

                break;
        }
    }

    private void SkipUntil(char terminator)
    {
        while (Peek() != terminator && !InputEnded())
            ReadNextChar();
    }

    private void AddIdentifierToken()
    {
        while (IsAlphaNumeric(Peek()))
            ReadNextChar();

        var lexeme = _input.Substring(_tokenStart, _currentChar - _tokenStart);
        AddToken(keywords.ContainsKey(lexeme) ? keywords[lexeme] : TokenKind.Identifier);
    }

    private void AddNumberToken()
    {
        var isDouble = false;
        while (IsDigit(Peek()))
            ReadNextChar();

        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            isDouble = true;
            ReadNextChar();

            while (IsDigit(Peek()))
                ReadNextChar();
        }

        var value = _input.Substring(_tokenStart, _currentChar - _tokenStart);
        if (isDouble)
            AddToken(TokenKind.DoubleLiteral, double.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture));
        else
            AddToken(TokenKind.IntLiteral, int.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture));
    }

    private void AddStringToken()
    {
        var builder = new StringBuilder();
        while (Peek() != '"' && !InputEnded())
        {
            if (Peek() == '\n')
            {
                StartNewLine();
            }
            else if (Peek() == '\\')
            {
                // read the \
                ReadNextChar();
                // read the (possibly) escaped char
                var esc = ReadNextChar();
                if (!escapedChars.ContainsKey(esc))
                {
                    _errorReporter.ErrorAtLine(_currentLine, $"Unknown escape sequence at position {_currentLinePos}");
                    SkipUntil('"');
                    return;
                }

                builder.Append(escapedChars[esc]);
            }

            builder.Append(ReadNextChar());
        }

        if (InputEnded())
        {
            _errorReporter.ErrorAtLine(_currentLine, $"Expected a \" at position {_currentLinePos}");
            return;
        }

        // read the terminating " without adding it to the builder
        ReadNextChar();
        AddToken(TokenKind.StringLiteral, builder.ToString());
    }

    private void AddCharToken()
    {
        if (InputEnded())
        {
            _errorReporter.ErrorAtLine(_currentLine, $"Unexpected character \' at position {_currentLinePos}");
            return;
        }

        var ch = ReadNextChar();
        if (ch == '\'')
        {
            _errorReporter.ErrorAtLine(_currentLine, $"Empty character literal at position {_currentLinePos}");
            return;
        }
        else if (ch == '\\')
        {
            var esc = ReadNextChar();
            if (!escapedChars.ContainsKey(esc))
            {
                _errorReporter.ErrorAtLine(_currentLine, $"Unknown escape sequence at position {_currentLinePos}");
                SkipUntil('\'');
                return;
            }

            ch = escapedChars[esc];
        }

        if (Peek() != '\'' || InputEnded())
        {
            _errorReporter.ErrorAtLine(_currentLine, $"Expected a closing ' at position {_currentLinePos}");
            return;
        }

        // read the terminating '
        ReadNextChar();
        AddToken(TokenKind.CharLiteral, ch);
    }

    private char ReadNextChar()
    {
        _currentLinePos++;
        return _input[_currentChar++];
    }

    private void AddToken(TokenKind kind, object? value = null)
    {
        var lexeme = _input.Substring(_tokenStart, _currentChar - _tokenStart);
        _tokens.Add(new Token(kind, lexeme, value, _currentLine));
    }

    private bool NextCharMatching(char expected)
    {
        if (InputEnded() || _input[_currentChar] != expected)
            return false;

        _currentLinePos++;
        _currentChar++;
        return true;
    }

    private char Peek()
    {
        return InputEnded() ? '\0' : _input[_currentChar];
    }

    private char PeekNext()
    {
        return (_currentChar + 1 >= _input.Length) ? '\0' : _input[_currentChar + 1];
    }

    private bool InputEnded()
    {
        return _currentChar >= _input.Length;
    }

    private bool IsDigit(char ch)
    {
        return ch is >= '0' and <= '9';
    }

    private bool IsAlpha(char ch)
    {
        return ch is >= 'a' and <= 'z' || ch is >= 'A' and <= 'Z' || ch == '_';
    }

    private bool IsAlphaNumeric(char ch)
    {
        return IsAlpha(ch) || IsDigit(ch);
    }

    private void StartNewLine()
    {
        _currentLinePos = 0;
        _currentLine++;
    }
}