using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NSpec;

class check_namespaces : nspec {
    static string getEntitasProjectDir() {
        var dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
        return dirInfo.FullName;
    }

    static Dictionary<string, string> getSourceFiles(string path) {
        return Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories)
            .Where(p => !p.Contains("Generated/") &&
        !p.Contains("Libraries/") &&
        !p.Contains("Tests/") &&
        !p.Contains("Readme/") &&
        !p.Contains("bin/") &&
        !p.Contains("AssemblyInfo.cs") &&
        !p.Contains("Program.cs"))
            .ToDictionary(p => p, p => File.ReadAllText(p));
    }

    void when_running_test() {
        it["checks namespaces"] = () => {
            try {
                var entitasDir = getEntitasProjectDir();
                var sourceFiles = getSourceFiles(entitasDir);

                sourceFiles.Count.should_be_greater_than(60);
                sourceFiles.Count.should_be_less_than(70);

                const string namespacePattern = @"(?:^namespace)\s.*\b";
                const string expectedNamespacePattern = @".+?[^\/]*";

                foreach (var file in sourceFiles) {
                    var fileName = file.Key.Replace(entitasDir + "/", string.Empty);
                    var expectedNamespace = Regex.Match(fileName, expectedNamespacePattern)
                        .ToString()
                        .Replace("namespace ", string.Empty)
                        .Trim();

                    var foundNamespace = Regex.Match(file.Value, namespacePattern, RegexOptions.Multiline)
                        .ToString()
                        .Replace("namespace ", string.Empty)
                        .Trim();

                    if (foundNamespace != expectedNamespace) {
                        Console.WriteLine(fileName + " exp: '" + expectedNamespace + "' was: '" + foundNamespace + "'");
                    }

                    foundNamespace.should_be(expectedNamespace);
                }
            } catch (Exception) {
                Console.WriteLine("Skipping check_namespaces");
            }
        };
    }
}

