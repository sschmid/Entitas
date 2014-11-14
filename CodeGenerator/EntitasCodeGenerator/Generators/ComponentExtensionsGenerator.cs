using System;
using System.Reflection;
using System.Collections.Generic;

namespace Entitas.CodeGenerator {
    public class ComponentExtensionsGenerator {
        public const string classSuffix = "GeneratedExtension";

        public Dictionary<string, string> GenerateComponentExtensions(Type[] components) {
            var extensions = new Dictionary<string, string>();
            foreach (var type in components) {
                if (shouldGenerate(type)) {
                    var code = generateComponentExtension(type);
                    extensions.Add(type + classSuffix, code);
                }
            }

            return extensions;
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
                code += addEntityRepositoryMethods(type);
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
            var code = addEntityClassHeader();
            code += addGetMethods(type);
            code += addHasMethods(type);
            code += addAddMethods(type);
            code += addReplaceMethods(type);
            code += addRemoveMethods(type);
            code += addCloseClass();
            return code;
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
         * ENTITY REPOSITORY METHODS
         *
         */

        static string addEntityRepositoryMethods(Type type) {
            var code = addEntityRepositoryClassHeader();
            code += addRepoGetMethods(type);
            code += addRepoHasMethods(type);
            code += addRepoAddMethods(type);
            code += addRepoReplaceMethods(type);
            code += addRepoRemoveMethods(type);
            code += addCloseClass();
            return code;
        }

        static string addEntityRepositoryClassHeader() {
            return "\n    public partial class EntityRepository {";
        }

        static string addRepoGetMethods(Type type) {
            string getMehod = isSingletonComponent(type) ? @"
        public Entity $nameEntity { get { return GetCollection(Matcher.$Name).GetSingleEntity(); } }
" : @"
        public Entity $nameEntity { get { return GetCollection(Matcher.$Name).GetSingleEntity(); } }

        public $Type $name { get { return $nameEntity.$name; } }
";
            return buildString(type, getMehod);
        }

        static string addRepoHasMethods(Type type) {
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

        static object addRepoAddMethods(Type type) {
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

        static string addRepoReplaceMethods(Type type) {
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

        static string addRepoRemoveMethods(Type type) {
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
        static AllOfEntityMatcher _matcher$Name;

        public static AllOfEntityMatcher $Name {
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
            var a2_lowercaseName = char.ToLower(a1_name[0]) + a1_name.Substring(1);
            var a3_tag = indicesLookupTag(type);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var a4_fieldNamesWithType = fieldNamesWithType(fields);
            var a5_fieldAssigns = fieldAssignments(fields);
            var a6_fieldNames = fieldNames(fields);

            return string.Format(format, a0_type, a1_name, a2_lowercaseName, a3_tag, a4_fieldNamesWithType, a5_fieldAssigns, a6_fieldNames);
        }

        static string createFormatString(string format) {
            format = format.Replace("{", "{{");
            format = format.Replace("}", "}}");
            format = format.Replace("$Type", "{0}");
            format = format.Replace("$Name", "{1}");
            format = format.Replace("$name", "{2}");
            format = format.Replace("$Ids", "{3}");
            format = format.Replace("$typedArgs", "{4}");
            format = format.Replace("$assign", "{5}");
            format = format.Replace("$args", "{6}");
            return format;
        }

        static string indicesLookupTag(Type type) {
            Attribute[] attrs = Attribute.GetCustomAttributes(type);
            foreach (Attribute attr in attrs) {
                var lookup = attr as EntityRepositoryAttribute;
                if (lookup != null) {
                    return lookup.tag;
                }
            }

            return "ComponentIds";
        }

        static string fieldNamesWithType(FieldInfo[] fields) {
            var namesWithType = string.Empty;
            for (int i = 0; i < fields.Length; i++) {
                var arg = fields[i].Name;
                var newArg = "new" + char.ToUpper(arg[0]) + arg.Substring(1);
                var type = getTypeName(fields[i].FieldType);
                namesWithType += type + " " + newArg;
                if (i < fields.Length - 1) {
                    namesWithType += ", ";
                }
            }
            return namesWithType;
        }

        static string fieldAssignments(FieldInfo[] fieldInfos) {
            var assignments = string.Empty;
            const string format = "            component.{0} = {1};";
            for (int i = 0; i < fieldInfos.Length; i++) {
                var arg = fieldInfos[i].Name;
                var newArg = "new" + char.ToUpper(arg[0]) + arg.Substring(1);
                assignments += string.Format(format, arg, newArg);
                if (i < fieldInfos.Length - 1) {
                    assignments += "\n";
                }
            }

            return assignments;
        }

        static string fieldNames(FieldInfo[] fields) {
            var namesWithType = string.Empty;
            for (int i = 0; i < fields.Length; i++) {
                var arg = fields[i].Name;
                var newArg = "new" + char.ToUpper(arg[0]) + arg.Substring(1);
                namesWithType += newArg;
                if (i < fields.Length - 1) {
                    namesWithType += ", ";
                }
            }
            return namesWithType;
        }

        static Dictionary<string, string> typeShortcuts = new Dictionary<string, string>() {
            { "Byte", "byte" },
            { "SByte", "sbyte" },
            { "Int32", "int" },
            { "UInt32", "uint" },
            { "Int16", "short" },
            { "UInt16", "ushort" },
            { "Int64", "long" },
            { "UInt64", "ulong" },
            { "Single", "float" },
            { "Double", "double" },
            { "Char", "char" },
            { "Boolean", "bool" },
            { "Object", "object" },
            { "String", "string" },
            { "Decimal", "decimal" }
        };

        static string getTypeName(Type type) {
            return typeShortcuts.ContainsKey(type.Name) ?
                typeShortcuts[type.Name] : simpleTypeString(type);
        }

        static string simpleTypeString(Type type) {
            Type[] types = type.GetGenericArguments();
            var simplified = type.FullName.Split('`')[0];
            if (types.Length > 0) {
                simplified += "<";
                for (int i = 0; i < types.Length; i++) {
                    simplified += getTypeName(types[i]);
                    if (i < types.Length - 1) {
                        simplified += ", ";
                    }
                }
                simplified += ">";
            }

            return simplified;
        }

        static string addCloseClass() {
            return "    }\n";
        }
    }
}

