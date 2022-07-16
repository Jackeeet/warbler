namespace CodeGen;

internal static class InterfaceGenerator
{
    internal static void DefineVisitorInterface(string outputDir, List<string> types)
    {
        var path = Path.Join(outputDir, "IExpressionVisitor.cs");
        File.Delete(path);
        using StreamWriter file = new(path, append: true);

        file.WriteLine("namespace Warbler.Expressions;");

        file.WriteLine("public interface IExpressionVisitor<out T>");
        file.WriteLine("{");

        foreach (var type in types)
        {
            var typeName = type.Split(":")[0].Trim();
            file.WriteLine($"\tT Visit{typeName}Expression({typeName}Expression expression);");
        }

        file.WriteLine("}");
        file.WriteLine();
    }
}