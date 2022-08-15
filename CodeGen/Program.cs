namespace CodeGen;

internal class Program
{
    static void Main(string[] args)
    {
        switch (args.Length)
        {
            case 1:
                var testDir = @"C:\dev\Warbler\Tests\InterpreterTests\";
                var nameSpace = "InterpreterTests";
                var stubs = new List<string>()
                {
                    "UnaryExpressionsData",
                    "BinaryExpressionsData",
                    "TernaryExpressionsData",
                    "LiteralExpressionsData",
                    "VariableExpressionsData",
                    "BlockExpressionsData"
                };

                foreach (var fileName in stubs)
                    TestStubGenerator.SetupTestStub(testDir, nameSpace, fileName);
                break;
            default:
                var expressionsDir = @"C:\dev\Warbler\Warbler\Expressions\";
                var exprTypes = new List<string>
                {
                    "Unary                  : Token Op # Expression Expression",
                    "Binary                 : Expression Left # Token Op # Expression Right",
                    "Ternary                : Expression Condition # Expression ThenBranch # Expression ElseBranch",
                    "Literal                : object Value",
                    "VariableDeclaration    : Token VarType # Token Name # Expression Initializer",
                    "Variable               : Token Name",
                    "Assignment             : Token Name # Expression Value",
                    "Block                  : Guid BlockId # List<Expression?> Expressions",
                    "Conditional            : Expression Condition # Expression ThenBranch # Expression? ElseBranch",
                    "WhileLoop              : Expression Condition # Expression Actions",
                    "FunctionDefinition     : Token Name # List<Tuple<Token, Token>> Parameters # Token ReturnType # BlockExpression Body",
                    "Call                   : Expression Called # List<Expression> Args"
                };

                ExpressionsGenerator.DefineAst(expressionsDir, exprTypes);
                InterfaceGenerator.DefineVisitorInterface(expressionsDir, exprTypes);
                break;
        }
    }
}