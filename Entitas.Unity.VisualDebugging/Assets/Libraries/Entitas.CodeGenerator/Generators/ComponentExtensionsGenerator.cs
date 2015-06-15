using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Entitas.CodeGenerator {
    public class ComponentExtensionsGenerator : IComponentCodeGenerator {

        const string classSuffix = "GeneratedExtension";

        public CodeGenFile[] Generate(Type[] components) {
            return components
                    .Where(shouldGenerate)
                    .Aggregate(new List<CodeGenFile>(), (files, type) => {
                        files.Add(new CodeGenFile {
                            fileName = type + classSuffix,
                            fileContent = generateComponentExtension(type)
                        });
                        return files;
                    }).ToArray();
        }

        static bool shouldGenerate(Type type) {
            return !Attribute.GetCustomAttributes(type)
                .Any(attr => attr is DontGenerateAttribute);
        }

        static string generateComponentExtension(Type type) {
            return type.PoolName() == string.Empty
                        ? addDefaultPoolCode(type)
                        : addCustomPoolCode(type);
        }

        static string addDefaultPoolCode(Type type) {
            var code = addNamespace();
            code += addEntityMethods(type);
            if (isSingleEntity(type)) {
                code += addPoolMethods(type);
            }
            code += addMatcher(type);
            code += closeNamespace();
            return code;
        }

        static string addCustomPoolCode(Type type) {
            var code = addUsing();
            code += addNamespace();
            code += addEntityMethods(type);
            if (isSingleEntity(type)) {
                code += addPoolMethods(type);
            }
            code += closeNamespace();
            code += addMatcher(type);
            return code;
        }

        static string addUsing() {
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

        static string addEntityMethods(Type type) {
            return addEntityClassHeader()
                    + addGetMethods(type)
                    + addHasMethods(type)
                    + addAddMethods(type)
                    + addReplaceMethods(type)
                    + addRemoveMethods(type)
                    + addCloseClass();
        }

        static string addEntityClassHeader() {
            return "\n    public partial class Entity {";
        }

        static string addGetMethods(Type type) {
            string getMethod = isSingletonComponent(type) ?
                "\n        static readonly $Type $nameComponent = new $Type();\n" :
                "\n        public $Type $name { get { return ($Type)GetComponent($Ids.$Name); } }\n";
            return buildString(type, getMethod);
        }

        static string addHasMethods(Type type) {
            string hasMethod = isSingletonComponent(type) ? @"
        public bool is$Name {
            get { return HasComponent($Ids.$Name); }
            set {
                if (value != is$Name) {
                    if (value) {
                        AddComponent($Ids.$Name, $nameComponent);
                    } else {
                        RemoveComponent($Ids.$Name);
                    }
                }
            }
        }

        public Entity Is$Name(bool value) {
            is$Name = value;
            return this;
        }
" : @"
        public bool has$Name { get { return HasComponent($Ids.$Name); } }
";
            return buildString(type, hasMethod);
        }

        static string addAddMethods(Type type) {
            return isSingletonComponent(type) ? string.Empty : buildString(type, @"
        public Entity Add$Name($Type component) {
            return AddComponent($Ids.$Name, component);
        }

        public Entity Add$Name($typedArgs) {
            var component = new $Type();
$assign
            return Add$Name(component);
        }
");
        }

        static string addReplaceMethods(Type type) {
            return isSingletonComponent(type) ? string.Empty : buildString(type, @"
        public Entity Replace$Name($typedArgs) {
            $Type component;
            if (has$Name) {
                WillRemoveComponent($Ids.$Name);
                component = $name;
            } else {
                component = new $Type();
            }
$assign
            return ReplaceComponent($Ids.$Name, component);
        }
");
        }

        static string addRemoveMethods(Type type) {
            return isSingletonComponent(type) ? string.Empty : buildString(type, @"
        public Entity Remove$Name() {
            return RemoveComponent($Ids.$Name);
        }
");
        }

        /*
         *
         * POOL METHODS
         *
         */

        static string addPoolMethods(Type type) {
            return addPoolClassHeader(type)
                    + addPoolGetMethods(type)
                    + addPoolHasMethods(type)
                    + addPoolAddMethods(type)
                    + addPoolReplaceMethods(type)
                    + addPoolRemoveMethods(type)
                    + addCloseClass();
        }

        static string addPoolClassHeader(Type type) {
            return buildString(type, "\n    public partial class Pool {");
        }

        static string addPoolGetMethods(Type type) {
            string getMehod = isSingletonComponent(type) ? @"
        public Entity $nameEntity { get { return GetGroup($TagMatcher.$Name).GetSingleEntity(); } }
" : @"
        public Entity $nameEntity { get { return GetGroup($TagMatcher.$Name).GetSingleEntity(); } }

        public $Type $name { get { return $nameEntity.$name; } }
";
            return buildString(type, getMehod);
        }

        static string addPoolHasMethods(Type type) {
            string hasMethod = isSingletonComponent(type) ? @"
        public bool is$Name {
            get { return $nameEntity != null; }
            set {
                var entity = $nameEntity;
                if (value != (entity != null)) {
                    if (value) {
                        CreateEntity().is$Name = true;
                    } else {
                        DestroyEntity(entity);
                    }
                }
            }
        }
" : @"
        public bool has$Name { get { return $nameEntity != null; } }
";
            return buildString(type, hasMethod);
        }

        static object addPoolAddMethods(Type type) {
            return isSingletonComponent(type) ? string.Empty : buildString(type, @"
        public Entity Set$Name($Type component) {
            if (has$Name) {
                throw new SingleEntityException($TagMatcher.$Name);
            }
            var entity = CreateEntity();
            entity.Add$Name(component);
            return entity;
        }

        public Entity Set$Name($typedArgs) {
            if (has$Name) {
                throw new SingleEntityException($TagMatcher.$Name);
            }
            var entity = CreateEntity();
            entity.Add$Name($args);
            return entity;
        }
");
        }

        static string addPoolReplaceMethods(Type type) {
            return isSingletonComponent(type) ? string.Empty : buildString(type, @"
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

        static string addPoolRemoveMethods(Type type) {
            return isSingletonComponent(type) ? string.Empty : buildString(type, @"
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

        static string addMatcher(Type type) {
            return buildString(type, @"
    public partial class $TagMatcher {
        static AllOfMatcher _matcher$Name;

        public static AllOfMatcher $Name {
            get {
                if (_matcher$Name == null) {
                    _matcher$Name = new $TagMatcher($Ids.$Name);
                }

                return _matcher$Name;
            }
        }
    }
");
        }

        /*
         *
         * HELPERS
         *
         */

        static bool isSingleEntity(Type type) {
            return Attribute.GetCustomAttributes(type)
                .Any(attr => attr is SingleEntityAttribute);
        }

        static bool isSingletonComponent(Type type) {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            return fields.Length == 0;
        }

        static string buildString(Type type, string format) {
            format = createFormatString(format);
            var a0_type = type;
            var a1_name = type.RemoveComponentSuffix();
            var a2_lowercaseName = a1_name.LowercaseFirst();
            var a3_tag = type.PoolName();
            var a4_ids = type.IndicesLookupTag();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var a5_fieldNamesWithType = fieldNamesWithType(fields);
            var a6_fieldAssigns = fieldAssignments(fields);
            var a7_fieldNames = fieldNames(fields);

            return string.Format(format, a0_type, a1_name, a2_lowercaseName,
                a3_tag, a4_ids, a5_fieldNamesWithType, a6_fieldAssigns, a7_fieldNames);
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
                        .Replace("$args", "{7}");
        }

        static string fieldNamesWithType(FieldInfo[] fields) {
            var typedArgs = fields.Select(arg => {
                var newArg = "new" + arg.Name.UppercaseFirst();
                var type = TypeGenerator.Generate(arg.FieldType);
                return type + " " + newArg;
            }).ToArray();

            return string.Join(", ", typedArgs);
        }

        static string fieldAssignments(FieldInfo[] fields) {
            const string format = "            component.{0} = {1};";
            var assignments = fields.Select(arg => {
                var newArg = "new" + arg.Name.UppercaseFirst();
                return string.Format(format, arg.Name, newArg);
            }).ToArray();

            return string.Join("\n", assignments);
        }

        static string fieldNames(FieldInfo[] fields) {
            var args = fields.Select(arg => "new" + arg.Name.UppercaseFirst()).ToArray();
            return string.Join(", ", args);
        }

        static string addCloseClass() {
            return "    }\n";
        }
    }
}

