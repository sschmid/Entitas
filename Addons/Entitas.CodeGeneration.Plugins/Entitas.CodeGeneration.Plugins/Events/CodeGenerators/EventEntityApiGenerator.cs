using System.Linq;
using DesperateDevs.CodeGeneration;

namespace Entitas.CodeGeneration.Plugins {

    public class EventEntityApiGenerator : AbstractGenerator {

        public override string name { get { return "Event (Entity API)"; } }

        const string TEMPLATE = @"public static class ${EventListener}
{
    public static void Add${EventListener}(this ${EntityType} entity, I${EventListener} value)
    {
        var listeners = entity.Has${EventListener}()
            ? entity.Get${EventListener}().Value
            : new System.Collections.Generic.List<I${EventListener}>();
        listeners.Add(value);
        entity.Replace${EventListener}(listeners);
    }

    public static void Remove${EventListener}(this ${EntityType} entity, I${EventListener} value, bool removeComponentWhenEmpty = true)
    {
        var listeners = entity.Get${EventListener}().Value;
        listeners.Remove(value);
        if (removeComponentWhenEmpty && listeners.Count == 0)
            entity.Remove${EventListener}();
        else
            entity.Replace${EventListener}(listeners);
    }
}
";

        public override CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ComponentData>()
                .Where(d => d.IsEvent())
                .SelectMany(generate)
                .ToArray();
        }

        CodeGenFile[] generate(ComponentData data) {
            return data.GetContextNames()
                .SelectMany(contextName => generate(contextName, data))
                .ToArray();
        }

        CodeGenFile[] generate(string contextName, ComponentData data) {
            return data.GetEventData()
                .Select(eventData =>
                {
                    var fileContent = TEMPLATE
                        .Replace(data, contextName, eventData)
                        .WrapInNamespace(data.GetNamespace(), contextName);

                    return new CodeGenFile(
                        data.GetTypeName().ToFileName(contextName),
                        fileContent,
                        GetType().FullName
                    );
                }).ToArray();
        }
    }
}
