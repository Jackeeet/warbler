namespace CodeGen;

internal static class TestStubGenerator
{
    internal static void SetupTestStub(string outputDir, string nameSpace, string fileName)
    {
        var path = Path.Join(outputDir, fileName + ".cs");
        using StreamWriter file = new(path, append: true);
        
        file.WriteLine("using System.Collections.Generic;");
        file.WriteLine("using Warbler.Expressions;");
        file.WriteLine();
        file.WriteLine($"namespace Tests.{nameSpace};");
        file.WriteLine();
        
        file.WriteLine($"public static class {fileName}");
        file.WriteLine("{");
        
        file.WriteLine("\tpublic static readonly List<string> ValidNames = new() { };");
        file.WriteLine();
        
        file.WriteLine("\tpublic static readonly List<string> InvalidNames = new() {};");
        file.WriteLine();
        
        file.WriteLine("\tpublic static readonly Dictionary<string, Expression> Inputs = new() {};");
        file.WriteLine();
        
        file.WriteLine("\tpublic static readonly Dictionary<string, Expression> Outputs = new() {};");
        
        file.WriteLine("}");
    }
}