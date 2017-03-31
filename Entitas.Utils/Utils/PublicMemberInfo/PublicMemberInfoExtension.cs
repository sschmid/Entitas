using System;
using System.Collections.Generic;
using System.Reflection;

namespace Entitas {

    public static class PublicMemberInfoExtension {

        public static List<PublicMemberInfo> GetPublicMemberInfos(this Type type) {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            var fieldInfos = type.GetFields(bindingFlags);
            var propertyInfos = type.GetProperties(bindingFlags);
            var memberInfos = new List<PublicMemberInfo>(
                fieldInfos.Length + propertyInfos.Length
            );

            for(int i = 0; i < fieldInfos.Length; i++) {
                memberInfos.Add(new PublicMemberInfo(fieldInfos[i]));
            }

            for(int i = 0; i < propertyInfos.Length; i++) {
                var propertyInfo = propertyInfos[i];
                if(propertyInfo.CanRead && propertyInfo.CanWrite && propertyInfo.GetIndexParameters().Length == 0) {
                    memberInfos.Add(new PublicMemberInfo(propertyInfo));
                }
            }

            return memberInfos;
        }

        public static object PublicMemberClone(this object obj) {
            var clone = Activator.CreateInstance(obj.GetType());
            CopyPublicMemberValues(obj, clone);
            return clone;
        }

        public static T PublicMemberClone<T>(this object obj) where T : new() {
            var clone = new T();
            CopyPublicMemberValues(obj, clone);
            return clone;
        }

        public static void CopyPublicMemberValues(this object source, object target) {
            var memberInfos = source.GetType().GetPublicMemberInfos();
            for(int i = 0; i < memberInfos.Count; i++) {
                var info = memberInfos[i];
                info.SetValue(target, info.GetValue(source));
            }
        }
    }
}
