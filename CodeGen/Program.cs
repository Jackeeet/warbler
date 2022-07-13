using System.Text;

namespace CodeGen;

internal class Program
{
    static void Main(string[] args)
    {
        var outputDir = @"C:\dev\Warbler\Warbler\Parser\";
        var exprTypes = new List<string>
        {
            "Unary      : Token Op, Expression Expression",
            "Binary     : Expression Left, Token Op, Expression Right",
            "Ternary    : Expression Condition, Expression ThenBranch, Expression ElseBranch",
            "Literal    : object? Value",
            "Grouping   : Expression Expression",
        };

        DefineAst(outputDir, "Expression", exprTypes);
    }

    private static void DefineVisitorInterface(StreamWriter file, string baseName, List<string> types)
    {
        file.WriteLine($"public interface I{baseName}Visitor<out T>");
        file.WriteLine("{");

        foreach (var type in types)
        {
            var typeName = type.Split(":")[0].Trim();
            file.WriteLine($"\tT Visit{typeName}{baseName}({typeName}{baseName} {baseName.ToLower()});");
        }

        file.WriteLine("}");
        file.WriteLine();
    }

    private static void DefineAst(string outputDir, string baseName, List<string> types)
    {
        var path = Path.Join(outputDir, baseName + ".cs");
        File.Delete(path);
        using StreamWriter file = new(path, append: true);

        file.WriteLine("using Warbler.Scanner;");
        file.WriteLine("namespace Warbler.Parser;");
        file.WriteLine();

        DefineVisitorInterface(file, baseName, types);

        file.WriteLine($"public abstract class {baseName}");
        file.WriteLine("{");
        file.WriteLine($"\tpublic abstract T Accept<T>(I{baseName}Visitor<T> visitor);");
        file.WriteLine("}");
        file.WriteLine();

        foreach (var type in types)
        {
            var typeData = type.Split(':');
            var className = typeData[0].Trim();
            var fields = typeData[1].Trim();
            DefineType(file, baseName, className, fields);
        }

        file.WriteLine();
    }

    private static void DefineType(StreamWriter file, string baseName, string className, string fieldList)
    {
        file.WriteLine($"public class {className}{baseName} : {baseName}");
        file.WriteLine("{");
        var fields = fieldList.Split(", ");

        // fields
        foreach (var field in fields)
        {
            var fieldData = field.Split(" ");
            var fieldType = fieldData[0].Trim();
            // if (fieldType == "Expression" || fieldType == "Statement")
            // {
            //     fieldType += "?";
            // }

            var fieldName = fieldData[1].Trim();
            file.WriteLine($"\tpublic readonly {fieldType} {fieldName};");
        }

        file.WriteLine();

        // ctor
        var ctorFieldList = GetCtorFieldList(fields);
        file.WriteLine($"\tpublic {className}{baseName}({ctorFieldList})");
        file.WriteLine("\t{");
        foreach (var field in fields)
        {
            var fieldData = field.Split(" ");
            var fieldName = fieldData[1].Trim();
            file.WriteLine($"\t\t{fieldName} = {fieldName.ToLower()};");
        }

        file.WriteLine("\t}");

        file.WriteLine();
        file.WriteLine($"\tpublic override T Accept<T>(I{baseName}Visitor<T> visitor)");
        file.WriteLine("\t{");
        file.WriteLine($"\t\treturn visitor.Visit{className}{baseName}(this);");
        file.WriteLine("\t}");

        file.WriteLine("}");
        file.WriteLine();
    }

    private static string GetCtorFieldList(string[] fields)
    {
        var sb = new StringBuilder();
        foreach (var field in fields)
        {
            var fieldData = field.Split(" ");
            var fieldType = fieldData[0].Trim();
            // if (fieldType == "Expression" || fieldType == "Statement")
            // {
            //     fieldType += "?";
            // }

            var fieldName = fieldData[1].Trim();
            sb.Append($"{fieldType} {fieldName.ToLower()}, ");
        }

        var result = sb.ToString().Substring(0, sb.Length - 2);
        return result;
    }
}