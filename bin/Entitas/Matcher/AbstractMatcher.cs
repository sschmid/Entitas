using System;
using System.Collections.Generic;

namespace Entitas {
    public abstract class AbstractMatcher : IMatcher {
        public int[] indices { get { return _indices; } }

        readonly int[] _indices;
        readonly int _hash;

        protected AbstractMatcher(int[] indices) {
            var indicesSet = new HashSet<int>(indices);
            _indices = new int[indicesSet.Count];
            indicesSet.CopyTo(_indices);
            Array.Sort(_indices);

            int hash = GetType().GetHashCode();
            for (int i = 0, indicesLength = _indices.Length; i < indicesLength; i++) {
                hash ^= _indices[i] * 647;
            }
            hash ^= _indices.Length * 997;
            _hash = hash;
        }

        public abstract bool Matches(Entity entity);

        public override bool Equals(object obj) {
            if (obj == null || obj.GetType() != GetType() || obj.GetHashCode() != GetHashCode()) {
                return false;
            }

            var matcher = (AbstractMatcher)obj;
            if (matcher.indices.Length != _indices.Length) {
                return false;
            }

            for (int i = 0, _indicesLength = _indices.Length; i < _indicesLength; i++) {
                if (matcher.indices[i] != _indices[i]) {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode() {
            return _hash;
        }

        public override string ToString() {
            const string seperator = ", ";
            var indexStr = string.Empty;
            for (int i = 0, indicesLength = _indices.Length; i < indicesLength; i++) {
                indexStr += _indices[i] + seperator;
            }

            if (indexStr != string.Empty) {
                indexStr = indexStr.Substring(0, indexStr.Length - seperator.Length);
            }

            var name = GetType().Name;
            if (name.EndsWith("Matcher")) {
                name = name.Replace("Matcher", string.Empty);
            }
            return string.Format("{0}({1})", name, indexStr);
        }
    }
}
