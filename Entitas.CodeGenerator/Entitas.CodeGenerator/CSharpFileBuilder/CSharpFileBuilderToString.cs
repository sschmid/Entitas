using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Entitas.CodeGenerator {
    public partial class CSharpFileBuilder {

        public override string ToString() {
            return getUsings() + getNamespaces();
        }

        string getUsings() {
            if (_usings.Count == 0) {
                return string.Empty;
            }

            const string format = "using {0};\n";
            return _usings.Aggregate(string.Empty, (result, u) =>
                result + string.Format(format, u)) + "\n";
        }

        string getNamespaces() {
            const string format = "namespace {0}";
            var namespaces = _namespaceDescriptions
                .Select(nsd => {
                    var classes = getClasses(nsd);
                    return string.IsNullOrEmpty(nsd.name)
                        ? classes
                        : codeBlock(string.Format(format, nsd.name), classes);
                })
                .Where(classes => !string.IsNullOrEmpty(classes))
                .ToArray();

            return string.Join("\n\n", namespaces);
        }

        static string getClasses(NamespaceDescription nsd) {
            const string format = "{0}class {1}";
            var classes = nsd.classDescriptions
                .Select(cd => {
                    var classDeclaration = string.Format(format, getModifiers(cd.modifiers), cd.name);

                    var inheritance = getInheritance(cd);
                    if (!string.IsNullOrEmpty(inheritance)) {
                        classDeclaration += " : " + inheritance;
                    }

                    var constructors = getConstructors(cd);
                    var properties = getProperties(cd);
                    var fields = getFields(cd);
                    var methods = getMethods(cd);

                    var body = constructors;

                    if (!string.IsNullOrEmpty(body) && !string.IsNullOrEmpty(properties)) {
                        body += "\n\n";
                    }
                    body += properties;

                    if (!string.IsNullOrEmpty(body) && !string.IsNullOrEmpty(fields)) {
                        body += "\n\n";
                    }
                    body += fields;

                    if (!string.IsNullOrEmpty(body) && !string.IsNullOrEmpty(methods)) {
                        body += "\n\n";
                    }
                    body += methods;

                    return codeBlock(classDeclaration, body);
                }).ToArray();

            return string.Join("\n\n", classes);
        }

        static string getInheritance(ClassDescription cd) {
            var inheritedTypes = new List<string>();
            if (cd.baseClass != null) {
                inheritedTypes.Add(cd.baseClass);
            }
            inheritedTypes.AddRange(cd.interfaces);

            return string.Join(", ", inheritedTypes.ToArray());
        }

        static string getConstructors(ClassDescription cd) {
            if (cd.constructorDescriptions.Length == 0) {
                return string.Empty;
            }

            const string format = "{0}{1}({2})";
            const string baseCallFormat = " : base({0})";
            var constructors = cd.constructorDescriptions
                .Select(ctor => {
                    var c = string.Format(format, getModifiers(ctor.modifiers), ctor.name, getParameters(ctor.parameters));
                    if (!string.IsNullOrEmpty(ctor.baseCall)) {
                        c += string.Format(baseCallFormat, ctor.baseCall);
                    }

                    return codeBlock(c, ctor.body);
                }).ToArray();

            return string.Join("\n\n", constructors);
        }

        static string getProperties(ClassDescription cd) {
            if (cd.propertyDescriptions.Length == 0) {
                return string.Empty;
            }

            const string defaultFormat = "{0}{1} {2} {{ get; set; }}";
            const string format = "{0}{1} {2}";

            var properties = cd.propertyDescriptions
                .Select(pd => {
                    var prop = string.Format(format, getModifiers(pd.modifiers), pd.type, pd.name);
                    if (!string.IsNullOrEmpty(pd.getter)) {
                        return !string.IsNullOrEmpty(pd.setter)
                            ? codeBlock(prop, codeBlock("get", pd.getter) + "\n" + codeBlock("set", pd.setter))
                            : codeBlock(prop, codeBlock("get", pd.getter));
                    }

                    return !string.IsNullOrEmpty(pd.setter)
                        ? codeBlock(prop, codeBlock("set", pd.setter))
                        : string.Format(defaultFormat, getModifiers(pd.modifiers), pd.type, pd.name);
                }).ToArray();

            return string.Join("\n", properties);
        }

        static string getFields(ClassDescription cd) {
            if (cd.fieldDescriptions.Length == 0) {
                return string.Empty;
            }

            const string format = "{0}{1} {2};";
            const string formatWithValue = "{0}{1} {2} = {3};";
            var fields = cd.fieldDescriptions
                .Select(fd => {
                    return string.IsNullOrEmpty(fd.value)
                        ? string.Format(format, getModifiers(fd.modifiers), fd.type, fd.name)
                        : string.Format(formatWithValue, getModifiers(fd.modifiers), fd.type, fd.name, fd.value);
                }).ToArray();

            return string.Join("\n", fields);
        }

        static string getMethods(ClassDescription cd) {
            if (cd.methodDescriptions.Length == 0) {
                return string.Empty;
            }

            const string format = "{0}{1} {2}({3})";
            var methods = cd.methodDescriptions
                .Select(md => codeBlock(string.Format(format, getModifiers(md.modifiers),
                    md.returnType ?? "void", md.name, getParameters(md.parameters)), md.body)).ToArray();

            return string.Join("\n\n", methods);
        }

        static string getParameters(MethodParameterDescription[] parameters) {
            if (parameters == null || parameters.Length == 0) {
                return string.Empty;
            }

            const string format = "{0}{1} {2}";
            const string defaultValueFormat = "{0}{1} {2} = {3}";
            var methods = parameters
                .Select(p => {
                    var keyword = string.IsNullOrEmpty(p.keyword)
                        ? string.Empty
                        : p.keyword + " ";

                    return string.IsNullOrEmpty(p.defaultValue)
                        ? string.Format(format, keyword, p.type, p.name)
                        : string.Format(defaultValueFormat, keyword, p.type, p.name, p.defaultValue);
                }).ToArray();

            return string.Join(", ", methods);
        }

        static string codeBlock(string name, string body) {
            const string emptyBlockFormat = "{0} {{\n}}";
            const string blockFormat = "{0} {{\n{1}\n}}";
            if (!string.IsNullOrEmpty(body)) {
                body = indentNewLines(body);
            }

            return string.IsNullOrEmpty(body)
                ? string.Format(emptyBlockFormat, name)
                : string.Format(blockFormat, name, body);
        }

        static string indentNewLines(string text) {
            const string indent = "    ";

            var indentedText = new StringBuilder();
            using (var reader = new StringReader(text)) {
                var line = reader.ReadLine();
                while (line != null) {
                    var aLine = line;
                    line = reader.ReadLine();

                    if (aLine != string.Empty) {
                        indentedText.Append(indent);
                        indentedText.Append(aLine);
                    }

                    if (line != null) {
                        indentedText.Append("\n");
                    }
                }
            }

            return indentedText.ToString();
        }

        static string getModifiers(string[] modifiers) {
            return modifiers == null
                ? string.Empty
                : modifiers.Aggregate(string.Empty, (result, m) => result + m + " ");
        }
    }
}