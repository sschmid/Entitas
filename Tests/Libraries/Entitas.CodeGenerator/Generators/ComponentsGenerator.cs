using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Entitas.CodeGenerator {
    public class ComponentsGenerator : IComponentCodeGenerator {

        const string CLASS_SUFFIX = "GeneratedExtension";

        public CodeGenFile[] Generate(ComponentInfo[] componentInfos) {
            return componentInfos
                .Where(info => info.generateMethods)
                .Aggregate(new List<CodeGenFile>(), (files, info) => {
                    files.Add(new CodeGenFile {
                        fileName = info.type + CLASS_SUFFIX,
                        fileContent = generateComponentExtension(info).ToUnixLineEndings()
                    });
                    return files;
                }).ToArray();
        }

        static string generateComponentExtension(ComponentInfo componentInfo) {
            return componentInfo.pools.Length == 0
                        ? addDefaultPoolCode(componentInfo)
                        : addCustomPoolCode(componentInfo);
        }

        static string addDefaultPoolCode(ComponentInfo componentInfo) {
            var code = addNamespace();
            code += addEntityMethods(componentInfo);
            if (componentInfo.isSingleEntity) {
                code += addPoolMethods(componentInfo);
            }
            code += addMatcher(componentInfo);
            code += closeNamespace();
            return code;
        }

        static string addCustomPoolCode(ComponentInfo componentInfo) {
            var code = addUsings();
            code += addNamespace();
            code += addEntityMethods(componentInfo);
            if (componentInfo.isSingleEntity) {
                code += addPoolMethods(componentInfo);
            }
            code += closeNamespace();
            code += addMatcher(componentInfo);
            return code;
        }

        static string addUsings() {
            return "using Entitas;\n\n";
        }

        static string addNamespace() {
            return @"namespace Entitas {";
        }

        static string closeNamespace() {
            return "}\n";
        }

        /*
         *
         * ENTITY METHODS
         *
         */

        static string addEntityMethods(ComponentInfo componentInfo) {
            return addEntityClassHeader()
                    + addGetMethods(componentInfo)
                    + addHasMethods(componentInfo)
                    + addAddMethods(componentInfo)
                    + addReplaceMethods(componentInfo)
                    + addRemoveMethods(componentInfo)
                    + addCloseClass();
        }

        static string addEntityClassHeader() {
            return "\n    public partial class Entity {";
        }

        static string addGetMethods(ComponentInfo componentInfo) {
            var getMethod = componentInfo.isSingletonComponent
                    ? "\n        static readonly $Type $nameComponent = new $Type();\n"
                    : "\n        public $Type $name { get { return ($Type)GetComponent($Ids.$Name); } }\n";

            return buildString(componentInfo, getMethod);
        }

        static string addHasMethods(ComponentInfo componentInfo) {
            var hasMethod = componentInfo.isSingletonComponent ? @"
        public bool $prefix$Name {
            get { return HasComponent($Ids.$Name); }
            set {
                if (value != $prefix$Name) {
                    if (value) {
                        AddComponent($Ids.$Name, $nameComponent);
                    } else {
                        RemoveComponent($Ids.$Name);
                    }
                }
            }
        }

        public Entity $Prefix$Name(bool value) {
            $prefix$Name = value;
            return this;
        }
" : @"
        public bool has$Name { get { return HasComponent($Ids.$Name); } }
";
            return buildString(componentInfo, hasMethod);
        }

        static string addAddMethods(ComponentInfo componentInfo) {
            return componentInfo.isSingletonComponent ? string.Empty : buildString(componentInfo, @"
        public Entity Add$Name($typedArgs) {
            var componentPool = GetComponentPool($Ids.$Name);
            var component = ($Type)(componentPool.Count > 0 ? componentPool.Pop() : new $Type());
$assign
            return AddComponent($Ids.$Name, component);
        }
");
        }

        static string addReplaceMethods(ComponentInfo componentInfo) {
            return componentInfo.isSingletonComponent ? string.Empty : buildString(componentInfo, @"
        public Entity Replace$Name($typedArgs) {
            var componentPool = GetComponentPool($Ids.$Name);
            var component = ($Type)(componentPool.Count > 0 ? componentPool.Pop() : new $Type());
$assign
            ReplaceComponent($Ids.$Name, component);
            return this;
        }
");
        }

        static string addRemoveMethods(ComponentInfo componentInfo) {
            return componentInfo.isSingletonComponent ? string.Empty : buildString(componentInfo, @"
        public Entity Remove$Name() {
            return RemoveComponent($Ids.$Name);;
        }
");
        }

        /*
         *
         * POOL METHODS
         *
         */

        static string addPoolMethods(ComponentInfo componentInfo) {
            return addPoolClassHeader()
                    + addPoolGetMethods(componentInfo)
                    + addPoolHasMethods(componentInfo)
                    + addPoolAddMethods(componentInfo)
                    + addPoolReplaceMethods(componentInfo)
                    + addPoolRemoveMethods(componentInfo)
                    + addCloseClass();
        }

        static string addPoolClassHeader() {
            return "\n    public partial class Pool {";
        }

        static string addPoolGetMethods(ComponentInfo componentInfo) {
            var getMehod = componentInfo.isSingletonComponent ? @"
        public Entity $nameEntity { get { return GetGroup($TagMatcher.$Name).GetSingleEntity(); } }
" : @"
        public Entity $nameEntity { get { return GetGroup($TagMatcher.$Name).GetSingleEntity(); } }

        public $Type $name { get { return $nameEntity.$name; } }
";
            return buildString(componentInfo, getMehod);
        }

        static string addPoolHasMethods(ComponentInfo componentInfo) {
            var hasMethod = componentInfo.isSingletonComponent ? @"
        public bool $prefix$Name {
            get { return $nameEntity != null; }
            set {
                var entity = $nameEntity;
                if (value != (entity != null)) {
                    if (value) {
                        CreateEntity().$prefix$Name = true;
                    } else {
                        DestroyEntity(entity);
                    }
                }
            }
        }
" : @"
        public bool has$Name { get { return $nameEntity != null; } }
";
            return buildString(componentInfo, hasMethod);
        }

        static object addPoolAddMethods(ComponentInfo componentInfo) {
            return componentInfo.isSingletonComponent ? string.Empty : buildString(componentInfo, @"
        public Entity Set$Name($typedArgs) {
            if (has$Name) {
                throw new EntitasException(""Could not set $name!\n"" + this + "" already has an entity with $Type!"",
                    ""You should check if the pool already has a $nameEntity before setting it or use pool.Replace$Name()."");
            }
            var entity = CreateEntity();
            entity.Add$Name($args);
            return entity;
        }
");
        }

        static string addPoolReplaceMethods(ComponentInfo componentInfo) {
            return componentInfo.isSingletonComponent ? string.Empty : buildString(componentInfo, @"
        public Entity Replace$Name($typedArgs) {
            var entity = $nameEntity;
            if (entity == null) {
                entity = Set$Name($args);
            } else {
                entity.Replace$Name($args);
            }

            return entity;
        }
");
        }

        static string addPoolRemoveMethods(ComponentInfo componentInfo) {
            return componentInfo.isSingletonComponent ? string.Empty : buildString(componentInfo, @"
        public void Remove$Name() {
            DestroyEntity($nameEntity);
        }
");
        }

        /*
        *
        * MATCHER
        *
        */

        static string addMatcher(ComponentInfo componentInfo) {
            const string matcherFormat = @"
    public partial class $TagMatcher {
        static IMatcher _matcher$Name;

        public static IMatcher $Name {
            get {
                if (_matcher$Name == null) {
                    var matcher = (Matcher)Matcher.AllOf($Ids.$Name);
                    matcher.componentNames = $Ids.componentNames;
                    _matcher$Name = matcher;
                }

                return _matcher$Name;
            }
        }
    }
";
            if (componentInfo.pools.Length == 0) {
                return buildString(componentInfo, matcherFormat);
            }

            var matchers = componentInfo.pools.Aggregate(string.Empty, (acc, poolName) => {
                return acc + buildString(componentInfo, matcherFormat.Replace("$Tag", poolName));
            });

            return buildString(componentInfo, matchers);
        }

        /*
         *
         * HELPERS
         *
         */

        static string buildString(ComponentInfo componentInfo, string format) {
            format = createFormatString(format);
            var a0_type = componentInfo.type;
            var a1_name = componentInfo.type.RemoveComponentSuffix();
            var a2_lowercaseName = a1_name.LowercaseFirst();
            var poolNames = componentInfo.pools;
            var a3_tag = poolNames.Length == 0 ? string.Empty : poolNames[0];
            var lookupTags = componentInfo.ComponentLookupTags();
            var a4_ids = lookupTags.Length == 0 ? string.Empty : lookupTags[0];
            var fieldInfos = componentInfo.fieldInfos;
            var a5_fieldNamesWithType = fieldNamesWithType(fieldInfos);
            var a6_fieldAssigns = fieldAssignments(fieldInfos);
            var a7_fieldNames = fieldNames(fieldInfos);
            var prefix = componentInfo.singleComponentPrefix;
            var a8_prefix = prefix.UppercaseFirst();
            var a9_lowercasePrefix = prefix.LowercaseFirst();

            return string.Format(format, a0_type, a1_name, a2_lowercaseName,
                a3_tag, a4_ids, a5_fieldNamesWithType, a6_fieldAssigns, a7_fieldNames,
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

        static string fieldNamesWithType(ComponentFieldInfo[] fieldInfos) {
            var typedArgs = fieldInfos
                .Select(info => info.type + " new" + info.name.UppercaseFirst())
                .ToArray();

            return string.Join(", ", typedArgs);
        }

        static string fieldAssignments(ComponentFieldInfo[] fieldInfos) {
            const string format = "            component.{0} = {1};";
            var assignments = fieldInfos.Select(info => {
                var newArg = "new" + info.name.UppercaseFirst();
                return string.Format(format, info.name, newArg);
            }).ToArray();

            return string.Join("\n", assignments);
        }

        static string fieldNames(ComponentFieldInfo[] fieldInfos) {
            var args = fieldInfos.Select(info => "new" + info.name.UppercaseFirst()).ToArray();
            return string.Join(", ", args);
        }

        static string addCloseClass() {
            return "    }\n";
        }
    }
}

