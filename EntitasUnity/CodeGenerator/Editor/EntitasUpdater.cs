using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Entitas;
using Entitas.CodeGenerator;
using UnityEditor;
using UnityEngine;

public static class EntitasUpdater {

    [MenuItem("Entitas/Update API")]
    public static void UpdateAPI() {
        var sourceFiles = getSourceFiles(Application.dataPath);
        updateMatcher(sourceFiles);
        AssetDatabase.Refresh();
    }

    static void updateMatcher(Dictionary<string, string> sourceFiles) {
        var replacements = getMatcherReplacements();
        var updatedFiles = new Dictionary<string, string>();
        foreach (var file in sourceFiles) {
            var code = file.Value;
            foreach (var r in replacements) {
                var pattern = "\\b" + r.Key + "\\b";
                if (Regex.IsMatch(code, pattern)) {
                    code = Regex.Replace(code, pattern, r.Value);
                    updatedFiles[file.Key] = code;
                }
            }
        }

        foreach (var file in updatedFiles) {
            File.WriteAllText(file.Key, file.Value);
            Debug.Log("Updated " + file.Key);
        }
    }

    static Dictionary<string, string> getSourceFiles(string path) {
        var config = EntitasCodeGeneratorEditor.LoadConfig();
        return Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories)
            .Where(p => !p.Contains(config.generatedFolderPath))
            .ToDictionary(p => p, p => File.ReadAllText(p));
    }

    static Dictionary<string, string> getMatcherReplacements() {
        var types = Assembly.GetAssembly(typeof(Entity)).GetTypes();
        return EntitasCodeGenerator.GetComponents(types)
            .Where(type => type.PoolName() != string.Empty)
            .ToDictionary(
                type => "Matcher." + type.RemoveComponentSuffix(),
                type => type.PoolName() + "Matcher." + type.RemoveComponentSuffix()
            );
    }
}

