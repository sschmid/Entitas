using System;
using System.Collections.Generic;
using System.Linq;

namespace Entitas {
    public abstract class AbstractEntityMatcher : IEntityMatcher {
        public int[] indices { get { return _indices; } }

        readonly int[] _indices;
        readonly int _hash;

        protected AbstractEntityMatcher() {
        }

        protected AbstractEntityMatcher(int[] indices) {
            _indices = new HashSet<int>(indices).ToArray();
            int hash = GetType().GetHashCode();
            for (int i = 0, indicesLength = _indices.Length; i < indicesLength; i++)
                hash ^= _indices[i] * 977;

            hash ^= _indices.Length * 997;
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
            var indexStr = string.Empty;
            for (int i = 0, indicesLength = _indices.Length; i < indicesLength; i++)
                indexStr += _indices[i] + seperator;

            if (indexStr != string.Empty)
                indexStr = indexStr.Substring(0, indexStr.Length - seperator.Length);

            return string.Format("{0}({1})", GetType().Name, indexStr);
        }
    }
}
