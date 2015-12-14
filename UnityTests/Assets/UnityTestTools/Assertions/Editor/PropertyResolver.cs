using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityTest
{
    [Serializable]
    public class PropertyResolver
    {
        public string[] ExcludedFieldNames { get; set; }
        public Type[] ExcludedTypes { get; set; }
        public Type[] AllowedTypes { get; set; }

        public PropertyResolver()
        {
            ExcludedFieldNames = new string[] { };
            ExcludedTypes = new Type[] { };
            AllowedTypes = new Type[] { };
        }

        public IList<string> GetFieldsAndPropertiesUnderPath(GameObject go, string propertPath)
        {
            propertPath = propertPath.Trim();
            if (!PropertyPathIsValid(propertPath))
            {
                throw new ArgumentException("Incorrect property path: " + propertPath);
            }

            var idx = propertPath.LastIndexOf('.');

            if (idx < 0)
            {
                var components = GetFieldsAndPropertiesFromGameObject(go, 2, null);
                return components;
            }

            var propertyToSearch = propertPath;
            Type type;
            if (MemberResolver.TryGetMemberType(go, propertyToSearch, out type))
            {
                idx = propertPath.Length - 1;
            }
            else
            {
                propertyToSearch = propertPath.Substring(0, idx);
                if (!MemberResolver.TryGetMemberType(go, propertyToSearch, out type))
                {
                    var components = GetFieldsAndPropertiesFromGameObject(go, 2, null);
                    return components.Where(s => s.StartsWith(propertPath.Substring(idx + 1))).ToArray();
                }
            }

            var resultList = new List<string>();
            var path = "";
            if (propertyToSearch.EndsWith("."))
                propertyToSearch = propertyToSearch.Substring(0, propertyToSearch.Length - 1);
            foreach (var c in propertyToSearch)
            {
                if (c == '.')
                    resultList.Add(path);
                path += c;
            }
            resultList.Add(path);
            foreach (var prop in type.GetProperties().Where(info => info.GetIndexParameters().Length == 0))
            {
                if (prop.Name.StartsWith(propertPath.Substring(idx + 1)))
                    resultList.Add(propertyToSearch + "." + prop.Name);
            }
            foreach (var prop in type.GetFields())
            {
                if (prop.Name.StartsWith(propertPath.Substring(idx + 1)))
                    resultList.Add(propertyToSearch + "." + prop.Name);
            }
            return resultList.ToArray();
        }

        internal bool PropertyPathIsValid(string propertPath)
        {
            if (propertPath.StartsWith("."))
                return false;
            if (propertPath.IndexOf("..") >= 0)
                return false;
            if (Regex.IsMatch(propertPath, @"\s"))
                return false;
            return true;
        }

        public IList<string> GetFieldsAndPropertiesFromGameObject(GameObject gameObject, int depthOfSearch, string extendPath)
        {
            if (depthOfSearch < 1) throw new ArgumentOutOfRangeException("depthOfSearch has to be greater than 0");

            var goVals = GetPropertiesAndFieldsFromType(typeof(GameObject),
                                                        depthOfSearch - 1).Select(s => "gameObject." + s);

            var result = new List<string>();
            if (AllowedTypes == null || !AllowedTypes.Any() || AllowedTypes.Contains(typeof(GameObject)))
                result.Add("gameObject");
            result.AddRange(goVals);

            foreach (var componentType in GetAllComponents(gameObject))
            {
                if (AllowedTypes == null || !AllowedTypes.Any() || AllowedTypes.Any(t => t.IsAssignableFrom(componentType)))
                    result.Add(componentType.Name);

                if (depthOfSearch > 1)
                {
                    var vals = GetPropertiesAndFieldsFromType(componentType, depthOfSearch - 1);
                    var valsFullName = vals.Select(s => componentType.Name + "." + s);
                    result.AddRange(valsFullName);
                }
            }

            if (!string.IsNullOrEmpty(extendPath))
            {
                var memberResolver = new MemberResolver(gameObject, extendPath);
                var pathType = memberResolver.GetMemberType();
                var vals = GetPropertiesAndFieldsFromType(pathType, depthOfSearch - 1);
                var valsFullName = vals.Select(s => extendPath + "." + s);
                result.AddRange(valsFullName);
            }

            return result;
        }

        private string[] GetPropertiesAndFieldsFromType(Type type, int level)
        {
            level--;

            var result = new List<string>();
            var fields = new List<MemberInfo>();
            fields.AddRange(type.GetFields().Where(f => !Attribute.IsDefined(f, typeof(ObsoleteAttribute))).ToArray());
            fields.AddRange(type.GetProperties().Where(info => info.GetIndexParameters().Length == 0 && !Attribute.IsDefined(info, typeof(ObsoleteAttribute))).ToArray());

            foreach (var member in fields)
            {
                var memberType = GetMemberFieldType(member);
                var memberTypeName = memberType.Name;

                if (AllowedTypes == null
                    || !AllowedTypes.Any()
                    || (AllowedTypes.Any(t => t.IsAssignableFrom(memberType)) && !ExcludedFieldNames.Contains(memberTypeName)))
                {
                    result.Add(member.Name);
                }

                if (level > 0 && IsTypeOrNameNotExcluded(memberType, memberTypeName))
                {
                    var vals = GetPropertiesAndFieldsFromType(memberType, level);
                    var valsFullName = vals.Select(s => member.Name + "." + s);
                    result.AddRange(valsFullName);
                }
            }
            return result.ToArray();
        }

        private Type GetMemberFieldType(MemberInfo info)
        {
            if (info.MemberType == MemberTypes.Property)
                return (info as PropertyInfo).PropertyType;
            if (info.MemberType == MemberTypes.Field)
                return (info as FieldInfo).FieldType;
            throw new Exception("Only properties and fields are allowed");
        }

        internal Type[] GetAllComponents(GameObject gameObject)
        {
            var result = new List<Type>();
            var components = gameObject.GetComponents(typeof(Component));
            foreach (var component in components)
            {
                var componentType = component.GetType();
                if (IsTypeOrNameNotExcluded(componentType, null))
                {
                    result.Add(componentType);
                }
            }
            return result.ToArray();
        }

        private bool IsTypeOrNameNotExcluded(Type memberType, string memberTypeName)
        {
            return !ExcludedTypes.Contains(memberType) && !ExcludedFieldNames.Contains(memberTypeName);
        }
    }
}
