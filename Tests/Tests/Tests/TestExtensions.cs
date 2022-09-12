using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entitas;
using NSpec;

public static class TestExtensions
{
    public static void Fail(this nspec spec)
    {
        "but did".should_be("should not happen");
    }

    public static TestEntity CreateEntity(this nspec spec)
    {
        var entity = new TestEntity();
        entity.Initialize(0, CID.TotalComponents, new Stack<IComponent>[CID.TotalComponents]);
        return entity;
    }

    public static string GetProjectRoot()
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (current.Name != "Entitas" && current.Name != "Entitas-CSharp") current = current.Parent;
        return current.FullName;
    }

    public static Dictionary<string, string> GetSourceFiles(string path)
    {
        return Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories)
            .Where(p =>
                !p.Contains(dir("DesperateDevs")) &&
                !p.Contains(dir("Generated")) &&
                !p.Contains(dir("Libraries")) &&
                !p.Contains(dir("Tests")) &&
                !p.Contains(dir("Examples")) &&
                !p.Contains(dir("Readme")) &&
                !p.Contains(dir("Build")) &&
                !p.Contains(dir("bin")) &&
                !p.Contains(dir("obj")) &&
                !p.Contains(dir("packages")) &&
                !p.Contains("AssemblyInfo.cs") &&
                !p.Contains("Commands.cs") &&
                !p.Contains("Program.cs")
            ).ToDictionary(p => p, p => File.ReadAllText(p));
    }

    public static Dictionary<string, string> GetSourceFilesInclAllProjects(string path)
    {
        return Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories)
            .Where(p =>
                !p.Contains(dir("Generated")) &&
                !p.Contains(dir("Libraries")) &&
                !p.Contains(dir("Build")) &&
                !p.Contains(dir("bin")) &&
                !p.Contains(dir("obj")) &&
                !p.Contains("AssemblyInfo.cs")
            ).ToDictionary(p => p, p => File.ReadAllText(p));
    }

    static string dir(params string[] paths)
    {
        return paths.Aggregate(string.Empty, (pathString, p) => pathString + p + Path.DirectorySeparatorChar);
    }
}
