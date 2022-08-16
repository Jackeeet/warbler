namespace Warbler.Utils.Token;

public enum TokenKind
{
    // single-character tokens
    Not,            // !
    Equal,          // =
    Plus,           // + 
    Minus,          // -
    Asterisk,       // *
    Slash,          // /
    Percent,         // %
    Comma,          // ,
    Dot,            // . 
    Colon,          // :
    Semicolon,      // ;
    Question,       // ? Option / Ternary
    Hat,            // ^ Raise to power
    LeftBracket,    // (
    RightBracket,   // )
    LessThan,       // <
    GreaterThan,    // >
    
    // double-character tokens
    RightBird,      // :> (block start)
    LeftBird,       // <: (block end)
    RightArrow,     // -> import
    NotEqual,       // !=
    DoubleEqual,    // ==
    LessEqual,      // <=
    GreaterEqual,   // >=
    DoublePlus,     // ++ Haskell-style string concatenation
    PlusEqual,      // +=
    MinusEqual,     // -=
    AsteriskEqual,  // *= 
    SlashEqual,     // /=
    // DoubleMinus and DoubleSlash are reserved for comments 

    // literals
    Identifier,
    CharLiteral,
    StringLiteral,  
    IntLiteral,
    DoubleLiteral,
    
    // keywords
    If,
    Then,
    Else,
    Case,
    Of,
    While,
    For,
    ForEach,
    In,
    FuncDef,        // \ for function definition
    Def,            // implicit type variable
    Type,           // user-defined types
    And,
    Or,
    True,
    False,
    Bool,
    Int,
    Double,
    Char,
    String,
    FuncType,       // fn for function type signatures

    Eof
}