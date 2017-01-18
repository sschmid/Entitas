using System.Collections.Generic;
using System.Linq;
using Entitas.Serialization;

namespace Entitas.CodeGenerator {

    public class ComponentExtensionsGenerator : IComponentCodeGenerator {

        public CodeGenFile[] Generate(ComponentInfo[] componentInfos) {
            var generatorName = GetType().FullName;
            return componentInfos
                .Where(info => info.generateMethods)
                .Select(info => generateComponentExtensions(info, generatorName))
                .SelectMany(codeGenFile => codeGenFile)
                .ToArray();
        }

        static CodeGenFile[] generateComponentExtensions(ComponentInfo componentInfo, string generatorName) {
            return componentInfo.contexts
                        .Select(context => generateComponentExtension(componentInfo, context, generatorName))
                        .SelectMany(codeGenFile => codeGenFile)
                        .GroupBy(file => file.fileName)
                        .Select(group => group.First())
                        .ToArray();
        }

        static CodeGenFile[] generateComponentExtension(ComponentInfo componentInfo, string context, string generatorName) {
            var codeGenFiles = new List<CodeGenFile>();

            if(componentInfo.generateComponent) {
                codeGenFiles.Add(new CodeGenFile(
                    "Components/" + componentInfo.fullTypeName,
                    addUsings("Entitas") + generateComponent(componentInfo),
                    generatorName));
            }

            var code = addUsings("Entitas");
            code += addEntityMethods(componentInfo, context);
            if(componentInfo.isSingleEntity) {
                code += addContextMethods(componentInfo, context);
            }
            code += addMatcher(componentInfo, context);

            codeGenFiles.Add(new CodeGenFile(
                context + "/Components/" + context + componentInfo.fullTypeName,
                code, generatorName));

            return codeGenFiles.ToArray();
        }

        static string generateComponent(ComponentInfo componentInfo) {
            const string hideInBlueprintInspector = "[Entitas.Serialization.Blueprints.HideInBlueprintInspector]\n";
            const string componentFormat = @"public class {0} : IComponent {{

    public {1} {2};
}}
";
            var memberInfo = componentInfo.memberInfos[0];
            var code = string.Format(componentFormat, componentInfo.fullTypeName, memberInfo.type, memberInfo.name);
            return componentInfo.hideInBlueprintInspector
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

        static string addEntityMethods(ComponentInfo componentInfo, string context) {
            return addEntityClassHeader(componentInfo, context)
                    + addGetMethods(componentInfo, context)
                    + addHasMethods(componentInfo, context)
                    + addAddMethods(componentInfo, context)
                    + addReplaceMethods(componentInfo, context)
                    + addRemoveMethods(componentInfo, context)
                    + addCloseClass();
        }

        static string addEntityClassHeader(ComponentInfo componentInfo, string context) {
            return buildString(componentInfo, "public sealed partial class $TagEntity : Entity {\n", context);
        }

        static string addGetMethods(ComponentInfo componentInfo, string context) {
            var getMethod = componentInfo.isSingletonComponent
                    ? "\n    static readonly $Type $nameComponent = new $Type();\n"
                    : "\n    public $Type $name { get { return ($Type)GetComponent($Ids.$Name); } }\n";

            return buildString(componentInfo, getMethod, context);
        }

        static string addHasMethods(ComponentInfo componentInfo, string context) {
            var hasMethod = componentInfo.isSingletonComponent ? @"
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
            return buildString(componentInfo, hasMethod, context);
        }

        static string addAddMethods(ComponentInfo componentInfo, string context) {
            return componentInfo.isSingletonComponent ? string.Empty : buildString(componentInfo, @"
    public void Add$Name($typedArgs) {
        var component = CreateComponent<$Type>($Ids.$Name);
$assign
        AddComponent($Ids.$Name, component);
    }
", context);
        }

        static string addReplaceMethods(ComponentInfo componentInfo, string context) {
            return componentInfo.isSingletonComponent ? string.Empty : buildString(componentInfo, @"
    public void Replace$Name($typedArgs) {
        var component = CreateComponent<$Type>($Ids.$Name);
$assign
        ReplaceComponent($Ids.$Name, component);
    }
", context);
        }

        static string addRemoveMethods(ComponentInfo componentInfo, string context) {
            return componentInfo.isSingletonComponent ? string.Empty : buildString(componentInfo, @"
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

        static string addContextMethods(ComponentInfo componentInfo, string context) {
            return addContextClassHeader(componentInfo, context)
                    + addContextGetMethods(componentInfo, context)
                    + addContextHasMethods(componentInfo, context)
                    + addContextAddMethods(componentInfo, context)
                    + addContextReplaceMethods(componentInfo, context)
                    + addContextRemoveMethods(componentInfo, context)
                    + addCloseClass();
        }

        static string addContextClassHeader(ComponentInfo componentInfo, string context) {
            return buildString(componentInfo, "\npublic sealed partial class $TagContext : Context<$TagEntity> {\n", context);
        }

        static string addContextGetMethods(ComponentInfo componentInfo, string context) {
            var getMehod = componentInfo.isSingletonComponent ? @"
    public $TagEntity $nameEntity { get { return GetGroup($TagMatcher.$Name).GetSingleEntity(); } }
" : @"
    public $TagEntity $nameEntity { get { return GetGroup($TagMatcher.$Name).GetSingleEntity(); } }
    public $Type $name { get { return $nameEntity.$name; } }
";
            return buildString(componentInfo, getMehod, context);
        }

        static string addContextHasMethods(ComponentInfo componentInfo, string context) {
            var hasMethod = componentInfo.isSingletonComponent ? @"
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
            return buildString(componentInfo, hasMethod, context);
        }

        static object addContextAddMethods(ComponentInfo componentInfo, string context) {
            return componentInfo.isSingletonComponent ? string.Empty : buildString(componentInfo, @"
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

        static string addContextReplaceMethods(ComponentInfo componentInfo, string context) {
            return componentInfo.isSingletonComponent ? string.Empty : buildString(componentInfo, @"
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

        static string addContextRemoveMethods(ComponentInfo componentInfo, string context) {
            return componentInfo.isSingletonComponent ? string.Empty : buildString(componentInfo, @"
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

       static string addMatcher(ComponentInfo componentInfo, string context) {
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

            return buildString(componentInfo, matcherFormat, context);
        }

        /*
         *
         * HELPERS
         *
         */

        static string buildString(ComponentInfo componentInfo, string format, string context) {
            format = createFormatString(format);
            var a0_type = componentInfo.fullTypeName;
            var a1_name = componentInfo.typeName.RemoveComponentSuffix();
            var a2_lowercaseName = a1_name.LowercaseFirst();
            var a3_context = context;
            var a4_componentLookup = context + CodeGenerator.COMPONENT_LOOKUP;
            var memberInfos = componentInfo.memberInfos;
            var a5_memberNamesWithType = memberNamesWithType(memberInfos);
            var a6_memberAssigns = memberAssignments(memberInfos);
            var a7_memberNames = memberNames(memberInfos);
            var prefix = componentInfo.singleComponentPrefix;
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
