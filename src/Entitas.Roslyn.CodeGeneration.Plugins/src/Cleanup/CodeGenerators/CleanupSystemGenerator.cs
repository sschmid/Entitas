using System.IO;
using System.Linq;
using Jenny;
using Entitas.CodeGeneration.Attributes;
using Entitas.CodeGeneration.Plugins;

namespace Entitas.Roslyn.CodeGeneration.Plugins
{
    public class CleanupSystemGenerator : AbstractGenerator
    {
        public override string Name => "Cleanup (System)";

        const string DESTROY_ENTITY_TEMPLATE =
            @"using System.Collections.Generic;
using Entitas;

public sealed class Destroy${ComponentName}${SystemType} : ICleanupSystem {

    readonly IGroup<${EntityType}> _group;
    readonly List<${EntityType}> _buffer = new List<${EntityType}>();

    public Destroy${ComponentName}${SystemType}(Contexts contexts) {
        _group = contexts.${contextName}.GetGroup(${MatcherType}.${ComponentName});
    }

    public void Cleanup() {
        foreach (var e in _group.GetEntities(_buffer)) {
            e.Destroy();
        }
    }
}
";

        const string REMOVE_COMPONENT_TEMPLATE =
            @"using System.Collections.Generic;
using Entitas;

public sealed class Remove${ComponentName}${SystemType} : ICleanupSystem {

    readonly IGroup<${EntityType}> _group;
    readonly List<${EntityType}> _buffer = new List<${EntityType}>();

    public Remove${ComponentName}${SystemType}(Contexts contexts) {
        _group = contexts.${contextName}.GetGroup(${MatcherType}.${ComponentName});
    }

    public void Cleanup() {
        foreach (var e in _group.GetEntities(_buffer)) {
            e.${removeComponent};
        }
    }
}
";

        public override CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<CleanupData>()
            .SelectMany(generate)
            .ToArray();

        CodeGenFile[] generate(CleanupData data) => data
            .componentData.GetContextNames()
            .Select(contextName => generate(contextName, data))
            .ToArray();

        CodeGenFile generate(string contextName, CleanupData data)
        {
            var template = data.cleanupMode == CleanupMode.DestroyEntity
                ? DESTROY_ENTITY_TEMPLATE
                : REMOVE_COMPONENT_TEMPLATE;

            var fileContent = template
                .Replace("${SystemType}", contextName.AddSystemSuffix())
                .Replace("${EntityType}", contextName.AddEntitySuffix())
                .Replace("${MatcherType}", contextName.AddMatcherSuffix())
                .Replace("${removeComponent}", removeComponent(data))
                .Replace(data.componentData, contextName);

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar +
                "Systems" + Path.DirectorySeparatorChar +
                (data.cleanupMode == CleanupMode.DestroyEntity ? "Destroy" : "Remove") + data.componentData.ComponentName() + contextName.AddSystemSuffix() + ".cs",
                fileContent,
                GetType().FullName
            );
        }

        static string removeComponent(CleanupData data)
        {
            if (data.componentData.GetMemberData().Length == 0)
                return $"{data.componentData.PrefixedComponentName()} = false";

            return $"Remove{data.componentData.ComponentName()}()";
        }
    }
}
