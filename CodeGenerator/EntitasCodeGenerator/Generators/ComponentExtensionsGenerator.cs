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
                if (attr is DontGenerate) {
                    return false;
                }
            }

            return true;
        }

        static string generateComponentExtension(Type type) {
            var code = addClassHeader(type);
            code += addAddMethods(type);
            code += addReplaceMethods(type);
            code += addHasMethods(type);
            code += addRemoveMethods(type);
            code += addGetMethods(type);
            code += addCloseClass();
            return code;
        }

        static string addClassHeader(Type type) {
            var format = "using Entitas;\n\npublic static class {0} {{\n";
            var className = type + classSuffix;
            return string.Format(format, className);
        }

        static string addAddMethods(Type type) {
            const string noFields = @"
    public static {0} instance = new {0}();

    public static void FlagAs{1}(this Entity entity) {{
        entity.AddComponent({2}.{1}, instance);
    }}
";
            const string singleNoFields = @"
    public static Entity AddSingle{1}(this EntityRepository repo) {{
        if (repo.GetSingleEntity({2}.{1}) != null) {{
            throw new SingleEntityException(EntityMatcher.AllOf(new [] {{ {2}.{1} }}));
        }}

        var entity = repo.CreateEntity();
        entity.AddComponent({2}.{1}, instance);
        return entity;
    }}
";
            const string withFields = @"
    public static void Add{1}(this Entity entity, {3}) {{
        var component = new {0}();
{4}
        entity.AddComponent({2}.{1}, component);
    }}
";
            const string singleWithFields = @"
    public static Entity AddSingle{1}(this EntityRepository repo, {3}) {{
        if (repo.GetSingleEntity({2}.{1}) != null) {{
            throw new SingleEntityException(EntityMatcher.AllOf(new [] {{ {2}.{1} }}));
        }}

        var entity = repo.CreateEntity();
        var component = new {0}();
{4}
        entity.AddComponent({2}.{1}, component);
        return entity;
    }}
";
            string format;
            if (isSingletonComponent(type)) {
                format = isOnSingleEntity(type) ? noFields + singleNoFields : noFields;
            } else {
                format = isOnSingleEntity(type) ? withFields + singleWithFields : withFields;
            }

            return buildString(type, format);
        }

        static string addReplaceMethods(Type type) {
            const string withFields = @"
    public static void Replace{1}(this Entity entity, {0} component) {{
        entity.ReplaceComponent({2}.{1}, component);
    }}

    public static void Replace{1}(this Entity entity, {3}) {{
        const int componentId = {2}.{1};
        {0} component;
        if (entity.HasComponent(componentId)) {{
            component = ({0})entity.GetComponent(componentId);
        }} else {{
            component = new {0}();
        }}
{4}
        entity.ReplaceComponent(componentId, component);
    }}
";
            const string singleWithFields = @"
    public static Entity ReplaceSingle{1}(this EntityRepository repo, {0} component) {{
        const int componentId = {2}.{1};
        Entity entity = repo.GetSingleEntity(componentId);
        if (entity == null) {{
            entity = repo.CreateEntity();
            entity.AddComponent(componentId, component);
        }} else {{
            entity.ReplaceComponent(componentId, component);
        }}
        return entity;
    }}

    public static Entity ReplaceSingle{1}(this EntityRepository repo, {3}) {{
        const int componentId = {2}.{1};
        Entity entity = repo.GetSingleEntity(componentId);
        {0} component;
        if (entity == null) {{
            entity = repo.CreateEntity();
            component = new {0}();
        }} else {{
            component = ({0})entity.GetComponent(componentId);
        }}
{4}
        entity.ReplaceComponent(componentId, component);
        return entity;
    }}
";
            string format = string.Empty;
            if (!isSingletonComponent(type)) {
                format = isOnSingleEntity(type) ? withFields + singleWithFields : withFields;
            }

            return buildString(type, format);
        }

        static string addHasMethods(Type type) {
            const string noFields = @"
    public static bool Is{1}(this Entity entity) {{
        return entity.HasComponent({2}.{1});
    }}
";
            const string withFields = @"
    public static bool Has{1}(this Entity entity) {{
        return entity.HasComponent({2}.{1});
    }}
";
            const string single = @"
    public static bool HasSingle{1}(this EntityRepository repo) {{
        return repo.GetSingleEntity({2}.{1}) != null;
    }}
";
            string format;
            if (isSingletonComponent(type)) {
                format = isOnSingleEntity(type) ? noFields + single : noFields;
            } else {
                format = isOnSingleEntity(type) ? withFields + single : withFields;
            }

            return buildString(type, format);
        }

        static string addRemoveMethods(Type type) {
            const string noFields = @"
    public static void Unflag{1}(this Entity entity) {{
        entity.RemoveComponent({2}.{1});
    }}
";
            const string withFields = @"
    public static void Remove{1}(this Entity entity) {{
        entity.RemoveComponent({2}.{1});
    }}
";
            const string single = @"
    public static void RemoveSingle{1}(this EntityRepository repo) {{
        var entity = repo.GetSingleEntity({2}.{1});
        repo.DestroyEntity(entity);
    }}
";
            string format;
            if (isSingletonComponent(type)) {
                format = isOnSingleEntity(type) ? noFields + single : noFields;
            } else {
                format = isOnSingleEntity(type) ? withFields + single : withFields;
            }

            return buildString(type, format);
        }

        static string addGetMethods(Type type) {
            const string singleNoFields = @"
    public static Entity GetSingle{1}Entity(this EntityRepository repo) {{
        return repo.GetSingleEntity({2}.{1});
    }}
";
            const string withFields = @"
    public static {0} Get{1}(this Entity entity) {{
        return ({0})entity.GetComponent({2}.{1});
    }}
";
            const string singleWithFields = @"
    public static {0} GetSingle{1}(this EntityRepository repo) {{
        const int componentId = {2}.{1};
        var entity = repo.GetSingleEntity(componentId);
        return ({0})entity.GetComponent(componentId);
    }}

    public static Entity GetSingle{1}Entity(this EntityRepository repo) {{
        return repo.GetSingleEntity({2}.{1});
    }}
";
            string format;
            if (isSingletonComponent(type)) {
                format = isOnSingleEntity(type) ? singleNoFields : "";
            } else {
                format = isOnSingleEntity(type) ? withFields + singleWithFields : withFields;
            }

            return buildString(type, format);
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
                var name = fields[i].Name;
                var type = getTypeName(fields[i].FieldType);
                namesWithType += type + " " + name;
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

        static string fieldAssignments(FieldInfo[] fieldInfos) {
            var assignments = string.Empty;
            const string format = "        component.{0} = {0};";
            for (int i = 0; i < fieldInfos.Length; i++) {
                assignments += string.Format(format, fieldInfos[i].Name);
                if (i < fieldInfos.Length - 1) {
                    assignments += "\n";
                }
            }

            return assignments;
        }

        static bool isOnSingleEntity(Type type) {
            Attribute[] attrs = Attribute.GetCustomAttributes(type);
            foreach (Attribute attr in attrs) {
                if (attr is SingleEntity) {
                    return true;
                }
            }

            return false;
        }

        static string addCloseClass() {
            return "\n}";
        }

        static string buildString(Type type, string format) {
            var a0_type = type;
            var a1_name = type.RemoveComponentSuffix();
            var a2_tag = indicesLookupTag(type);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var a3_fieldNames = fieldNamesWithType(fields);
            var a4_fieldAssigns = fieldAssignments(fields);

            return string.Format(format, a0_type, a1_name, a2_tag, a3_fieldNames, a4_fieldAssigns);
        }

        static bool isSingletonComponent(Type type) {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            return fields.Length == 0;
        }
    }
}

