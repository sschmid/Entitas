using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Extensions;
using DesperateDevs.Roslyn;
using DesperateDevs.Serialization;
using Entitas.CodeGeneration.Attributes;
using Entitas.CodeGeneration.Plugins;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Entitas.Roslyn.CodeGeneration.Plugins
{
    public class ContextsComponentDataProvider : IComponentDataProvider, IConfigurable
    {
        public Dictionary<string, string> DefaultProperties => _contextNamesConfig.DefaultProperties;

        readonly ContextNamesConfig _contextNamesConfig = new ContextNamesConfig();

        public void Configure(Preferences preferences)
        {
            _contextNamesConfig.Configure(preferences);
        }

        public void Provide(INamedTypeSymbol type, ComponentData data)
        {
            var contextNames = GetContextNamesOrDefault(type);
            data.SetContextNames(contextNames);
        }

        public string[] GetContextNames(INamedTypeSymbol type)
        {
            var contextNames = new List<string>();
            var contextAttribute = typeof(ContextAttribute).ToCompilableString();
            foreach (var attribute in type.GetAttributes())
            {
                var contextNameCandidate = attribute.AttributeClass.ToString().Replace("Attribute", string.Empty);
                if (attribute.AttributeClass.BaseType == null && _contextNamesConfig.contextNames.Contains(contextNameCandidate))
                {
                    // Possible compiler error. Just take the attribute name
                    contextNames.Add(contextNameCandidate);
                }
                else if (attribute.AttributeClass.BaseType != null && attribute.AttributeClass.BaseType.ToCompilableString() == contextAttribute)
                {
                    // Generated context attribute
                    var declaration = attribute.AttributeConstructor.DeclaringSyntaxReferences.First().GetSyntax();
                    var baseConstructorInit = (ConstructorInitializerSyntax)declaration.DescendantNodes().First(node => node.IsKind(SyntaxKind.BaseConstructorInitializer));
                    var argument = (LiteralExpressionSyntax)baseConstructorInit.ArgumentList.Arguments.First().Expression;
                    var name = argument.ToString().Replace("\"", string.Empty);
                    contextNames.Add(name);
                }
                else if (attribute.AttributeClass.ToCompilableString().Contains(contextAttribute))
                {
                    // Entitas.CodeGeneration.Attributes.ContextAttribute
                    var name = (string)attribute.ConstructorArguments.First().Value;
                    contextNames.Add(name);
                }
            }

            return contextNames.ToArray();
        }

        public string[] GetContextNamesOrDefault(INamedTypeSymbol type)
        {
            var contextNames = GetContextNames(type);
            if (contextNames.Length == 0)
                contextNames = new[] {_contextNamesConfig.contextNames[0]};

            return contextNames;
        }
    }
}
