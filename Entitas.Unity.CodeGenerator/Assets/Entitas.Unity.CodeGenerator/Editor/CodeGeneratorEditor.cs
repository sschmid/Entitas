using System;
using System.Linq;
using System.Reflection;
using Entitas;
using Entitas.CodeGenerator;
using UnityEditor;

namespace Entitas.Unity.CodeGenerator {
    public static class CodeGeneratorEditor {

        [MenuItem("Entitas/Generate")]
        public static void Generate() {
            var types = Assembly.GetAssembly(typeof(Entity)).GetTypes();
            var config = new CodeGeneratorConfig(EntitasPreferencesEditor.LoadConfig());

            var disabledCodeGenerators = config.disabledCodeGenerators;
            var codeGenerators = GetCodeGenerators()
                .Where(type => !disabledCodeGenerators.Contains(type.Name))
                .Select(type => (ICodeGenerator)Activator.CreateInstance(type))
                .ToArray();

            Entitas.CodeGenerator.CodeGenerator.Generate(types, config.pools, config.generatedFolderPath, codeGenerators);

            AssetDatabase.Refresh();
        }

        public static Type[] GetCodeGenerators() {
            return Assembly.GetAssembly(typeof(ICodeGenerator)).GetTypes()
                .Where(type => type.GetInterfaces().Contains(typeof(ICodeGenerator))
                    && type != typeof(ICodeGenerator)
                    && type != typeof(IPoolCodeGenerator)
                    && type != typeof(IComponentCodeGenerator)
                    && type != typeof(ISystemCodeGenerator))
                .OrderBy(type => type.FullName)
                .ToArray();
        }
    }
}
