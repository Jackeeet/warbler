using System.Diagnostics;
using Warbler.ErrorReporting;
using Warbler.Errors;
using Warbler.Expressions;
using Warbler.Utils;
using Syntax = Warbler.Resources.Errors.Syntax;

namespace Warbler.Parser;

public class WarblerParser
{
    private readonly List<Token> _tokens;
    private readonly IErrorReporter _errorReporter;
    private readonly IGuidProvider _guidProvider;
    private int _current;

    private delegate Expression TopLevelExprHandler();

    private readonly Dictionary<TokenKind, TopLevelExprHandler> _topLevelExprHandlers;

    public WarblerParser(List<Token> tokens, IErrorReporter errorReporter, IGuidProvider guidProvider)
    {
        _tokens = tokens;
        _errorReporter = errorReporter;
        _guidProvider = guidProvider;
        _current = 0;

        _topLevelExprHandlers = new Dictionary<TokenKind, TopLevelExprHandler>()
        {
            { TokenKind.Func, ParseFunctionDeclaration },
            { TokenKind.While, ParseWhileLoop },
            { TokenKind.If, ParseConditional },
            { TokenKind.RightBird, ParseBlock }
        };
    }

    public List<Expression> Parse()
    {
        var expressions = new List<Expression>();
        while (!IsAtEnd)
        {
            try
            {
                expressions.Add(ParseProgram());
            }
            catch (ParseError)
            {
                Synchronise();
            }
        }

        return expressions;
    }

    private Expression ParseProgram()
    {
        var expressionKind = CurrentToken.Kind;
        return _topLevelExprHandlers.ContainsKey(expressionKind)
            ? _topLevelExprHandlers[expressionKind]()
            : ParseExpression();
    }

    private Expression ParseFunctionDeclaration()
    {
        NextToken();
        var line = CurrentToken.LineNumber;
        var name = Consume(TokenKind.Identifier, Syntax.ExpectedIdentifier);
        Consume(TokenKind.LeftBracket, Syntax.ExpectedOpeningBracket);

        var parameters = new List<Tuple<Token, Token>>();
        if (!HasKind(TokenKind.RightBracket))
        {
            do
            {
                var paramType = ConsumeAny(Syntax.ExpectedParameterType,
                    TokenKind.Int, TokenKind.Double, TokenKind.Bool, TokenKind.Char, TokenKind.String);
                var paramName = Consume(TokenKind.Identifier, Syntax.ExpectedParameterName);
                parameters.Add(Tuple.Create(paramType, paramName));
            } while (Matching(TokenKind.Comma));
        }

        Consume(TokenKind.RightBracket, Syntax.ExpectedClosingParamsBracket);
        var returnType = ConsumeAny(Syntax.ExpectedReturnType,
            TokenKind.Int, TokenKind.Double, TokenKind.Bool, TokenKind.Char, TokenKind.String);
        var body = (BlockExpression)ParseBlock();

        return new FunctionDefinitionExpression(name, parameters, returnType, body) { Line = line };
    }

    private Expression ParseWhileLoop()
    {
        var line = CurrentToken.LineNumber;
        NextToken();
        var condition = ParseBasicExpression();
        var actions = ParseBlock();

        return new WhileLoopExpression(condition, actions) { Line = line };
    }

    private Expression ParseConditional()
    {
        var line = CurrentToken.LineNumber;
        NextToken();
        var condition = ParseBasicExpression();
        Consume(TokenKind.Then, "Expected \"then\" after condition");
        var thenBranch = ParseBlock();
        var elseBranch = Matching(TokenKind.Else) ? ParseProgram() : null;

        return new ConditionalExpression(condition, thenBranch, elseBranch) { Line = line };
    }

    private Expression ParseBlock()
    {
        if (Matching(TokenKind.RightBird))
        {
            var line = PreviousToken.LineNumber;
            var expressions = new List<Expression?>();
            while (!HasKind(TokenKind.LeftBird) && !IsAtEnd)
                expressions.Add(ParseProgram());

            Consume(TokenKind.LeftBird, Syntax.UnterminatedBlock);
            return new BlockExpression(_guidProvider.Get(), expressions) { Line = line };
        }

        return ParseExpression();
    }

    private Expression ParseExpression()
    {
        if (Matching(TokenKind.Int, TokenKind.Double, TokenKind.Bool,
                TokenKind.Char, TokenKind.String, TokenKind.Def))
        {
            return ParseVariableDeclaration();
        }

        return ParseAssignment();
    }

    private Expression ParseAssignment()
    {
        var expression = ParseBasicExpression();
        if (Matching(TokenKind.Equal))
        {
            var equals = PreviousToken;
            var value = ParseBasicExpression();
            if (expression is VariableExpression varExpr)
            {
                var name = varExpr.Name;
                return new AssignmentExpression(name, value) { Line = name.LineNumber };
            }

            _errorReporter.ErrorAtToken(equals, Syntax.InvalidAssignmentTarget);
        }

        return expression;
    }

