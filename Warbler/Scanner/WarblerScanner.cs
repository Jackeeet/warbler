using System.Globalization;
using System.Text;
using Warbler.ErrorReporting;
using Warbler.Expressions;
using Syntax = Warbler.Resources.Errors.Syntax;

namespace Warbler.Scanner;

public class WarblerScanner
{
    private delegate void TokenHandler(char ch);

    private delegate bool TokenPredicate(char ch);

    private readonly List<Tuple<TokenPredicate, TokenHandler>> _tokenHandlers;

    private static readonly Dictionary<char, TokenKind> singleTokenChars = new()
    {
        { '%', TokenKind.Modulo },
        { '^', TokenKind.Hat },
        { ',', TokenKind.Comma },
        { '.', TokenKind.Dot },
        { ';', TokenKind.Semicolon },
        { '?', TokenKind.Question },
        { '(', TokenKind.LeftBracket },
        { ')', TokenKind.RightBracket },
    };

    private static readonly Dictionary<char, Tuple<char, TokenKind, TokenKind>> doubleTokenChars = new()
    {
        { '!', Tuple.Create('=', TokenKind.NotEqual, TokenKind.Not) },
        { '=', Tuple.Create('=', TokenKind.DoubleEqual, TokenKind.Equal) },
        { '*', Tuple.Create('=', TokenKind.AsteriskEqual, TokenKind.Asterisk) },
        { '/', Tuple.Create('=', TokenKind.SlashEqual, TokenKind.Slash) },
        { '>', Tuple.Create('=', TokenKind.GreaterEqual, TokenKind.GreaterThan) },
        { ':', Tuple.Create('>', TokenKind.RightBird, TokenKind.Colon) },
    };

    private static readonly Dictionary<char, Tuple<char, TokenKind, char, TokenKind, TokenKind>> tripleTokenChars =
        new()
        {
            { '+', Tuple.Create('=', TokenKind.PlusEqual, '+', TokenKind.DoublePlus, TokenKind.Plus) },
            { '-', Tuple.Create('=', TokenKind.MinusEqual, '>', TokenKind.RightArrow, TokenKind.Minus) },
            { '<', Tuple.Create('=', TokenKind.LessEqual, ':', TokenKind.LeftBird, TokenKind.LessThan) },
        };

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
        { "func", TokenKind.Func },
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

        _tokenHandlers = new List<Tuple<TokenPredicate, TokenHandler>>()
        {
            Tuple.Create<TokenPredicate, TokenHandler>(
                IsBlankChar, (_) => { }),
            Tuple.Create<TokenPredicate, TokenHandler>(
                CommentChar, (_) => SkipUntil('\n')),
            Tuple.Create<TokenPredicate, TokenHandler>(
                singleTokenChars.ContainsKey, (ch) => AddToken(singleTokenChars[ch])),
            Tuple.Create<TokenPredicate, TokenHandler>(
                doubleTokenChars.ContainsKey, AddDoubleToken),
            Tuple.Create<TokenPredicate, TokenHandler>(
                tripleTokenChars.ContainsKey, AddTripleToken),
            Tuple.Create<TokenPredicate, TokenHandler>(
                IsAlpha, (_) => AddIdentifierToken()),
            Tuple.Create<TokenPredicate, TokenHandler>(
                IsDigit, (_) => AddNumberToken()),
            Tuple.Create<TokenPredicate, TokenHandler>(
                (ch) => ch == '\'', (_) => AddCharToken()),
            Tuple.Create<TokenPredicate, TokenHandler>(
                (ch) => ch == '"', (_) => AddStringToken()),
            Tuple.Create<TokenPredicate, TokenHandler>(
                (ch) => ch == '\n', (_) => StartNewLine())
        };
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
        var ch = ReadNextChar();
        var handled = false;
        foreach (var handler in _tokenHandlers)
        {
            var matchesPredicate = handler.Item1;
            var addToken = handler.Item2;
            if (matchesPredicate(ch))
            {
                handled = true;
                addToken(ch);
                break;
            }
        }

        if (!handled)
        {
            _errorReporter.ErrorAtLine(_currentLine,
                string.Format(Syntax.UnexpectedChar, _input[_currentChar - 1], _currentLinePos - 1));
        }
    }

    private static bool IsBlankChar(char ch)
    {
        return ch == ' ' || ch == '\r' || ch == '\t';
    }

    private void AddDoubleToken(char ch)
    {
        var tokens = doubleTokenChars[ch];
        var nextChar = tokens.Item1;
        AddToken(NextCharMatching(nextChar) ? tokens.Item2 : tokens.Item3);
    }

    private bool CommentChar(char ch)
    {
        return (ch == '-' || ch == '/') && NextCharMatching(ch);
    }

    private void AddTripleToken(char ch)
    {
        var tokens = tripleTokenChars[ch];
        var tk = tokens.Item5;
        if (NextCharMatching(tokens.Item1))
            tk = tokens.Item2;
        else if (NextCharMatching(tokens.Item3))
            tk = tokens.Item4;

        AddToken(tk);
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
                if (!ReadEscapedChar(builder)) return;
            }

            builder.Append(ReadNextChar());
        }

        if (InputEnded())
        {
            _errorReporter.ErrorAtLine(_currentLine, string.Format(Syntax.UnterminatedString, _currentLinePos));
            return;
        }

        // read the terminating " without adding it to the builder
        ReadNextChar();
        AddToken(TokenKind.StringLiteral, builder.ToString());
    }

    private bool ReadEscapedChar(StringBuilder builder)
    {
        // read the \
        ReadNextChar();
        // read the (possibly) escaped char
        var esc = ReadNextChar();
        if (!escapedChars.ContainsKey(esc))
        {
            _errorReporter.ErrorAtLine(_currentLine, string.Format(Syntax.UnknownEscape, _currentLinePos));
            SkipUntil('"');
            return false;
        }

        builder.Append(escapedChars[esc]);
        return true;
    }

    private void AddCharToken()
    {
        if (InputEnded())
        {
            _errorReporter.ErrorAtLine(_currentLine, Syntax.ApostropheAtEof);
            return;
        }

        var ch = ReadNextChar();
        if (ch == '\'')
        {
            _errorReporter.ErrorAtLine(_currentLine, string.Format(Syntax.EmptyChar, _currentLinePos));
            return;
        }
        else if (ch == '\\')
        {
            var esc = ReadNextChar();
            if (!escapedChars.ContainsKey(esc))
            {
                _errorReporter.ErrorAtLine(_currentLine, string.Format(Syntax.UnknownEscape, _currentLinePos));
                SkipUntil('\'');
                return;
            }

            ch = escapedChars[esc];
        }

        if (Peek() != '\'' || InputEnded())
        {
            _errorReporter.ErrorAtLine(_currentLine, string.Format(Syntax.UnterminatedChar, _currentLinePos));
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