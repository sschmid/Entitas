using System;
using System.Collections.Generic;
using System.Linq;
using CSharpCodeGenerator;
using CSharpCodeGenerator.DataStructures;
using Entitas.CodeGeneration;
using Entitas.Serialization;

namespace Entitas.CodeGenerator
{
    public class RoslynComponentInfoProvider : ICodeGeneratorDataProvider
    {
        private const string POOL_ATTRIBUTE_IDENTIFIER = "Pool";
        private readonly ProjectStructure _project;

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
                componentInfos = GetComponentInfos();
            }
        }

        private ComponentInfo[] GetComponentInfos()
        {
            // get all syntax nodes where a class is defined
            // get all information from class
            var componentInfos = new List<ComponentInfo>();
            foreach (var document in _project.Documents)
            {
                foreach (var classStructure in document.Classes)
                {
                    if (IsValidIComponentClass(classStructure))
                    {
                        componentInfos.Add(GetComponentInfo(classStructure));
                    }
                }
            }
            return componentInfos.ToArray();
        }

        private bool IsValidIComponentClass(ClassStructure classNode)
        {
            if (classNode.ModifierFlags.HasFlag(ModifierFlags.Abstract))
                return false;
            return true;
        }

        private ComponentInfo GetComponentInfo(ClassStructure classDeclarationNode)
        {
            var pools = GetPools(classDeclarationNode.Attributes);
            var fullClassName = classDeclarationNode.FullClassName;
            List<PublicMemberInfo> publicMemberInfos = GetPublicMemberInfos(classDeclarationNode.Fields);
            var isSingleEntity = false;
            return new ComponentInfo(fullClassName, publicMemberInfos, pools.ToArray(),
                isSingleEntity, generateMethods: true, generateIndex: true, singleComponentPrefix: "is");
        }

        private List<PublicMemberInfo> GetPublicMemberInfos(IEnumerable<FieldStructure> fields)
        {
            var result = new List<PublicMemberInfo>();
            foreach (var field in fields)
            {
                if (field.AccessModifier == AccessModifier.Public && field.ModifierFlags.Equals(ModifierFlags.None))
                {
                    result.Add(new PublicMemberInfo(_project.GetFullMetadataName(field), field.Identifier));
                }
            }
            return result;
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

