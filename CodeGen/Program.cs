namespace CodeGen;

internal class Program
{
    static void Main(string[] args)
    {
        switch (args.Length)
        {
            case 1:
                var testDir = @"C:\dev\Warbler\Tests\CheckerTests\";
                var nameSpace = "CheckerTests";
                var stubs = new List<string>()
                {
                    // "BinaryExpressionsData",
                    // "TernaryExpressionsData",
                    // "LiteralExpressionsData"
                };

                foreach (var fileName in stubs)
                    TestStubGenerator.SetupTestStub(testDir, nameSpace, fileName);
                break;
            default:
                var expressionsDir = @"C:\dev\Warbler\Warbler\Expressions\";
                var exprTypes = new List<string>
                {
                    "Unary                  : Token Op, Expression Expression",
                    "Binary                 : Expression Left, Token Op, Expression Right",
                    "Ternary                : Expression Condition, Expression ThenBranch, Expression ElseBranch",
                    "Literal                : object Value",
                    "VariableDeclaration    : Token VarType, Token Name, Expression Initializer",
                    "Variable               : Token Name",
                    "Assignment             : Token Name, Expression Value"
                };

                ExpressionsGenerator.DefineAst(expressionsDir, exprTypes);
                InterfaceGenerator.DefineVisitorInterface(expressionsDir, exprTypes);
                break;
        }
    }
}