    private Expression ParseVariableDeclaration()
    {
        var type = PreviousToken;
        var name = Consume(TokenKind.Identifier, Syntax.ExpectedIdentifier);
        Consume(TokenKind.Equal, Syntax.ExpectedAssignment);
        var initializer = ParseBasicExpression();

        return new VariableDeclarationExpression(type, name, initializer) { Line = name.LineNumber };
    }

    private Expression ParseBasicExpression()
    {
        var expression = ParseEquality();
        if (Matching(TokenKind.Question))
        {
            var thenExpression = ParseBasicExpression();
            Consume(TokenKind.Colon, Syntax.ExpectedColon);
            var elseExpression = ParseBasicExpression();

            expression = new TernaryExpression(expression, thenExpression, elseExpression)
            {
                Line = expression.Line
            };
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
            expression = new BinaryExpression(expression, op, rightExpression) { Line = op.LineNumber };
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
            expression = new BinaryExpression(expression, op, rightExpression) { Line = op.LineNumber };
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
            expression = new BinaryExpression(expression, op, rightExpression) { Line = op.LineNumber };
        }

        return expression;
    }

    private Expression ParseMultiplicative()
    {
        var expression = ParseUnary();

        while (Matching(TokenKind.Asterisk, TokenKind.Slash, TokenKind.Percent))
        {
            var op = PreviousToken;
            var rightExpression = ParseUnary();
            expression = new BinaryExpression(expression, op, rightExpression) { Line = op.LineNumber };
        }

        return expression;
    }

    private Expression ParseUnary()
    {
        if (Matching(TokenKind.Minus, TokenKind.Not))
        {
            var op = PreviousToken;
            var rightExpression = ParseUnary();
            return new UnaryExpression(op, rightExpression) { Line = op.LineNumber };
        }

        return ParseExponential();
    }

    private Expression ParseExponential()
    {
        var expression = ParseCall();

        while (Matching(TokenKind.Hat))
        {
            var op = PreviousToken;
            var rightExpression = ParseExponential();
            expression = new BinaryExpression(expression, op, rightExpression)
            {
                Line = op.LineNumber
            };
        }

        return expression;
    }

    private Expression ParseCall()
    {
        var expression = ParsePrimary();
        while (true)
        {
            if (Matching(TokenKind.LeftBracket))
                expression = FinishCallParse(expression);
            else break;
        }

        return expression;
    }

    private Expression FinishCallParse(Expression called)
    {
        var arguments = new List<Expression>();

        if (!HasKind(TokenKind.RightBracket))
        {
            do
            {
                // todo consider limiting max arg count
                arguments.Add(ParseExpression());
            } while (Matching(TokenKind.Comma));
        }

        Consume(TokenKind.RightBracket, Syntax.ExpectedClosingArgsBracket);

        return new CallExpression(called, arguments) { Line = called.Line };
    }

    private Expression ParsePrimary()
    {
        if (Matching(TokenKind.True))
        {
            return new LiteralExpression(true)
            {
                Type = new WarblerType(ExpressionType.Boolean),
                Line = PreviousToken.LineNumber
            };
        }

        if (Matching(TokenKind.False))
        {
            return new LiteralExpression(false)
            {
                Type = new WarblerType(ExpressionType.Boolean),
                Line = PreviousToken.LineNumber
            };
        }

        if (Matching(TokenKind.IntLiteral, TokenKind.DoubleLiteral,
                TokenKind.StringLiteral, TokenKind.CharLiteral))
        {
            var type = PreviousToken.Kind switch
            {
                TokenKind.IntLiteral => ExpressionType.Integer,
                TokenKind.DoubleLiteral => ExpressionType.Double,
                TokenKind.StringLiteral => ExpressionType.String,
                TokenKind.CharLiteral => ExpressionType.Char,
                _ => throw new ArgumentException() // unreachable
            };
            Debug.Assert(PreviousToken.Literal != null, "PreviousToken.Literal != null");
            return new LiteralExpression(PreviousToken.Literal)
            {
                Type = new WarblerType(type),
                Line = PreviousToken.LineNumber
            };
        }

        if (Matching(TokenKind.LeftBracket))
        {
            var expression = ParseBasicExpression();
            Consume(TokenKind.RightBracket, Syntax.ExpectedClosingBracket);
            return expression;
        }

        if (Matching(TokenKind.Identifier))
        {
            return new VariableExpression(PreviousToken)
            {
                Line = PreviousToken.LineNumber
            };
        }

        throw HandleParseError(CurrentToken, Syntax.ExpectedExpression);
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

    private Token ConsumeAny(string message, params TokenKind[] kinds)
    {
        foreach (var kind in kinds)
        {
            if (HasKind(kind))
                return NextToken();
        }

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
                case TokenKind.Func:
                case TokenKind.In:
                case TokenKind.Int:
                case TokenKind.Of:
                // case TokenKind.Ret:
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