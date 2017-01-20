using System;
using System.Collections.Generic;
using System.Linq;
using Entitas.Utils;

namespace Entitas.CodeGenerator {

    public static class ComponentCodeGeneratorDataExtension {

        public static Type GetType(this CodeGeneratorData data) {
            return (Type)data[ComponentDataProvider.COMPONENT_TYPE];
        }

        public static string GetFullTypeName(this CodeGeneratorData data) {
            return (string)data[ComponentDataProvider.COMPONENT_FULL_TYPE_NAME];
        }

        public static string GetShortTypeName(this CodeGeneratorData data) {
            var fullTypeName = GetFullTypeName(data);
            var nameSplit = fullTypeName.Split('.');
            return nameSplit[nameSplit.Length - 1];
        }

        public static List<PublicMemberInfo> GetMemberInfos(this CodeGeneratorData data) {
            return (List<PublicMemberInfo>)data[ComponentDataProvider.COMPONENT_MEMBER_INFOS];
        }

        public static string[] GetContexts(this CodeGeneratorData data) {
            return (string[])data[ComponentDataProvider.COMPONENT_CONTEXTS];
        }

        public static bool IsUnique(this CodeGeneratorData data) {
            return (bool)data[ComponentDataProvider.COMPONENT_IS_UNIQUE];
        }

        public static string GetUniqueComponentPrefix(this CodeGeneratorData data) {
            return (string)data[ComponentDataProvider.COMPONENT_UNIQUE_PREFIX];
        }

        public static bool IsComponent(this CodeGeneratorData data) {
            return (bool)data[ComponentDataProvider.COMPONENT_GENERATE_COMPONENT];
        }

        public static bool ShouldGenerateMethods(this CodeGeneratorData data) {
            return (bool)data[ComponentDataProvider.COMPONENT_GENERATE_METHODS];
        }

        public static bool ShouldGenerateIndex(this CodeGeneratorData data) {
            return (bool)data[ComponentDataProvider.COMPONENT_GENERATE_INDEX];
        }

        public static bool ShouldHideInBlueprintInspector(this CodeGeneratorData data) {
            return (bool)data[ComponentDataProvider.COMPONENT_HIDE_IN_BLUEPRINT_INSPECTOR];
        }
    }

    public class ComponentDataProvider : ICodeGeneratorDataProvider {

        public const string COMPONENT_TYPE = "component_type";
        public const string COMPONENT_FULL_TYPE_NAME = "component_fullTypeName";
        public const string COMPONENT_MEMBER_INFOS = "component_memberInfos";
        public const string COMPONENT_CONTEXTS = "component_contexts";
        public const string COMPONENT_IS_UNIQUE = "component_isUnique";
        public const string COMPONENT_UNIQUE_PREFIX = "component_uniquePrefix";
        public const string COMPONENT_GENERATE_COMPONENT = "component_generateComponent";
        public const string COMPONENT_GENERATE_METHODS = "component_generateMethods";
        public const string COMPONENT_GENERATE_INDEX = "component_generateIndex";
        public const string COMPONENT_HIDE_IN_BLUEPRINT_INSPECTOR = "component_HideInBlueprintInspector";

        readonly Type[] _types;

        public ComponentDataProvider(Type[] types) {
            _types = types;
        }

        public CodeGeneratorData[] GetData() {
            var dataFromComponents = _types
                .Where(type => !type.IsInterface)
                .Where(type => !type.IsAbstract)
                .Where(type => type.GetInterfaces().Any(i => i.FullName == "Entitas.IComponent"))
                .Select(type => createDataForComponent(type));

            var dataFromNonComponents = _types
                .Where(type => !type.IsGenericType)
                .Where(type => !type.GetInterfaces().Any(i => i.FullName == "Entitas.IComponent"))
                .Where(type => getContexts(type).Length > 0)
                .SelectMany(type => createDataForNonComponent(type));

            var generatedComponentsLookup = dataFromNonComponents.ToLookup(data => data[COMPONENT_FULL_TYPE_NAME]);

            return dataFromComponents
                .Where(data => !generatedComponentsLookup.Contains(data[COMPONENT_FULL_TYPE_NAME]))
                .Concat(dataFromNonComponents)
                .ToArray();
        }

