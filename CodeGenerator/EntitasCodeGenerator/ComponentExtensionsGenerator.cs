using System;
using System.Reflection;
using System.Collections.Generic;

namespace Entitas.CodeGenerator {
    public class ComponentExtensionsGenerator : CodeGenerator {
        const string classSuffix = "GeneratedExtension";

        public void GenerateComponentExtensions(Type[] components, string generatedFolder) {
            foreach (var type in components)
                if (shouldGenerate(type))
                    generateComponentExtension(type, generatedFolder);
        }

        static bool shouldGenerate(Type type) {
            Attribute[] attrs = Attribute.GetCustomAttributes(type);
            foreach (Attribute attr in attrs)
                if (attr is DontGenerate)
                    return false;

            return true;
        }

        static void generateComponentExtension(Type type, string generatedFolder) {
            var code = string.Empty;
            code = addClassHeader(code, type);
            code = addAddMethods(code, type);
            code = addReplaceMethods(code, type);
            code = addHasMethods(code, type);
            code = addRemoveMethods(code, type);
            code = addFieldGetterMethods(code, type);
            code = addCloseClass(code);
            var filePath = generatedFolder + type + classSuffix + ".cs";
            write(filePath, code);
        }

        static string addClassHeader(string str, Type type) {
            str += string.Format(
                @"using Entitas;

public static class {0} {{
", type + classSuffix);
            return str;
        }

        static string addAddMethods(string str, Type type) {
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var a0_type = type;
            var a1_tag = indicesLookupTag(type);
            var a2_name = type.RemoveComponentSuffix();
            var a3_fieldNames = fieldNamesWithType(fieldInfos);
            var a4_fieldAssigns = fieldAssignments(fieldInfos);

            const string defaultFormat = @"
    public static void AddComponent(this Entity entity, {0} component) {{
        entity.AddComponent({1}.{2}, component);
    }}

    public static void Add{2}(this Entity entity, {0} component) {{
        entity.AddComponent({1}.{2}, component);
    }}
";
            const string defaultWithFieldsFormat = @"
    public static void Add{2}(this Entity entity, {3}) {{
        var component = new {0}();
{4}
        entity.AddComponent({1}.{2}, component);
    }}
";
            const string singleEntityFormat = @"
    public static Entity AddSingle{2}(this EntityRepository repo) {{
        var entity = repo.GetEntityFromPool();
        entity.AddComponent({1}.{2}, new {0}());
        return entity;
    }}
";
            const string singleEntityWithFieldsFormat = @"
    public static Entity AddSingle{2}(this EntityRepository repo, {3}) {{
        var entity = repo.GetEntityFromPool();
        var component = new {0}();
{4}
        entity.AddComponent({1}.{2}, component);
        return entity;
    }}
";
            const string singletonFormat = @"
    public static void FlagAs{2}(this Entity entity) {{
        entity.AddComponent({1}.{2}, {0}.singleton);
    }}
";
            const string singleEntitySingletonFormat = @"
    public static Entity AddSingle{2}(this EntityRepository repo) {{
        var entity = repo.GetEntityFromPool();
        entity.AddComponent({1}.{2}, {0}.singleton);
        return entity;
    }}
";

            string format;
            if (isSingletonComponent(type)) {
                format = isOnSingleEntity(type) ? singletonFormat + singleEntitySingletonFormat : singletonFormat;
            } else {
                if (a3_fieldNames.Length == 0)
                    format = isOnSingleEntity(type) ? singleEntityFormat : defaultFormat;
                else
                    format = isOnSingleEntity(type) ? singleEntityWithFieldsFormat : defaultFormat + defaultWithFieldsFormat;
            }

            return str + string.Format(format, a0_type, a1_tag, a2_name, a3_fieldNames, a4_fieldAssigns);
        }

        static string addReplaceMethods(string str, Type type) {
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var a0_type = type;
            var a1_tag = indicesLookupTag(type);
            var a2_name = type.RemoveComponentSuffix();
            var a3_fieldNames = fieldNamesWithType(fieldInfos);
            var a4_fieldAssigns = fieldAssignments(fieldInfos);

            const string defaultFormat = @"
    public static void ReplaceComponent(this Entity entity, {0} component) {{
        entity.ReplaceComponent({1}.{2}, component);
    }}

    public static void Replace{2}(this Entity entity, {0} component) {{
        entity.ReplaceComponent({1}.{2}, component);
    }}
";
            const string fieldFormat = @"
    public static void Replace{2}(this Entity entity, {3}) {{
        {0} component;
        if (entity.HasComponent({1}.{2}))
            component = ({0})entity.GetComponent({1}.{2});
        else
            component = new {0}();

{4}
        entity.ReplaceComponent({1}.{2}, component);
    }}
";
            const string singleEntityFormat = @"
    public static void ReplaceSingle{2}(this EntityRepository repo) {{
        var entity = repo.GetSingleEntity({1}.{2});
        var component = ({0})entity.GetComponent({1}.{2});
        entity.ReplaceComponent({1}.{2}, component);
    }}
";
            const string singleEntityWithFieldsFormat = @"
    public static void ReplaceSingle{2}(this EntityRepository repo, {3}) {{
        var entity = repo.GetSingleEntity({1}.{2});
        var component = ({0})entity.GetComponent({1}.{2});
{4}
        entity.ReplaceComponent({1}.{2}, component);
    }}
";
            string format = string.Empty;
            if (isSingletonComponent(type)) {
            } else {
                if (a3_fieldNames.Length == 0)
                    format = isOnSingleEntity(type) ? defaultFormat + singleEntityFormat : defaultFormat;
                else
                    format = isOnSingleEntity(type) ? defaultFormat + fieldFormat + singleEntityWithFieldsFormat : defaultFormat + fieldFormat;
            }


            return str + string.Format(format, a0_type, a1_tag, a2_name, a3_fieldNames, a4_fieldAssigns);
        }

        static string addHasMethods(string str, Type type) {
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var a0_type = type;
            var a1_tag = indicesLookupTag(type);
            var a2_name = type.RemoveComponentSuffix();
            var a3_fieldNames = fieldNamesWithType(fieldInfos);
            var a4_fieldAssigns = fieldAssignments(fieldInfos);

            const string defaultFormat = @"
    public static bool Has{2}(this Entity entity) {{
        return entity.HasComponent({1}.{2});
    }}
";
            const string singleEntity = @"
    public static bool HasSingle{2}(this EntityRepository repo) {{
        return repo.GetSingleEntity({1}.{2}) != null;
    }}
";
            const string singletonFormat = @"
    public static bool Is{2}(this Entity entity) {{
        return entity.HasComponent({1}.{2});
    }}
";
            string format;
            if (isSingletonComponent(type))
                format = isOnSingleEntity(type) ? singletonFormat + singleEntity : singletonFormat;
            else
                format = isOnSingleEntity(type) ? singleEntity : defaultFormat;

            return str + string.Format(format, a0_type, a1_tag, a2_name, a3_fieldNames, a4_fieldAssigns);
        }

        static string addRemoveMethods(string str, Type type) {
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var a0_type = type;
            var a1_tag = indicesLookupTag(type);
            var a2_name = type.RemoveComponentSuffix();
            var a3_fieldNames = fieldNamesWithType(fieldInfos);
            var a4_fieldAssigns = fieldAssignments(fieldInfos);

            const string defaultFormat = @"
    public static void Remove{2}(this Entity entity) {{
        entity.RemoveComponent({1}.{2});
    }}
";
            const string singleEntity = @"
    public static void RemoveSingle{2}(this EntityRepository repo) {{
        repo.PushToPool(repo.GetSingleEntity({1}.{2}));
    }}
";
            const string singletonFormat = @"
    public static void Unflag{2}(this Entity entity) {{
        entity.RemoveComponent({1}.{2});
    }}
";
            string format;
            if (isSingletonComponent(type))
                format = isOnSingleEntity(type) ? singletonFormat + singleEntity : singletonFormat;
            else
                format = isOnSingleEntity(type) ? singleEntity : defaultFormat;

            return str + string.Format(format, a0_type, a1_tag, a2_name, a3_fieldNames, a4_fieldAssigns);
        }

        static string addFieldGetterMethods(string str, Type type) {
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var a0_type = type;
            var a1_tag = indicesLookupTag(type);
            var a2_name = type.RemoveComponentSuffix();

            const string fieldFormat = @"
    public static {0} Get{2}(this Entity entity) {{
        return ({0})entity.GetComponent({1}.{2});
    }}
";
            const string oneFieldFormat = @"
    public static {3} Get{2}Value(this Entity entity) {{
        return (({0})entity.GetComponent({1}.{2})).{4};
    }}
";
            const string multipleFieldFormat = @"
    public static {3} Get{2}{5}(this Entity entity) {{
        return (({0})entity.GetComponent({1}.{2})).{4};
    }}
 ";
            const string singleEntityFieldFormat = @"
    public static Entity GetSingle{2}Entity(this EntityRepository repo) {{
        return repo.GetSingleEntity({1}.{2});
    }}
    
    public static {0} GetSingle{2}(this EntityRepository repo) {{
        var entity = repo.GetSingleEntity({1}.{2});
        return ({0})entity.GetComponent({1}.{2});
    }}
";
            const string singleEntityOneField = @"
    public static {3} GetSingle{2}Value(this EntityRepository repo) {{
        var entity = repo.GetSingleEntity({1}.{2});
        return (({0})entity.GetComponent({1}.{2})).{4};
    }}
";
            const string singleEntityMultipleField = @"
    public static {3} GetSingle{2}{5}(this EntityRepository repo) {{
        var entity = repo.GetSingleEntity({1}.{2});
        return (({0})entity.GetComponent({1}.{2})).{4};
    }}
";
            var getters = string.Format(fieldFormat, a0_type, a1_tag, a2_name);
            string format = fieldInfos.Length == 1 ? oneFieldFormat : multipleFieldFormat;
            if (isOnSingleEntity(type)) {
                getters += string.Format(singleEntityFieldFormat, a0_type, a1_tag, a2_name);
                format += fieldInfos.Length == 1 ? singleEntityOneField : singleEntityMultipleField;
            }

            for (int i = 0; i < fieldInfos.Length; i++) {
                var a3_fieldType = getTypeName(fieldInfos[i].FieldType);
                var a4_fieldName = fieldInfos[i].Name;
                var a5_fieldNameCap = (char.ToUpper(a4_fieldName[0]) + a4_fieldName.Substring(1));
                getters += string.Format(format, a0_type, a1_tag, a2_name, a3_fieldType, a4_fieldName, a5_fieldNameCap);
            }

            return str + getters;
        }

        static string indicesLookupTag(Type type) {
            Attribute[] attrs = Attribute.GetCustomAttributes(type);
            foreach (Attribute attr in attrs) {
                var lookup = attr as EntityRepositoryAttribute;
                if (lookup != null)
                    return lookup.tag;
            }

            return "ComponentIds";
        }

        static string fieldNamesWithType(FieldInfo[] fieldInfos) {
            var namesWithType = string.Empty;
            for (int i = 0; i < fieldInfos.Length; i++) {
                var name = fieldInfos[i].Name;
                var type = getTypeName(fieldInfos[i].FieldType);
                namesWithType += type + " " + name;
                if (i < fieldInfos.Length - 1)
                    namesWithType += ", ";
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
            if (typeShortcuts.ContainsKey(type.Name))
                return typeShortcuts[type.Name];

            return simpleTypeString(type);
        }

        static string simpleTypeString(Type type) {
            Type[] typeStr = type.GetGenericArguments();
            var simplified = type.FullName.Split('`')[0];
            if (typeStr.Length > 0) {
                simplified += "<";
                for (int i = 0; i < typeStr.Length; ++i) {
                    simplified += simpleTypeString(typeStr[i]);
                    if (i < typeStr.Length - 1)
                        simplified += ", ";
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
                if (i < fieldInfos.Length - 1)
                    assignments += "\n";
            }

            return assignments;
        }

        static bool isSingletonComponent(Type type) {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            return (fields.Length == 1 && fields[0].Name.Equals("singleton"));
        }

        static bool isOnSingleEntity(Type type) {
            Attribute[] attrs = Attribute.GetCustomAttributes(type);
            foreach (Attribute attr in attrs)
                if (attr is SingleEntity)
                    return true;

            return false;
        }

        static string addCloseClass(string code) {
            return code + "\n}";
        }
    }
}

