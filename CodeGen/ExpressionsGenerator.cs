using System.Text;

namespace CodeGen;

internal static class ExpressionsGenerator
{
    internal static void DefineAst(string outputDir, List<string> types)
    {
        var baseName = "Expression";
        var path = Path.Join(outputDir, baseName + ".cs");
        File.Delete(path);
        using StreamWriter file = new(path, append: true);
        
        file.WriteLine("using Warbler.Utils;");
        file.WriteLine();
        
        file.WriteLine("namespace Warbler.Expressions;");
        file.WriteLine();

        file.WriteLine($"public abstract class {baseName}");
        file.WriteLine("{");
        file.WriteLine("\tpublic ExpressionType Type { get; set; }");
        file.WriteLine();
        file.WriteLine("\tpublic int Line { get; init; }");
        file.WriteLine();
        file.WriteLine($"\tpublic abstract T Accept<T>(I{baseName}Visitor<T> visitor);");
        file.WriteLine();
        file.WriteLine("\tpublic override string ToString() => $\"Type: {Type}, Line: {Line}\";");
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
        var fieldData = GetFieldData(fieldList);

        // fields
        foreach (var field in fieldData)
            file.WriteLine($"\tpublic readonly {field.Item1} {field.Item2};");
        file.WriteLine();

        // ctor
        var sb = new StringBuilder();
        foreach (var field in fieldData)
            sb.Append($"{field.Item1} {field.Item2.ToLower()}, ");
        file.WriteLine($"\tpublic {className}{baseName}({sb.ToString().Substring(0, sb.Length - 2)})");
        // ctor body
        file.WriteLine("\t{");
        foreach (var field in fieldData)
            file.WriteLine($"\t\t{field.Item2} = {field.Item2.ToLower()};");
        file.WriteLine("\t}");
        file.WriteLine();

        // visitor accept method
        file.WriteLine($"\tpublic override T Accept<T>(I{baseName}Visitor<T> visitor)");
        file.WriteLine("\t{");
        file.WriteLine($"\t\treturn visitor.Visit{className}{baseName}(this);");
        file.WriteLine("\t}");
        file.WriteLine();

        // equality members
        file.WriteLine($"\tprotected bool Equals({className}{baseName} other)");
        file.WriteLine("\t{");
        file.Write("\t\treturn ");
        sb = new StringBuilder();
        var equalityFields = new List<Tuple<string, string>>
        {
            Tuple.Create("ExpressionType", "Type"),
            Tuple.Create("int", "Line")
        };
        equalityFields.AddRange(fieldData);
        foreach (var field in equalityFields)
            sb.Append($"{field.Item2}.Equals(other.{field.Item2}) && ");
        file.Write(sb.ToString().Substring(0, sb.Length - 4));
        file.WriteLine(";");
        file.WriteLine("\t}");
        file.WriteLine();

        file.WriteLine("\tpublic override bool Equals(object? obj)");
        file.WriteLine("\t{");
        file.WriteLine($"\t\treturn obj is {className}{baseName} other && Equals(other);");
        file.WriteLine("\t}");
        file.WriteLine();


        file.WriteLine("\tpublic override int GetHashCode()");
        file.WriteLine("\t{");
        file.Write("\t\treturn ");
        if (fieldData.Count == 1)
        {
            file.WriteLine($"{fieldData[0].Item2}.GetHashCode();");
        }
        else
        {
            file.Write("HashCode.Combine(");
            sb = new StringBuilder();
            foreach (var field in fieldData)
                sb.Append($"{field.Item2}, ");
            file.Write(sb.ToString().Substring(0, sb.Length - 2));
            file.WriteLine(");");
        }

        file.WriteLine("\t}");

        file.WriteLine("}");
        file.WriteLine();
    }

    private static List<Tuple<string, string>> GetFieldData(string fieldList)
    {
        var fields = fieldList.Split(" # ");
        var fieldTypes = new List<string>();
        var fieldNames = new List<string>();
        foreach (var field in fields)
        {
            var fd = field.Split(" ");
            fieldTypes.Add(fd[0].Trim());
            fieldNames.Add(fd[1].Trim());
        }

        var fieldData = fieldTypes.Zip(fieldNames, Tuple.Create).ToList();
        return fieldData;
    }
}