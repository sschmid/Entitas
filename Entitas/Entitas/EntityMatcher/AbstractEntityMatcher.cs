using System;
using System.Collections.Generic;

namespace Entitas {
    public abstract class AbstractEntityMatcher : IEntityMatcher {
        protected HashSet<Type> _types;
        protected int _hash;

        protected AbstractEntityMatcher() {
        }

        protected AbstractEntityMatcher(IEnumerable<Type> types) {
            _types = new HashSet<Type>(types);
            int hash = GetType().GetHashCode();
            foreach (var type in _types)
                hash ^= type.GetHashCode();
            _hash = hash;
        }

        public abstract bool Matches(Entity entity);

        public bool HasType(Type type) {
            return _types.Contains(type);
        }

        public override bool Equals(object obj) {
            if (obj == this)
                return true;

            if (obj == null || obj.GetType() != GetType())
                return false;

            return obj.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode() {
            return _hash;
        }

        public override string ToString() {
            const string seperator = ", ";
            var typesStr = string.Empty;
            foreach (var type in _types)
                typesStr += type + seperator;

            if (typesStr != string.Empty)
                typesStr = typesStr.Substring(0, typesStr.Length - seperator.Length);

            return string.Format("{0}({1})", GetType().Name, typesStr);
        }
    }
}
