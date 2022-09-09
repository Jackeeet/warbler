# Grammar

--- TOP LEVEL ---------------------------------------------------------------------------------------

program           = {functionDef | whileLoop | conditional | block | expression}-;

functionDef       = "\", id, "(", [parameters], ")", type, block;

whileLoop         = "while", basic, innerBlock;

conditional       = "if", basic, "then", innerBlock, ["else", (conditional | innerBlock)];

block             = ":>" program "<:";

--- EXPRESSION -------------------------------------------------------------------------------------

expression        = varDeclaration | assignment;

varDeclaration    = ("def" | INTEGER | DOUBLE | CHAR | STRING | BOOLEAN ) id "=" basic;

assignment        = id, "=", basic | basic;

--- BASIC ------------------------------------------------------------------------------------------

basic             = equality | ternary;

ternary           = equality, "?", basic, ":", basic;

equality          = relational, [("!=" | "=="), relational];

relational        = additive, [(">" | "<" | ">=" | "<="), additive];

additive          = multiplicative, {("+" | "-" | "++"), multiplicative};

multiplicative    = unary, {("*" | "/" | "%"), unary};

unary             = ("-" | "!"), unary | exponential;

exponential       = call, {"^", call};

call         	  = primary, {"(", [arguments], ")"};

primary           = literal | grouping;

literal           = type | "true" | "false" | id;

grouping          = "(" basic ")";

--- HELPERS ----------------------------------------------------------------------------------------

innerBlock        = block | expression;

parameters        = type, id, {",", type, id};

arguments         = expression, {",", expression};

type              = STRING | CHAR | INTEGER | DOUBLE | BOOLEAN | functionType;

functionType      = "fn", "(", [typeArguments], ")", ":", type;

typeArguments     = type, {",", type};

id                = "_" | alpha, {character};

character         = "_" | digit | alpha

digit             = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9";

alpha             = "A" | "B" | "C" | "D" | "E" | "F" | "G" | "H" | "I" | "J" | "K" | "L" | "M" |
                    "N" | "O" | "P" | "Q" | "R" | "S" | "T" | "U" | "V" | "W" | "X" | "Y" | "Z" |
                    "a" | "b" | "c" | "d" | "e" | "f" | "g" | "h" | "i" | "j" | "k" | "l" | "m" | 
                    "n" | "o" | "p" | "q" | "r" | "s" | "t" | "u" | "v" | "w" | "x" | "y" | "z" ;

STRING            = "string";

CHAR              = "char";

INTEGER           = "int";

DOUBLE            = "double";

BOOLEAN           = "bool";
