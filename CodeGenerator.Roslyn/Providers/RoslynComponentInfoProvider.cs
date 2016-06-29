using System.Collections.Generic;
using System.Linq;
using CSharpCodeGenerator;
using CSharpCodeGenerator.DataStructures;
using Entitas.CodeGeneration;
using Entitas.Serialization;

namespace CodeGenerator.Roslyn.Providers
{
    public class RoslynComponentInfoProvider : ICodeGeneratorDataProvider
    {
        private const string POOL_ATTRIBUTE_IDENTIFIER = "Pool";
        private const string ICOMPONENT_INTERFACE_NAME = "Entitas.IComponent";
        private ProjectStructure _project;

        public string[] blueprintNames { get; }

        public ComponentInfo[] componentInfos { get; }

        public string[] poolNames { get; }
        
        public RoslynComponentInfoProvider(ProjectStructure project, string[] poolNames, string[] blueprintNames)
        {
            this.poolNames = poolNames;
            this.blueprintNames = blueprintNames;

            if (project != null)
            {
                _project = project;
                componentInfos = GetComponentInfos().ToArray();
            }
        }

        private IEnumerable<ComponentInfo> GetComponentInfos()
        {
            return from document in _project.Documents.AsParallel()
                        from classStructure in document.Classes
                            where IsValidIComponentClass(classStructure)
                            select GetComponentInfo(classStructure);
        }

        private bool IsValidIComponentClass(ClassStructure classNode)
        {
            if (classNode.ModifierFlags.HasFlag(ModifierFlags.Abstract))
                return false;
            if (classNode.BaseTypes == null)
                return false;

            foreach (var baseType in classNode.BaseTypes)
            {
                var name = _project.GetFullTypeName(baseType);
                if (name.Equals(ICOMPONENT_INTERFACE_NAME))
                {
                    return true;
                }
            }
            return false;
        }

        private ComponentInfo GetComponentInfo(ClassStructure classDeclarationNode)
        {
            var pools = GetPools(classDeclarationNode.Attributes);
            var fullClassName = classDeclarationNode.FullClassName;
            List<PublicMemberInfo> publicMemberInfos = GetPublicMemberInfos(classDeclarationNode.Fields).ToList();
            var isSingleEntity = false;
            return new ComponentInfo(fullClassName, publicMemberInfos, pools.ToArray(),
                isSingleEntity: isSingleEntity, generateMethods: true, generateIndex: true, singleComponentPrefix: "is");
        }

        private IEnumerable<PublicMemberInfo> GetPublicMemberInfos(IEnumerable<FieldStructure> fields)
        {
            return from field in fields
                        where field.AccessModifier == AccessModifier.Public && field.ModifierFlags.Equals(ModifierFlags.None)
                        select new PublicMemberInfo(_project.GetFullTypeName(field.DeclarationType), field.Identifier);
        }

        private static IEnumerable<string> GetPools(IEnumerable<AttributeStructure> attributes)
        {
            return attributes.Where(a => a.Identifier.Equals(POOL_ATTRIBUTE_IDENTIFIER)).Select(GetPoolValueWithoutQuotes);
        }

        private static string GetPoolValueWithoutQuotes(AttributeStructure attribute)
        {
            return attribute.Values.First().Replace("\"", string.Empty);
        }
    }
}

