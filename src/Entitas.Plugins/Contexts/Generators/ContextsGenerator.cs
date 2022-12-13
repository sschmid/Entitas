using System.Linq;
using Jenny;

namespace Entitas.Plugins
{
    public class ContextsGenerator : ICodeGenerator
    {
        public string Name => "Contexts";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string Template =
            @"public partial class Contexts : Entitas.IContexts
{
    public static Contexts Instance
    {
        get { return _instance ?? (_instance = new Contexts()); }
        set { _instance = value; }
    }

    static Contexts _instance;

${ContextPropertiesList}

    public Entitas.IContext[] AllContexts
    {
        get { return new Entitas.IContext[] {${ContextList}}; }
    }

    public Contexts()
    {
${ContextAssignmentsList}

        var postConstructors = System.Linq.Enumerable.Where(
            GetType().GetMethods(),
            method => System.Attribute.IsDefined(method, typeof(Entitas.Plugins.Attributes.ContextsPostConstructorAttribute))
        );

        foreach (var postConstructor in postConstructors)
            postConstructor.Invoke(this, null);
    }

    public void Reset()
    {
        foreach (var context in AllContexts)
            context.Reset();
    }
}
";

        const string ContextPropertyTemplate = @"    public ${Context.Type} ${Context.Name} { get; set; }";
        const string ContextListTemplate = @"${Context.Name}";
        const string ContextAssignmentTemplate = @"        ${Context.Name} = new ${Context.Type}();";

        public CodeGenFile[] Generate(CodeGeneratorData[] data)
        {
            var contexts = data
                .OfType<ContextData>()
                .OrderBy(d => d.Name)
                .ToArray();

            return new[]
            {
                new CodeGenFile(
                    "Contexts.cs",
                    Generate(contexts),
                    GetType().FullName)
            };
        }

        string Generate(ContextData[] data)
        {
            var contextPropertiesList = string.Join("\n", data.Select(d => d
                .ReplacePlaceholders(ContextPropertyTemplate)));

            var contextList = string.Join(", ", data.Select(d => d
                .ReplacePlaceholders(ContextListTemplate)));

            var contextAssignmentsList = string.Join("\n", data.Select(d => d
                .ReplacePlaceholders(ContextAssignmentTemplate)));

            return Template
                .Replace("${ContextPropertiesList}", contextPropertiesList)
                .Replace("${ContextList}", contextList)
                .Replace("${ContextAssignmentsList}", contextAssignmentsList);
        }
    }
}
