using System.IO;
using System.Linq;
using Jenny;
using Entitas.Plugins.Attributes;
using Entitas.Plugins;

namespace Entitas.Roslyn.CodeGeneration.Plugins
{
    public class CleanupSystemGenerator : AbstractGenerator
    {
        public override string Name => "Cleanup (System)";

        const string DestroyEntityTemplate =
            @"using System.Collections.Generic;
using Entitas;

public sealed class Destroy${ComponentName}${SystemType} : ICleanupSystem {

    readonly IGroup<${EntityType}> _group;
    readonly List<${EntityType}> _buffer = new List<${EntityType}>();

    public Destroy${ComponentName}${SystemType}(Contexts contexts) {
        _group = contexts.${context}.GetGroup(${MatcherType}.${ComponentName});
    }

    public void Cleanup() {
        foreach (var e in _group.GetEntities(_buffer)) {
            e.Destroy();
        }
    }
}
";

        const string RemoveComponentTemplate =
            @"using System.Collections.Generic;
using Entitas;

public sealed class Remove${ComponentName}${SystemType} : ICleanupSystem {

    readonly IGroup<${EntityType}> _group;
    readonly List<${EntityType}> _buffer = new List<${EntityType}>();

    public Remove${ComponentName}${SystemType}(Contexts contexts) {
        _group = contexts.${context}.GetGroup(${MatcherType}.${ComponentName});
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
            .SelectMany(d => Generate(d))
            .ToArray();

        CodeGenFile[] Generate(CleanupData data) => data
            .ComponentData.Contexts
            .Select(context => Generate(context, data))
            .ToArray();

        CodeGenFile Generate(string context, CleanupData data)
        {
            var template = data.CleanupMode == CleanupMode.DestroyEntity
                ? DestroyEntityTemplate
                : RemoveComponentTemplate;

            var fileContent = template
                .Replace("${SystemType}", context.AddSystemSuffix())
                .Replace("${EntityType}", context.AddEntitySuffix())
                .Replace("${MatcherType}", context.AddMatcherSuffix())
                .Replace("${removeComponent}", RemoveComponent(data))
                .Replace(data.ComponentData, context);

            var prefix = data.CleanupMode == CleanupMode.DestroyEntity ? "Destroy" : "Remove";
            return new CodeGenFile(
                Path.Combine(context, "Systems", $"{prefix}{data.ComponentData.Type.ToComponentName()}{context.AddSystemSuffix()}.cs"),
                fileContent,
                GetType().FullName
            );
        }

        static string RemoveComponent(CleanupData data) => data.ComponentData.MemberData.Length == 0
            ? $"{data.ComponentData.PrefixedComponentName()} = false"
            : $"Remove{data.ComponentData.Type.ToComponentName()}()";
    }
}
