using System;

namespace Entitas.VisualDebugging.Unity
{
    public static class TypeHelper
    {
        /// <summary>
        /// Returns a type name including generic type parameters.
        /// </summary>
        public static string GetTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                var simpleName = type.Name.Substring(0, type.Name.IndexOf('`'));
                string genericTypeParams = string.Empty;
                for (int i = 0; i < type.GenericTypeArguments.Length; i++)
                {
                    if (i > 0) genericTypeParams += ",";
                    genericTypeParams += GetTypeName(type.GenericTypeArguments[i]);
                }
                return string.Format("{0}<{1}>", simpleName, string.Join(", ", genericTypeParams));
            }
            return type.Name;
        }
    }
}
