using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Entitas.CodeGenerator {
    public class ComponentExtensionsGenerator {
        public const string classSuffix = "GeneratedExtension";

        public Dictionary<string, string> GenerateComponentExtensions(Type[] components) {
            return components
                .Where(shouldGenerate)
                .ToDictionary(
                type => type + classSuffix,
                type => generateComponentExtension(type)
            );
        }

        static bool shouldGenerate(Type type) {
            Attribute[] attrs = Attribute.GetCustomAttributes(type);
            foreach (Attribute attr in attrs) {
                if (attr is DontGenerateAttribute) {
                    return false;
                }
            }

            return true;
        }

        static string generateComponentExtension(Type type) {
            var code = addNamespace();
            code += addEntityMethods(type);
            if (isSingleEntity(type)) {
                code += addPoolMethods(type);
            }
            code += addMatcher(type);
            code += closeNamespace();
            return code;
        }

        static string addNamespace() {
            return @"namespace Entitas {";
        }

        static string closeNamespace() {
            return "}";
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
" : @"
        public bool has$Name { get { return HasComponent($Ids.$Name); } }
";
            return buildString(type, hasMethod);
        }

        static string addAddMethods(Type type) {
            return isSingletonComponent(type) ? string.Empty : buildString(type, @"
        public void Add$Name($Type component) {
            AddComponent($Ids.$Name, component);
        }

        public void Add$Name($typedArgs) {
            var component = new $Type();
$assign
            Add$Name(component);
        }
");
        }

        static string addReplaceMethods(Type type) {
            return isSingletonComponent(type) ? string.Empty : buildString(type, @"
        public void Replace$Name($typedArgs) {
            $Type component;
            if (has$Name) {
                WillRemoveComponent($Ids.$Name);
                component = $name;
            } else {
                component = new $Type();
            }
$assign
            ReplaceComponent($Ids.$Name, component);
        }
");
        }

        static string addRemoveMethods(Type type) {
            return isSingletonComponent(type) ? string.Empty : buildString(type, @"
        public void Remove$Name() {
            RemoveComponent($Ids.$Name);
        }
");
        }

        /*
         *
         * POOL METHODS
         *
         */

        static string addPoolMethods(Type type) {
            return addPoolClassHeader()
            + addPoolGetMethods(type)
            + addPoolHasMethods(type)
            + addPoolAddMethods(type)
            + addPoolReplaceMethods(type)
            + addPoolRemoveMethods(type)
            + addCloseClass();
        }

        static string addPoolClassHeader() {
            return "\n    public partial class Pool {";
        }

        static string addPoolGetMethods(Type type) {
            string getMehod = isSingletonComponent(type) ? @"
        public Entity $nameEntity { get { return GetGroup(Matcher.$Name).GetSingleEntity(); } }
" : @"
        public Entity $nameEntity { get { return GetGroup(Matcher.$Name).GetSingleEntity(); } }

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
                throw new SingleEntityException(Matcher.$Name);
            }
            var entity = CreateEntity();
            entity.Add$Name(component);
            return entity;
        }

        public Entity Set$Name($typedArgs) {
            if (has$Name) {
                throw new SingleEntityException(Matcher.$Name);
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
    public static partial class Matcher {
        static AllOfMatcher _matcher$Name;

        public static AllOfMatcher $Name {
            get {
                if (_matcher$Name == null) {
                    _matcher$Name = Matcher.AllOf(new [] { $Ids.$Name });
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
            Attribute[] attrs = Attribute.GetCustomAttributes(type);
            foreach (Attribute attr in attrs) {
                if (attr is SingleEntityAttribute) {
                    return true;
                }
            }

            return false;
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
            var a3_tag = indicesLookupTag(type);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var a4_fieldNamesWithType = fieldNamesWithType(fields);
            var a5_fieldAssigns = fieldAssignments(fields);
            var a6_fieldNames = fieldNames(fields);

            return string.Format(format, a0_type, a1_name, a2_lowercaseName,
                a3_tag, a4_fieldNamesWithType, a5_fieldAssigns, a6_fieldNames);
        }

        static string createFormatString(string format) {
            return format.Replace("{", "{{")
                .Replace("}", "}}")
                .Replace("$Type", "{0}")
                .Replace("$Name", "{1}")
                .Replace("$name", "{2}")
                .Replace("$Ids", "{3}")
                .Replace("$typedArgs", "{4}")
                .Replace("$assign", "{5}")
                .Replace("$args", "{6}");
        }

        static string indicesLookupTag(Type type) {
            Attribute[] attrs = Attribute.GetCustomAttributes(type);
            foreach (Attribute attr in attrs) {
                var lookup = attr as PoolAttribute;
                if (lookup != null) {
                    return lookup.tag;
                }
            }

            return "ComponentIds";
        }

        static string fieldNamesWithType(FieldInfo[] fields) {
            var typedArgs = fields.Select(arg => {
                var newArg = "new" + arg.Name.UppercaseFirst();
                var type = getTypeName(arg.FieldType);
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

        static Dictionary<string, string> typeShortcuts = new Dictionary<string, string>() {
            { "System.Byte", "byte" },
            { "System.SByte", "sbyte" },
            { "System.Int32", "int" },
            { "System.UInt32", "uint" },
            { "System.Int16", "short" },
            { "System.UInt16", "ushort" },
            { "System.Int64", "long" },
            { "System.UInt64", "ulong" },
            { "System.Single", "float" },
            { "System.Double", "double" },
            { "System.Char", "char" },
            { "System.Boolean", "bool" },
            { "System.Object", "object" },
            { "System.String", "string" },
            { "System.Decimal", "decimal" }
        };

        static string getTypeName(Type type) {
            string typeStr;
            var typeName = typeShortcuts.TryGetValue(type.FullName, out typeStr) ? typeStr : simpleTypeString(type);
            if (type.IsEnum) {
                typeName = typeName.Replace("+", ".");
            }
            return typeName;
        }

        static string simpleTypeString(Type type) {
            var typeString = type.FullName.Split('`')[0];
            var genericTypes = type.GetGenericArguments().Select<Type, string>(getTypeName).ToArray();
            return type.GetGenericArguments().Length == 0 ? typeString :
                typeString + "<" + string.Join(", ", genericTypes) + ">";
        }

        static string addCloseClass() {
            return "    }\n";
        }
    }
}

