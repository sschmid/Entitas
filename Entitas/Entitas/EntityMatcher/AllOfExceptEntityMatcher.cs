using System;
using System.Collections.Generic;

namespace Entitas {
    public class AllOfExceptEntityMatcher : AbstractEntityMatcher {
        public HashSet<Type> exceptTypes { get { return _exceptTypes; } }

        readonly HashSet<Type> _exceptTypes;

        public AllOfExceptEntityMatcher(IEnumerable<Type> types, IEnumerable<Type> except) {
            _types = new HashSet<Type>(types);
            _exceptTypes = new HashSet<Type>(except);
            int hash = GetType().GetHashCode();
            foreach (var type in _types)
                hash ^= type.GetHashCode();

            hash >>= 1;

            foreach (var type in _exceptTypes)
                hash ^= type.GetHashCode();

            _hash = hash;
        }

        public override bool Matches(Entity entity) {
            return entity.HasComponents(_types) && !entity.HasAnyComponent(_exceptTypes);
        }

        public override string ToString() {
            const string seperator = ", ";
            var typesStr = string.Empty;
            foreach (var type in _types)
                typesStr += type + seperator;

            if (typesStr != string.Empty)
                typesStr = typesStr.Substring(0, typesStr.Length - seperator.Length);

            var exceptTypesStr = string.Empty;
            foreach (var type in _exceptTypes)
                exceptTypesStr += type + seperator;

            if (exceptTypesStr != string.Empty)
                exceptTypesStr = exceptTypesStr.Substring(0, exceptTypesStr.Length - seperator.Length);

            return string.Format("{0}({1})({2})", GetType().Name, typesStr, exceptTypesStr);
        }
    }
}
