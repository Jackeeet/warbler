namespace Warbler.Scanner;

public enum TokenKind
{
    // single-character tokens
    Not,            // !
    Equal,          // =
    Plus,           // + 
    Minus,          // -
    Asterisk,       // *
    Slash,          // /
    Comma,          // ,
    Dot,            // . 
    Colon,          // :
    Semicolon,      // ;
    Question,       // ? Option / Ternary
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
    Fun,
    Def,
    Type,
    Inst,
    Ret,
    Print,
    And,
    Or,
    True,
    False,
    Bool,
    Int,
    Double,
    Char,
    String,
    
    Eof
}