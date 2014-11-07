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
            return @"
    public partial class Entity {";
        }

        static string addGetMethods(Type type) {
            string getMethod;
            if (!isSingletonComponent(type)) {
                getMethod = @"
        public {0} {2} {{ get {{ return ({0})GetComponent({3}.{1}); }} }}
";
            } else {
                getMethod = @"
        static readonly {0} {2}Component = new {0}();
";
            }
            return buildString(type, getMethod);
        }

        static string addHasMethods(Type type) {
            string hasMethod;
            if (!isSingletonComponent(type)) {
                hasMethod = @"
        public bool has{1} {{ get {{ return HasComponent({3}.{1}); }} }}
";
            } else {
                hasMethod = @"
        public bool is{1} {{
            get {{ return HasComponent({3}.{1}); }}
            set {{
                if (value != is{1}) {{
                    if (value) {{
                        AddComponent({3}.{1}, {2}Component);
                    }} else {{
                        RemoveComponent({3}.{1});
                    }}
                }}
            }}
        }}
";
            }
            return buildString(type, hasMethod);
        }

        static string addAddMethods(Type type) {
            if (!isSingletonComponent(type)) {
                const string addMethod = @"
        public void Add{1}({0} component) {{
            AddComponent({3}.{1}, component);
        }}

        public void Add{1}({4}) {{
            var component = new {0}();
{5}
            Add{1}(component);
        }}
";
                return buildString(type, addMethod);
            }

            return string.Empty;
        }

        static string addReplaceMethods(Type type) {
            if (!isSingletonComponent(type)) {
                const string replaceMethod = @"
        public void Replace{1}({0} component) {{
            ReplaceComponent({3}.{1}, component);
        }}

        public void Replace{1}({4}) {{
            {0} component;
            if (has{1}) {{
                WillRemoveComponent({3}.{1});
                component = {2};
            }} else {{
                component = new {0}();
            }}
{5}
            Replace{1}(component);
        }}
";
                return buildString(type, replaceMethod);
            } else {
                return string.Empty;
            }
        }

        static string addRemoveMethods(Type type) {
            if (!isSingletonComponent(type)) {
                const string removeMethod = @"
        public void Remove{1}() {{
            RemoveComponent({3}.{1});
        }}
";
                return buildString(type, removeMethod);
            }

            return string.Empty;
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
            return @"
    public partial class EntityRepository {";
        }

        static string addRepoGetMethods(Type type) {
            string getMehod;
            if (!isSingletonComponent(type)) {
                getMehod = @"
        public Entity {2}Entity {{ get {{ return GetCollection(Matcher.{1}).GetSingleEntity(); }} }}

        public {0} {2} {{ get {{ return {2}Entity.{2}; }} }}
";
            } else {
                getMehod = @"
        public Entity {2}Entity {{ get {{ return GetCollection(Matcher.{1}).GetSingleEntity(); }} }}
";
            }
            return buildString(type, getMehod);
        }

        static string addRepoHasMethods(Type type) {
            string hasMethod;
            if (!isSingletonComponent(type)) {
                hasMethod = @"
        public bool has{1} {{ get {{ return {2}Entity != null; }} }}
";
            } else {
                hasMethod = @"
        public bool is{1} {{
            get {{ return {2}Entity != null; }}
            set {{
                var entity = {2}Entity;
                if (value != (entity != null)) {{
                    if (value) {{
                        CreateEntity().is{1} = true;
                    }} else {{
                        DestroyEntity(entity);
                    }}
                }}
            }}
        }}
";
            }
            return buildString(type, hasMethod);
        }

        static object addRepoAddMethods(Type type) {
            if (!isSingletonComponent(type)) {
                const string addMethod = @"
        public Entity Set{1}({0} component) {{
            if (has{1}) {{
                throw new SingleEntityException(Matcher.{1});
            }}
            var entity = CreateEntity();
            entity.Add{1}(component);
            return entity;
        }}

        public Entity Set{1}({4}) {{
            if (has{1}) {{
                throw new SingleEntityException(Matcher.{1});
            }}
            var entity = CreateEntity();
            entity.Add{1}({6});
            return entity;
        }}
";
                return buildString(type, addMethod);
            }

            return string.Empty;
        }

        static string addRepoReplaceMethods(Type type) {
            if (!isSingletonComponent(type)) {
                const string replaceMethod = @"
        public Entity Replace{1}({0} component) {{
            var entity = {2}Entity;
            if (entity == null) {{
                entity = Set{1}(component);
            }} else {{
                entity.Replace{1}(component);
            }}

            return entity;
        }}

        public Entity Replace{1}({4}) {{
            var entity = {2}Entity;
            if (entity == null) {{
                entity = Set{1}({6});
            }} else {{
                entity.Replace{1}({6});
            }}

            return entity;
        }}
";
                return buildString(type, replaceMethod);
            } else {
                return string.Empty;
            }
        }

        static string addRepoRemoveMethods(Type type) {
            if (!isSingletonComponent(type)) {
                const string removeMethod = @"
        public void Remove{1}() {{
            DestroyEntity({2}Entity);
        }}
";
                return buildString(type, removeMethod);
            }

            return string.Empty;
        }

        /*
        *
        * MATCHER
        *
        */

        static string addMatcher(Type type) {
            const string matcher = @"
    public static partial class Matcher {{
        static AllOfEntityMatcher _matcher{1};

        public static AllOfEntityMatcher {1} {{
            get {{
                if (_matcher{1} == null) {{
                    _matcher{1} = EntityMatcher.AllOf(new [] {{ {3}.{1} }});
                }}

                return _matcher{1};
            }}
        }}
    }}
";
            return buildString(type, matcher);
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
            if (typeShortcuts.ContainsKey(type.Name)) {
                return typeShortcuts[type.Name];
            }

            return simpleTypeString(type);
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
            return @"    }
";
        }
    }
}

