using Warbler.ErrorReporting;
using Warbler.Scanner;

namespace Warbler.Parser;

public class WarblerParser
{
    private readonly List<Token> _tokens;
    private readonly IErrorReporter _errorReporter;
    private int _current;

    public WarblerParser(List<Token> tokens, IErrorReporter errorReporter)
    {
        _tokens = tokens;
        _errorReporter = errorReporter;
        _current = 0;
    }

    public Expression? Parse()
    {
        try
        {
            var expression = ParseExpression();
            if (!IsAtEnd)
                throw HandleParseError(CurrentToken, $"Unexpected token {CurrentToken}");
            return expression;
        }
        catch (ParseError)
        {
            return null;
        }
    }

    private Expression ParseExpression()
    {
        var expression = ParseEquality();
        if (Matching(TokenKind.Question))
        {
            var thenExpression = ParseExpression();
            Consume(TokenKind.Colon, "Expected a : ");
            var elseExpression = ParseExpression();

            expression = new TernaryExpression(expression, thenExpression, elseExpression);
        }

        return expression;
    }

    private Expression ParseEquality()
    {
        var expression = ParseRelational();

        if (Matching(TokenKind.DoubleEqual, TokenKind.NotEqual))
        {
            var op = PreviousToken;
            var rightExpression = ParseRelational();
            expression = new BinaryExpression(expression, op, rightExpression);
        }

        return expression;
    }

    private Expression ParseRelational()
    {
        var expression = ParseAdditive();

        if (Matching(TokenKind.GreaterThan, TokenKind.GreaterEqual, TokenKind.LessThan, TokenKind.LessEqual))
        {
            var op = PreviousToken;
            var rightExpression = ParseAdditive();
            expression = new BinaryExpression(expression, op, rightExpression);
        }

        return expression;
    }

    private Expression ParseAdditive()
    {
        var expression = ParseMultiplicative();

        while (Matching(TokenKind.Plus, TokenKind.Minus, TokenKind.DoublePlus))
        {
            var op = PreviousToken;
            var rightExpression = ParseMultiplicative();
            expression = new BinaryExpression(expression, op, rightExpression);
        }

        return expression;
    }

    private Expression ParseMultiplicative()
    {
        var expression = ParseUnary();

        while (Matching(TokenKind.Asterisk, TokenKind.Slash, TokenKind.Modulo))
        {
            var op = PreviousToken;
            var rightExpression = ParseUnary();
            expression = new BinaryExpression(expression, op, rightExpression);
        }

        return expression;
    }

    private Expression ParseUnary()
    {
        if (Matching(TokenKind.Minus, TokenKind.Not))
        {
            var op = PreviousToken;
            var rightExpression = ParseUnary();
            return new UnaryExpression(op, rightExpression);
        }

        return ParsePower();
    }

    private Expression ParsePower()
    {
        var expression = ParsePrimary();

        while (Matching(TokenKind.Hat))
        {
            var op = PreviousToken;
            var rightExpression = ParsePrimary();
            expression = new BinaryExpression(expression, op, rightExpression);
        }

        return expression;
    }

    private Expression ParsePrimary()
    {
        if (Matching(TokenKind.True))
        {
            return new LiteralExpression(true);
        }

        if (Matching(TokenKind.False))
        {
            return new LiteralExpression(false);
        }

        if (Matching(TokenKind.IntLiteral, TokenKind.DoubleLiteral,
                TokenKind.StringLiteral, TokenKind.CharLiteral))
        {
            return new LiteralExpression(PreviousToken.Literal);
        }

        if (Matching(TokenKind.LeftBracket))
        {
            var expression = ParseExpression();
            Consume(TokenKind.RightBracket, "Expected a ) after an expression");
            // could probably return the expression itself without wrapping it into a GroupingExpression here
            return new GroupingExpression(expression); 
        }

        throw HandleParseError(CurrentToken, "Expected an expression");
    }

    private bool Matching(params TokenKind[] kinds)
    {
        foreach (var kind in kinds)
        {
            if (HasKind(kind))
            {
                NextToken();
                return true;
            }
        }

        return false;
    }

    private bool HasKind(TokenKind kind)
    {
        if (IsAtEnd)
            return false;

        return CurrentToken.Kind == kind;
    }

    private Token NextToken()
    {
        if (!IsAtEnd)
            _current++;

        return PreviousToken;
    }

    private Token Consume(TokenKind kind, string message)
    {
        if (HasKind(kind))
            return NextToken();

        throw HandleParseError(CurrentToken, message);
    }

    private ParseError HandleParseError(Token token, string message)
    {
        _errorReporter.ErrorAtToken(token, message);
        // return instead of throw because some parsing errors are non-critical
        // and shouldn't stop the entire process
        // this way all errors will be reported but only those that require action
        // will be thrown
        return new ParseError();
    }

    private void Synchronise()
    {
        NextToken();
        while (!IsAtEnd)
        {
            switch (CurrentToken.Kind)
            {
                case TokenKind.Case:
                case TokenKind.Char:
                case TokenKind.Def:
                case TokenKind.Double:
                case TokenKind.For:
                case TokenKind.ForEach:
                case TokenKind.Fun:
                case TokenKind.In:
                case TokenKind.Inst:
                case TokenKind.Int:
                case TokenKind.Of:
                case TokenKind.Print:
                case TokenKind.Ret:
                case TokenKind.String:
                case TokenKind.Comma:
                    return;
            }

            NextToken();
        }
    }

    private Token CurrentToken => _tokens[_current];

    private Token PreviousToken => _tokens[_current - 1];

    private bool IsAtEnd => CurrentToken.Kind == TokenKind.Eof;
}