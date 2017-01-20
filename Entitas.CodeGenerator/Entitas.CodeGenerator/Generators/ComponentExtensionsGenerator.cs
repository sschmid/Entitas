using System.Collections.Generic;
using System.Linq;
using Entitas.Utils;

namespace Entitas.CodeGenerator {

    public class ComponentExtensionsGenerator : ICodeGenerator {

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            var generatorName = GetType().FullName;
            return data
                .Where(d => d.dataProviderName == typeof(ComponentDataProvider).FullName)
                .Where(d => d.ShouldGenerateMethods())
                .Select(d => generateComponentExtensions(d, generatorName))
                .SelectMany(codeGenFile => codeGenFile)
                .ToArray();
        }

        static CodeGenFile[] generateComponentExtensions(CodeGeneratorData data, string generatorName) {
            return data.GetContexts()
                    .Select(contextName => generateComponentExtension(data, contextName, generatorName))
                    .SelectMany(file => file)
                    .GroupBy(file => file.fileName)
                    .Select(group => group.First())
                    .ToArray();
        }

        static CodeGenFile[] generateComponentExtension(CodeGeneratorData data, string context, string generatorName) {
            var files = new List<CodeGenFile>();

            if(!data.IsComponent()) {
                files.Add(new CodeGenFile(
                    "Components/" + data.GetFullTypeName(),
                    addUsings("Entitas") + generateComponent(data),
                    generatorName));
            }

            var code = addUsings("Entitas");
            code += addEntityMethods(data, context);
            if(data.IsUnique()) {
                code += addContextMethods(data, context);
            }
            code += addMatcher(data, context);

            files.Add(new CodeGenFile(
                context + "/Components/" + context + data.GetFullTypeName(),
                code, generatorName));

            return files.ToArray();
        }

