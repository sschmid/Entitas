using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    [Generator(LanguageNames.CSharp)]
    public sealed class ComponentGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            var components = initContext.SyntaxProvider
                .CreateSyntaxProvider(IsComponentCandidate, CreateComponentDeclaration)
                .Where(static component => component is not null)
                .Select(static (component, _) => component!.Value);

            var fullNameOrContextsChanged = components.WithComparer(new FullNameAndContextsComparer());
            initContext.RegisterSourceOutput(fullNameOrContextsChanged, OnFullNameOrContextsChanged);

            var fullNameOrMembersOrContextsChanged = components.WithComparer(new FullNameAndMembersAndContextsComparer());
            initContext.RegisterSourceOutput(fullNameOrMembersOrContextsChanged, OnFullNameOrMembersOrContextsChanged);

            var fullNameOrMembersOrContextsOrIsUniqueChanged = components.WithComparer(new FullNameAndMembersAndContextsAndIsUniqueComparer());
            initContext.RegisterSourceOutput(fullNameOrMembersOrContextsOrIsUniqueChanged, OnFullNameOrMembersOrContextsOrIsUniqueChanged);

            var fullNameOrContextsOrEventsChanged = components.WithComparer(new FullNameAndContextsAndEventsComparer());
            initContext.RegisterSourceOutput(fullNameOrContextsOrEventsChanged, OnFullNameOrContextsOrEventsChanged);

            var contextInitializationChanged = initContext.SyntaxProvider
                .CreateSyntaxProvider(IsContextInitializationMethodCandidate, CreateContextInitializationMethodDeclaration)
                .Where(static method => method is not null)
                .Select(static (method, _) => method!.Value);

            initContext.RegisterImplementationSourceOutput(contextInitializationChanged, OnContextInitializationChanged);
        }

        static bool IsComponentCandidate(SyntaxNode node, CancellationToken _)
        {
            return node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 } candidate
                   && candidate.BaseList.Types.Any(static baseType => baseType.Type switch
                   {
                       IdentifierNameSyntax identifierNameSyntax => identifierNameSyntax.Identifier is { Text: "IComponent" },
                       QualifiedNameSyntax qualifiedNameSyntax => qualifiedNameSyntax is
                       {
                           Left: IdentifierNameSyntax { Identifier.Text: "Entitas" },
                           Right: IdentifierNameSyntax { Identifier.Text: "IComponent" }
                       },
                       _ => false
                   })
                   && candidate.Modifiers.Any(SyntaxKind.PublicKeyword)
                   && !candidate.Modifiers.Any(SyntaxKind.StaticKeyword)
                   && candidate.Modifiers.Any(SyntaxKind.SealedKeyword)
                   && !candidate.Modifiers.Any(SyntaxKind.PartialKeyword);
        }

        static ComponentDeclaration? CreateComponentDeclaration(GeneratorSyntaxContext syntaxContext, CancellationToken cancellationToken)
        {
            var candidate = (ClassDeclarationSyntax)syntaxContext.Node;
            var symbol = syntaxContext.SemanticModel.GetDeclaredSymbol(candidate, cancellationToken);
            if (symbol is null)
                return null;

            var componentInterface = syntaxContext.SemanticModel.Compilation.GetTypeByMetadataName("Entitas.IComponent");
            if (componentInterface is null)
                return null;

            var isComponent = symbol.Interfaces.Contains(componentInterface);
            if (!isComponent)
                return null;

            return new ComponentDeclaration(symbol, cancellationToken);
        }

        static bool IsContextInitializationMethodCandidate(SyntaxNode node, CancellationToken _)
        {
            return node is MethodDeclarationSyntax { AttributeLists.Count: > 0 } candidate
                   && candidate.AttributeLists.Any(static attributeList => attributeList.Attributes
                       .Any(static attribute => attribute.Name is IdentifierNameSyntax { Identifier.Text: "ContextInitialization" or "ContextInitializationAttribute" }))
                   && candidate.Modifiers.Any(SyntaxKind.PublicKeyword)
                   && candidate.Modifiers.Any(SyntaxKind.StaticKeyword)
                   && candidate.Modifiers.Any(SyntaxKind.PartialKeyword)
                   && candidate.ReturnType is PredefinedTypeSyntax predefined
                   && predefined.Keyword.IsKind(SyntaxKind.VoidKeyword);
        }

        static ContextInitializationMethodDeclaration? CreateContextInitializationMethodDeclaration(GeneratorSyntaxContext syntaxContext, CancellationToken cancellationToken)
        {
            var candidate = (MethodDeclarationSyntax)syntaxContext.Node;
            var symbol = syntaxContext.SemanticModel.GetDeclaredSymbol(candidate, cancellationToken);
            if (symbol is null)
                return null;

            if (!symbol.ContainingType.IsStatic || symbol.ContainingType.DeclaredAccessibility != Accessibility.Public)
                return null;

            var context = symbol.GetAttributes()
                .Where(static attribute => attribute.AttributeClass?.ToDisplayString() == "Entitas.Generators.Attributes.ContextInitializationAttribute")
                .Select(static attribute => attribute.ConstructorArguments.SingleOrDefault())
                .Where(static arg => arg.Type?.ToDisplayString() == "System.Type" && arg.Value is INamedTypeSymbol)
                .Select(static arg => ((INamedTypeSymbol)arg.Value!).ToDisplayString())
                .Distinct()
                .SingleOrDefault();

            if (context is null)
                return null;

            var components = GetOrderedComponentsFromAllAssemblies(context, syntaxContext.SemanticModel.Compilation, cancellationToken);
            if (components is null)
                return null;

            return new ContextInitializationMethodDeclaration(symbol, context, components.Value);
        }

        static ImmutableArray<ComponentDeclaration>? GetOrderedComponentsFromAllAssemblies(string context, Compilation compilation, CancellationToken cancellationToken)
        {
            var componentInterface = compilation.GetTypeByMetadataName("Entitas.IComponent");
            if (componentInterface is null)
                return null;

            var allComponents = new List<ComponentDeclaration>();
            var stack = new Stack<INamespaceSymbol>();
            stack.Push(compilation.GlobalNamespace);

            while (stack.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                foreach (var member in stack.Pop().GetMembers())
                {
                    if (member is INamespaceSymbol ns)
                    {
                        stack.Push(ns);
                    }
                    else if (member is INamedTypeSymbol symbol)
                    {
                        var isComponent = symbol.Interfaces.Contains(componentInterface);
                        if (!isComponent)
                            continue;

                        if (!ComponentDeclaration.GetContexts(symbol).Contains(context))
                            continue;

                        var component = new ComponentDeclaration(symbol, cancellationToken);
                        allComponents.Add(component);

                        var contextPrefix = component.ContextPrefix(context);
                        var contextAwareComponentPrefix = component.ContextAwareComponentPrefix(contextPrefix);
                        foreach (var @event in component.Events)
                        {
                            var eventStrings = new EventStrings(@event, component.ComponentPrefix, contextAwareComponentPrefix);
                            var eventComponent = ComponentDeclaration.FromEvent(component,
                                CombinedNamespace(component.Namespace, eventStrings.EventListenerComponent),
                                eventStrings.EventListenerComponent,
                                ImmutableArray.Create(new MemberDeclaration($"global::System.Collections.Generic.List<{eventStrings.EventListenerInterface}>", "Value")),
                                eventStrings.EventPrefix);

                            allComponents.Add(eventComponent);
                        }
                    }
                }
            }

            return allComponents
                .OrderBy(static component => component.FullName)
                .ToImmutableArray();
        }

        static void OnFullNameOrContextsChanged(SourceProductionContext spc, ComponentDeclaration component)
        {
            ComponentIndex(spc, component);
            Matcher(spc, component);
        }

        static void ComponentIndex(SourceProductionContext spc, ComponentDeclaration component)
        {
            foreach (var context in component.Contexts)
            {
                ComponentIndex(spc, component, context);
            }
        }

        static void ComponentIndex(SourceProductionContext spc, ComponentDeclaration component, string context)
        {
            var contextPrefix = component.ContextPrefix(context);
            var contextAwareComponentPrefix = component.ContextAwareComponentPrefix(contextPrefix);
            var className = $"{contextAwareComponentPrefix}ComponentIndex";
            spc.AddSource(
                GeneratedPath(CombinedNamespace(component.Namespace, className)),
                GeneratedFileHeader(GeneratorSource(nameof(ComponentIndex))) +
                $"using global::{contextPrefix};\n\n" +
                NamespaceDeclaration(component.Namespace,
                    $$"""
                    public static class {{className}}
                    {
                        public static ComponentIndex Index;
                    }

                    """));
        }

        static void Matcher(SourceProductionContext spc, ComponentDeclaration component)
        {
            foreach (var context in component.Contexts)
            {
                Matcher(spc, component, context);
            }
        }

        static void Matcher(SourceProductionContext spc, ComponentDeclaration component, string context)
        {
            var contextPrefix = component.ContextPrefix(context);
            var contextAwareComponentPrefix = component.ContextAwareComponentPrefix(contextPrefix);
            var className = $"{contextAwareComponentPrefix}Matcher";
            spc.AddSource(
                GeneratedPath(CombinedNamespace(component.Namespace, className)),
                GeneratedFileHeader(GeneratorSource(nameof(ComponentIndex))) +
                $"using global::{contextPrefix};\n" +
                $"using static global::{CombinedNamespace(component.Namespace, contextAwareComponentPrefix)}ComponentIndex;\n\n" +
                NamespaceDeclaration(component.Namespace,
                    $$"""
                    public sealed class {{className}}
                    {
                        static global::Entitas.IMatcher<Entity> _matcher;

                        public static global::Entitas.IMatcher<Entity> {{component.ComponentPrefix}}
                        {
                            get
                            {
                                if (_matcher == null)
                                {
                                    var matcher = (global::Entitas.Matcher<Entity>)global::Entitas.Matcher<Entity>.AllOf(Index.Value);
                                    matcher.componentNames = {{context}}.ComponentNames;
                                    _matcher = matcher;
                                }

                                return _matcher;
                            }
                        }
                    }

                    """));
        }

        static void OnFullNameOrMembersOrContextsChanged(SourceProductionContext spc, ComponentDeclaration component)
        {
            EntityExtension(spc, component);
        }

        static void EntityExtension(SourceProductionContext spc, ComponentDeclaration component)
        {
            foreach (var context in component.Contexts)
            {
                EntityExtension(spc, component, context);
            }
        }

        static void EntityExtension(SourceProductionContext spc, ComponentDeclaration component, string context)
        {
            var contextPrefix = component.ContextPrefix(context);
            var contextAwareComponentPrefix = component.ContextAwareComponentPrefix(contextPrefix);
            var className = $"{contextAwareComponentPrefix}EntityExtension";
            string content;
            if (component.Members.Length > 0)
            {
                content = $$"""
                    public static class {{className}}
                    {
                        public static bool Has{{component.ComponentPrefix}}(this Entity entity)
                        {
                            return entity.HasComponent(Index.Value);
                        }

                        public static Entity Add{{component.ComponentPrefix}}(this Entity entity, {{ComponentMethodParams(component)}})
                        {
                            var index = Index.Value;
                            var componentPool = entity.GetComponentPool(index);
                            var component = componentPool.Count > 0
                                ? ({{component.Name}})componentPool.Pop()
                                : new {{component.Name}}();
                    {{ComponentValueAssignments(component)}}
                            entity.AddComponent(index, component);
                            return entity;
                        }

                        public static Entity Replace{{component.ComponentPrefix}}(this Entity entity, {{ComponentMethodParams(component)}})
                        {
                            var index = Index.Value;
                            var componentPool = entity.GetComponentPool(index);
                            var component = componentPool.Count > 0
                                ? ({{component.Name}})componentPool.Pop()
                                : new {{component.Name}}();
                    {{ComponentValueAssignments(component)}}
                            entity.ReplaceComponent(index, component);
                            return entity;
                        }

                        public static Entity Remove{{component.ComponentPrefix}}(this Entity entity)
                        {
                            entity.RemoveComponent(Index.Value);
                            return entity;
                        }

                        public static {{component.Name}} Get{{component.ComponentPrefix}}(this Entity entity)
                        {
                            return ({{component.Name}})entity.GetComponent(Index.Value);
                        }
                    }

                    """;
            }
            else
            {
                content = $$"""
                    public static class {{className}}
                    {
                        static readonly {{component.Name}} Single{{component.Name}} = new {{component.Name}}();

                        public static bool Has{{component.ComponentPrefix}}(this Entity entity)
                        {
                            return entity.HasComponent(Index.Value);
                        }

                        public static Entity Add{{component.ComponentPrefix}}(this Entity entity)
                        {
                            entity.AddComponent(Index.Value, Single{{component.Name}});
                            return entity;
                        }

                        public static Entity Replace{{component.ComponentPrefix}}(this Entity entity)
                        {
                            entity.ReplaceComponent(Index.Value, Single{{component.Name}});
                            return entity;
                        }

                        public static Entity Remove{{component.ComponentPrefix}}(this Entity entity)
                        {
                            entity.RemoveComponent(Index.Value);
                            return entity;
                        }

                        public static {{component.Name}} Get{{component.ComponentPrefix}}(this Entity entity)
                        {
                            return ({{component.Name}})entity.GetComponent(Index.Value);
                        }
                    }

                    """;
            }

            spc.AddSource(
                GeneratedPath(CombinedNamespace(component.Namespace, className)),
                GeneratedFileHeader(GeneratorSource(nameof(EntityExtension))) +
                $"using global::{contextPrefix};\n" +
                $"using static global::{CombinedNamespace(component.Namespace, contextAwareComponentPrefix)}ComponentIndex;\n\n" +
                NamespaceDeclaration(component.Namespace, content));
        }

        static void OnFullNameOrMembersOrContextsOrIsUniqueChanged(SourceProductionContext spc, ComponentDeclaration component)
        {
            if (component.IsUnique)
            {
                ContextExtension(spc, component);
            }
        }

        static void ContextExtension(SourceProductionContext spc, ComponentDeclaration component)
        {
            foreach (var context in component.Contexts)
            {
                ContextExtension(spc, component, context);
            }
        }

        static void ContextExtension(SourceProductionContext spc, ComponentDeclaration component, string context)
        {
            var contextPrefix = component.ContextPrefix(context);
            var contextAwareComponentPrefix = component.ContextAwareComponentPrefix(contextPrefix);
            var className = $"{contextAwareComponentPrefix}ContextExtension";
            string content;
            if (component.Members.Length > 0)
            {
                content = $$"""
                    public static class {{className}}
                    {
                        public static bool Has{{component.ComponentPrefix}}(this {{context}} context)
                        {
                            return context.Get{{component.ComponentPrefix}}Entity() != null;
                        }

                        public static Entity Set{{component.ComponentPrefix}}(this {{context}} context, {{ComponentMethodParams(component)}})
                        {
                            if (context.Has{{component.ComponentPrefix}}())
                            {
                                throw new global::Entitas.EntitasException(
                                    $"Could not set {{component.ComponentPrefix}}!\n{context} already has an entity with {{component.FullName}}!",
                                    "You should check if the context already has a {{component.ComponentPrefix}}Entity before setting it or use context.Replace{{component.ComponentPrefix}}()."
                                );
                            }

                            return context.CreateEntity().Add{{component.ComponentPrefix}}({{ComponentMethodArgs(component)}});
                        }

                        public static Entity Replace{{component.ComponentPrefix}}(this {{context}} context, {{ComponentMethodParams(component)}})
                        {
                            var entity = context.Get{{component.ComponentPrefix}}Entity();
                            if (entity == null)
                                entity = context.CreateEntity().Add{{component.ComponentPrefix}}({{ComponentMethodArgs(component)}});
                            else
                                entity.Replace{{component.ComponentPrefix}}({{ComponentMethodArgs(component)}});

                            return entity;
                        }

                        public static void Remove{{component.ComponentPrefix}}(this {{context}} context)
                        {
                            context.Get{{component.ComponentPrefix}}Entity().Destroy();
                        }

                        public static Entity Get{{component.ComponentPrefix}}Entity(this {{context}} context)
                        {
                            return context.GetGroup({{contextAwareComponentPrefix}}Matcher.{{component.ComponentPrefix}}).GetSingleEntity();
                        }

                        public static {{component.Name}} Get{{component.ComponentPrefix}}(this {{context}} context)
                        {
                            return context.Get{{component.ComponentPrefix}}Entity().Get{{component.ComponentPrefix}}();
                        }
                    }

                    """;
            }
            else
            {
                content = $$"""
                    public static class {{className}}
                    {
                        public static bool Has{{component.ComponentPrefix}}(this {{context}} context)
                        {
                            return context.Get{{component.ComponentPrefix}}Entity() != null;
                        }

                        public static Entity Set{{component.ComponentPrefix}}(this {{context}} context)
                        {
                            return context.Get{{component.ComponentPrefix}}Entity() ?? context.CreateEntity().Add{{component.ComponentPrefix}}();
                        }

                        public static void Unset{{component.ComponentPrefix}}(this {{context}} context)
                        {
                            context.Get{{component.ComponentPrefix}}Entity()?.Destroy();
                        }

                        public static Entity Get{{component.ComponentPrefix}}Entity(this {{context}} context)
                        {
                            return context.GetGroup({{contextAwareComponentPrefix}}Matcher.{{component.ComponentPrefix}}).GetSingleEntity();
                        }
                    }

                    """;
            }

            spc.AddSource(
                GeneratedPath(CombinedNamespace(component.Namespace, className)),
                GeneratedFileHeader(GeneratorSource(nameof(ContextExtension))) +
                $"using global::{contextPrefix};\n\n" +
                NamespaceDeclaration(component.Namespace, content));
        }

        static void OnFullNameOrContextsOrEventsChanged(SourceProductionContext spc, ComponentDeclaration component)
        {
            if (component.Events.Length > 0)
            {
                Events(spc, component);
            }
        }

        static void Events(SourceProductionContext spc, ComponentDeclaration component)
        {
            foreach (var context in component.Contexts)
            {
                Events(spc, component, context);
            }
        }

        static void Events(SourceProductionContext spc, ComponentDeclaration component, string context)
        {
            var contextPrefix = component.ContextPrefix(context);
            var contextAwareComponentPrefix = component.ContextAwareComponentPrefix(contextPrefix);
            foreach (var @event in component.Events)
            {
                var eventStrings = new EventStrings(@event, component.ComponentPrefix, contextAwareComponentPrefix);
                spc.AddSource(
                    GeneratedPath(CombinedNamespace(component.Namespace, eventStrings.EventListenerComponent)),
                    GeneratedFileHeader(GeneratorSource(nameof(Events))) +
                    $"using global::{contextPrefix};\n\n" +
                    NamespaceDeclaration(component.Namespace,
                        $$"""
                        public interface {{eventStrings.EventListenerInterface}}
                        {
                            void {{eventStrings.EventMethod}}(Entity entity, {{ComponentMethodParams(component)}});
                        }

                        public sealed class {{eventStrings.EventListenerComponent}} : global::Entitas.IComponent
                        {
                            public global::System.Collections.Generic.List<{{eventStrings.EventListenerInterface}}> Value;
                        }

                        public static class {{eventStrings.EventListener}}EventEntityExtension
                        {
                            public static void Add{{eventStrings.EventPrefix}}(this Entity entity, {{eventStrings.EventListenerInterface}} value)
                            {
                                var listeners = entity.Has{{eventStrings.EventPrefix}}()
                                    ? entity.Get{{eventStrings.EventPrefix}}().Value
                                    : new global::System.Collections.Generic.List<{{eventStrings.EventListenerInterface}}>();
                                listeners.Add(value);
                                entity.Replace{{eventStrings.EventPrefix}}(listeners);
                            }

                            public static void Remove{{eventStrings.EventPrefix}}(this Entity entity, {{eventStrings.EventListenerInterface}} value, bool removeComponentWhenEmpty = true)
                            {
                                var listeners = entity.Get{{eventStrings.EventPrefix}}().Value;
                                listeners.Remove(value);
                                if (removeComponentWhenEmpty && listeners.Count == 0)
                                {
                                    entity.Remove{{eventStrings.EventPrefix}}();
                                }
                                else
                                {
                                    entity.Replace{{eventStrings.EventPrefix}}(listeners);
                                }
                            }
                        }
                        """));

                var eventComponent = ComponentDeclaration.FromEvent(component,
                    CombinedNamespace(component.Namespace, eventStrings.EventListenerComponent),
                    eventStrings.EventListenerComponent,
                    ImmutableArray.Create(new MemberDeclaration($"global::System.Collections.Generic.List<{eventStrings.EventListenerInterface}>", "Value")),
                    eventStrings.EventPrefix);

                ComponentIndex(
                    spc,
                    eventComponent,
                    context
                );

                EntityExtension(
                    spc,
                    eventComponent,
                    context);
            }
        }

        static string ComponentMethodParams(ComponentDeclaration component)
        {
            return string.Join(", ", component.Members.Select(static member => $"{member.Type} {member.ValidLowerFirstName}"));
        }

        static string ComponentMethodArgs(ComponentDeclaration component)
        {
            return string.Join(", ", component.Members.Select(static member => $"{member.ValidLowerFirstName}"));
        }

        static string ComponentValueAssignments(ComponentDeclaration component)
        {
            return string.Join("\n", component.Members.Select(static member => $"        component.{member.Name} = {member.ValidLowerFirstName};"));
        }

        static void OnContextInitializationChanged(SourceProductionContext spc, ContextInitializationMethodDeclaration method)
        {
            ContextInitializationMethod(spc, method);
        }

        static void ContextInitializationMethod(SourceProductionContext spc, ContextInitializationMethodDeclaration method)
        {
            spc.AddSource(
                GeneratedPath(CombinedNamespace(method.Namespace, $"{method.Class}.{method.Name}.ContextInitialization")),
                GeneratedFileHeader(GeneratorSource(nameof(ContextInitializationMethod))) +
                $"using global::{method.FullContextPrefix};\n\n" +
                NamespaceDeclaration(method.Namespace,
                    $$"""
                    public static partial class {{method.Class}}
                    {
                        public static partial void {{method.Name}}()
                        {
                    {{ComponentIndexAssignments(method, method.Components)}}

                            {{method.Context}}.ComponentNames = new string[]
                            {
                    {{ComponentNames(method.Components)}}
                            };

                            {{method.Context}}.ComponentTypes = new global::System.Type[]
                            {
                    {{ComponentTypes(method.Components)}}
                            };
                        }
                    }

                    """));

            static string ComponentIndexAssignments(ContextInitializationMethodDeclaration method, ImmutableArray<ComponentDeclaration> components)
            {
                return string.Join("\n", components.Select((component, i) =>
                {
                    var contextPrefix = "global::" + CombinedNamespace(component.Namespace, method.FullContextPrefix.Replace(".", string.Empty));
                    return $"        {contextPrefix}{component.ComponentPrefix}ComponentIndex.Index = new ComponentIndex({i});";
                }));
            }

            static string ComponentNames(ImmutableArray<ComponentDeclaration> components)
            {
                return string.Join(",\n", components.Select(static component => $"            \"{component.FullName.RemoveSuffix("Component")}\""));
            }

            static string ComponentTypes(ImmutableArray<ComponentDeclaration> components)
            {
                return string.Join(",\n", components.Select(static component => $"            typeof(global::{component.FullName})"));
            }
        }

        static string GeneratorSource(string source)
        {
            return $"{typeof(ComponentGenerator).FullName}.{source}";
        }
    }
}
