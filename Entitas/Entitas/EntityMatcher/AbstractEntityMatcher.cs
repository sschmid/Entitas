using System;
using System.Collections.Generic;
using System.Linq;

namespace Entitas {
    public abstract class AbstractEntityMatcher : IEntityMatcher {
        public Type[] types { get { return _types; } }

        readonly Type[] _types;
        readonly int _hash;

        protected AbstractEntityMatcher() {
        }

        protected AbstractEntityMatcher(Type[] types) {
            _types = new HashSet<Type>(types).ToArray();
            int hash = GetType().GetHashCode();
            foreach (var type in _types)
                hash ^= type.GetHashCode();
            _hash = hash;
        }

        public abstract bool Matches(Entity entity);

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