        CodeGeneratorData createDataForComponent(Type type) {
            var data = new CodeGeneratorData(GetType().FullName);

            data[COMPONENT_TYPE] = type;
            data[COMPONENT_FULL_TYPE_NAME] = type.ToCompilableString();
            data[COMPONENT_MEMBER_INFOS] = type.GetPublicMemberInfos();

            data[COMPONENT_CONTEXTS] = getContexts(type);
            data[COMPONENT_IS_UNIQUE] = getIsUnique(type);
            data[COMPONENT_UNIQUE_PREFIX] = getUniqueComponentPrefix(type);

            data[COMPONENT_GENERATE_COMPONENT] = false;

            data[COMPONENT_GENERATE_METHODS] = getGenerateMethods(type);
            data[COMPONENT_GENERATE_INDEX] = getGenerateIndex(type);
            data[COMPONENT_HIDE_IN_BLUEPRINT_INSPECTOR] = getHideInBlueprintInspector(type);

            return data;
        }

        CodeGeneratorData[] createDataForNonComponent(Type type) {
            return getComponentNames(type).Select(componentName => {
                var data = new CodeGeneratorData(GetType().FullName);

                data[COMPONENT_TYPE] = type;
                data[COMPONENT_FULL_TYPE_NAME] = componentName;
                data[COMPONENT_MEMBER_INFOS] = new List<PublicMemberInfo> { new PublicMemberInfo(type, "value") };

                data[COMPONENT_CONTEXTS] = getContexts(type);
                data[COMPONENT_IS_UNIQUE] = getIsUnique(type);
                data[COMPONENT_UNIQUE_PREFIX] = getUniqueComponentPrefix(type);

                data[COMPONENT_GENERATE_COMPONENT] = true;

                data[COMPONENT_GENERATE_METHODS] = getGenerateMethods(type);
                data[COMPONENT_GENERATE_INDEX] = getGenerateIndex(type);
                data[COMPONENT_HIDE_IN_BLUEPRINT_INSPECTOR] = getHideInBlueprintInspector(type);

                return data;
            }).ToArray();
        }

        string[] getContexts(Type type) {
            return Attribute.GetCustomAttributes(type)
                            .Where(attr => isTypeOrHasBaseType(attr.GetType(), "Entitas.CodeGenerator.ContextAttribute"))
                            .Select(attr => attr.GetType().GetField("contextName").GetValue(attr) as string)
                            .OrderBy(contextName => contextName)
                            .ToArray();
        }

        bool getIsUnique(Type type) {
            return Attribute.GetCustomAttributes(type)
                            .Any(attr => attr.GetType().FullName == "Entitas.CodeGenerator.UniqueAttribute");
        }

        string getUniqueComponentPrefix(Type type) {
            var attr = Attribute.GetCustomAttributes(type)
                                .SingleOrDefault(a => isTypeOrHasBaseType(a.GetType(), "Entitas.CodeGenerator.CustomPrefixAttribute"));

            return attr == null ? "is" : (string)attr.GetType().GetField("prefix").GetValue(attr);
        }

        bool getGenerateMethods(Type type) {
            return Attribute.GetCustomAttributes(type)
                            .All(attr => attr.GetType().FullName != "Entitas.CodeGenerator.DontGenerateAttribute");
        }

        bool getGenerateIndex(Type type) {
            var attr = Attribute.GetCustomAttributes(type)
                                .SingleOrDefault(a => isTypeOrHasBaseType(a.GetType(), "Entitas.CodeGenerator.DontGenerateAttribute"));

            return attr == null || (bool)attr.GetType().GetField("generateIndex").GetValue(attr);
        }

        bool getHideInBlueprintInspector(Type type) {
            var attr = Attribute.GetCustomAttributes(type)
                                .SingleOrDefault(a => isTypeOrHasBaseType(a.GetType(), "Entitas.Serialization.Blueprints.HideInBlueprintInspectorAttribute"));

            return attr != null;
        }

        string[] getComponentNames(Type type) {
            var attr = Attribute.GetCustomAttributes(type)
                                .SingleOrDefault(a => isTypeOrHasBaseType(a.GetType(), "Entitas.CodeGenerator.CustomComponentNameAttribute"));

            if(attr == null) {
                var nameSplit = type.ToCompilableString().Split('.');
                var componentName = nameSplit[nameSplit.Length - 1].AddComponentSuffix();
                return new[] { componentName };
            }

            return (string[])attr.GetType().GetField("componentNames").GetValue(attr);
        }

        bool hasBaseType(Type type, string fullTypeName) {
            if(type.FullName == fullTypeName) {
                return false;
            }

            return isTypeOrHasBaseType(type, fullTypeName);
        }

        bool isTypeOrHasBaseType(Type type, string fullTypeName) {
            var t = type;
            while(t != null) {
                if(t.FullName == fullTypeName) {
                    return true;
                }
                t = t.BaseType;
            }

            return false;
        }
    }
}
