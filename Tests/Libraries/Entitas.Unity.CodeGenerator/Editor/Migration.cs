using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Entitas;
using Entitas.CodeGenerator;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.CodeGenerator {

    public static class Migration {

        [MenuItem("Entitas/Migration/Migrate Matcher")]
        public static void MigrateMatcher() {
            if (EditorUtility.DisplayDialog(
                    "Migrate Matcher",
                    "Entitas 0.12.0 introduced prefixed matchers based on the PoolAttribute, " +
                    "e.g UIMatcher.Button (instead of Matcher.Button).\n\n" +
                    "If you were using a version < 0.12.0 click 'Migrate Matcher' to let Entitas update your source files.",
                    "Migrate Matcher",
                    "Cancel"
                )) {
                var sourceFiles = getSourceFiles(Application.dataPath);
                migrateMatcher(sourceFiles);
                AssetDatabase.Refresh();
            }
        }

        static void migrateMatcher(Dictionary<string, string> sourceFiles) {
            var replacements = getMatcherReplacements();
            var updatedFiles = new Dictionary<string, string>();
            foreach (var file in sourceFiles) {
                var code = file.Value;
                foreach (var r in replacements) {
                    var pattern = @"\b" + r.Key + @"\b";
                    if (Regex.IsMatch(code, pattern)) {
                        code = Regex.Replace(code, pattern, r.Value);
                        updatedFiles[file.Key] = code;
                    }
                }
            }

            foreach (var file in updatedFiles) {
                File.WriteAllText(file.Key, file.Value);
                Debug.Log("Migrated " + file.Key);
            }

            Debug.Log("Migrated " + updatedFiles.Count + " files");
        }

        static Dictionary<string, string> getSourceFiles(string path) {
            var config = new CodeGeneratorConfig(EntitasPreferencesEditor.LoadConfig());
            return Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories)
            .Where(p => !p.Contains(config.generatedFolderPath))
            .ToDictionary(p => p, p => File.ReadAllText(p));
        }

        static Dictionary<string, string> getMatcherReplacements() {
            var types = Assembly.GetAssembly(typeof(Entity)).GetTypes();
            return Entitas.CodeGenerator.CodeGenerator.GetComponents(types)
            .Where(type => type.PoolName() != string.Empty)
            .ToDictionary(
                type => "Matcher." + type.RemoveComponentSuffix(),
                type => type.PoolName() + "Matcher." + type.RemoveComponentSuffix()
            );
        }
    }
}