        static string generateComponent(CodeGeneratorData data) {
            const string hideInBlueprintInspector = "[Entitas.Serialization.Blueprints.HideInBlueprintInspector]\n";
            const string componentFormat = @"public class {0} : IComponent {{

    public {1} {2};
}}
";
            var memberInfo = data.GetMemberInfos()[0];
            var code = string.Format(componentFormat, data.GetFullTypeName(), memberInfo.type, memberInfo.name);
            return data.ShouldHideInBlueprintInspector()
                        ? hideInBlueprintInspector + code
                        : code;
        }

        static string addUsings(params string[] usings) {
            return string.Join("\n", usings
                .Select(name => "using " + name + ";")
                .ToArray()) + "\n\n";
        }

        /*
         *
         * ENTITY METHODS
         *
         */

        static string addEntityMethods(CodeGeneratorData data, string context) {
            return addEntityClassHeader(data, context)
                    + addGetMethods(data, context)
                    + addHasMethods(data, context)
                    + addAddMethods(data, context)
                    + addReplaceMethods(data, context)
                    + addRemoveMethods(data, context)
                    + addCloseClass();
        }

        static string addEntityClassHeader(CodeGeneratorData data, string context) {
            return buildString(data, "public sealed partial class $TagEntity : Entity {\n", context);
        }

        static string addGetMethods(CodeGeneratorData data, string context) {
            var getMethod = data.IsUnique()
                    ? "\n    static readonly $Type $nameComponent = new $Type();\n"
                    : "\n    public $Type $name { get { return ($Type)GetComponent($Ids.$Name); } }\n";

            return buildString(data, getMethod, context);
        }

        static string addHasMethods(CodeGeneratorData data, string context) {
            var hasMethod = data.IsUnique() ? @"
    public bool $prefix$Name {
        get { return HasComponent($Ids.$Name); }
        set {
            if(value != $prefix$Name) {
                if(value) {
                    AddComponent($Ids.$Name, $nameComponent);
                } else {
                    RemoveComponent($Ids.$Name);
                }
            }
        }
    }
" : @"    public bool has$Name { get { return HasComponent($Ids.$Name); } }
";
            return buildString(data, hasMethod, context);
        }

        static string addAddMethods(CodeGeneratorData data, string context) {
            return data.IsUnique() ? string.Empty : buildString(data, @"
    public void Add$Name($typedArgs) {
        var component = CreateComponent<$Type>($Ids.$Name);
$assign
        AddComponent($Ids.$Name, component);
    }
", context);
        }

        static string addReplaceMethods(CodeGeneratorData data, string context) {
            return data.IsUnique() ? string.Empty : buildString(data, @"
    public void Replace$Name($typedArgs) {
        var component = CreateComponent<$Type>($Ids.$Name);
$assign
        ReplaceComponent($Ids.$Name, component);
    }
", context);
        }

        static string addRemoveMethods(CodeGeneratorData data, string context) {
            return data.IsUnique() ? string.Empty : buildString(data, @"
    public void Remove$Name() {
        RemoveComponent($Ids.$Name);
    }
", context);
        }

        /*
         *
         * CONTEXT METHODS
         *
         */

        static string addContextMethods(CodeGeneratorData data, string context) {
            return addContextClassHeader(data, context)
                    + addContextGetMethods(data, context)
                    + addContextHasMethods(data, context)
                    + addContextAddMethods(data, context)
                    + addContextReplaceMethods(data, context)
                    + addContextRemoveMethods(data, context)
                    + addCloseClass();
        }

        static string addContextClassHeader(CodeGeneratorData data, string context) {
            return buildString(data, "\npublic sealed partial class $TagContext : Context<$TagEntity> {\n", context);
        }

        static string addContextGetMethods(CodeGeneratorData data, string context) {
            var getMehod = data.IsUnique() ? @"
    public $TagEntity $nameEntity { get { return GetGroup($TagMatcher.$Name).GetSingleEntity(); } }
" : @"
    public $TagEntity $nameEntity { get { return GetGroup($TagMatcher.$Name).GetSingleEntity(); } }
    public $Type $name { get { return $nameEntity.$name; } }
";
            return buildString(data, getMehod, context);
        }

        static string addContextHasMethods(CodeGeneratorData data, string context) {
            var hasMethod = data.IsUnique() ? @"
    public bool $prefix$Name {
        get { return $nameEntity != null; }
        set {
            var entity = $nameEntity;
            if(value != (entity != null)) {
                if(value) {
                    CreateEntity().$prefix$Name = true;
                } else {
                    DestroyEntity(entity);
                }
            }
        }
    }
" : @"    public bool has$Name { get { return $nameEntity != null; } }
";
            return buildString(data, hasMethod, context);
        }

        static object addContextAddMethods(CodeGeneratorData data, string context) {
            return data.IsUnique() ? string.Empty : buildString(data, @"
    public $TagEntity Set$Name($typedArgs) {
        if(has$Name) {
            throw new EntitasException(""Could not set $name!\n"" + this + "" already has an entity with $Type!"",
                ""You should check if the context already has a $nameEntity before setting it or use context.Replace$Name()."");
        }
        var entity = CreateEntity();
        entity.Add$Name($args);
        return entity;
    }
", context);
        }

        static string addContextReplaceMethods(CodeGeneratorData data, string context) {
            return data.IsUnique() ? string.Empty : buildString(data, @"
    public void Replace$Name($typedArgs) {
        var entity = $nameEntity;
        if(entity == null) {
            entity = Set$Name($args);
        } else {
            entity.Replace$Name($args);
        }
    }
", context);
        }

        static string addContextRemoveMethods(CodeGeneratorData data, string context) {
            return data.IsUnique() ? string.Empty : buildString(data, @"
    public void Remove$Name() {
        DestroyEntity($nameEntity);
    }
", context);
        }

        /*
        *
        * MATCHER
        *
        */

        static string addMatcher(CodeGeneratorData data, string context) {
            const string matcherFormat = @"
public sealed partial class $TagMatcher {

    static IMatcher<$TagEntity> _matcher$Name;

    public static IMatcher<$TagEntity> $Name {
        get {
            if(_matcher$Name == null) {
                var matcher = (Matcher<$TagEntity>)Matcher<$TagEntity>.AllOf($Ids.$Name);
                matcher.componentNames = $Ids.componentNames;
                _matcher$Name = matcher;
            }

            return _matcher$Name;
        }
    }
}
";

            return buildString(data, matcherFormat, context);
        }

        /*
         *
         * HELPERS
         *
         */

        static string buildString(CodeGeneratorData data, string format, string context) {
            format = createFormatString(format);
            var a0_type = data.GetFullTypeName();
            var typeName = data.GetShortTypeName();
            var a1_name = typeName.RemoveComponentSuffix();
            var a2_lowercaseName = a1_name.LowercaseFirst();
            var a3_context = context;
            var a4_componentLookup = context + ComponentIndicesGenerator.COMPONENT_LOOKUP;
            var memberInfos = data.GetMemberInfos();
            var a5_memberNamesWithType = memberNamesWithType(memberInfos);
            var a6_memberAssigns = memberAssignments(memberInfos);
            var a7_memberNames = memberNames(memberInfos);
            var prefix = data.GetUniqueComponentPrefix();
            var a8_prefix = prefix.UppercaseFirst();
            var a9_lowercasePrefix = prefix.LowercaseFirst();

            return string.Format(format, a0_type, a1_name, a2_lowercaseName,
                a3_context, a4_componentLookup, a5_memberNamesWithType, a6_memberAssigns, a7_memberNames,
                a8_prefix, a9_lowercasePrefix);
        }

        static string createFormatString(string format) {
            return format.Replace("{", "{{")
                        .Replace("}", "}}")
                        .Replace("$Type", "{0}")
                        .Replace("$Name", "{1}")
                        .Replace("$name", "{2}")
                        .Replace("$Tag", "{3}")
                        .Replace("$Ids", "{4}")
                        .Replace("$typedArgs", "{5}")
                        .Replace("$assign", "{6}")
                        .Replace("$args", "{7}")
                        .Replace("$Prefix", "{8}")
                        .Replace("$prefix", "{9}");
        }

        static string memberNamesWithType(List<PublicMemberInfo> memberInfos) {
            var typedArgs = memberInfos
                .Select(info => info.type.ToCompilableString() + " new" + info.name.UppercaseFirst())
                .ToArray();

            return string.Join(", ", typedArgs);
        }

        static string memberAssignments(List<PublicMemberInfo> memberInfos) {
            const string format = "        component.{0} = {1};";
            var assignments = memberInfos.Select(info => {
                var newArg = "new" + info.name.UppercaseFirst();
                return string.Format(format, info.name, newArg);
            }).ToArray();

            return string.Join("\n", assignments);
        }

        static string memberNames(List<PublicMemberInfo> memberInfos) {
            var args = memberInfos.Select(info => "new" + info.name.UppercaseFirst()).ToArray();
            return string.Join(", ", args);
        }

        static string addCloseClass() {
            return "}\n";
        }
    }
}
