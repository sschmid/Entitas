namespace Entitas {

    public partial class Matcher<TEntity> {

        public override bool Equals(object obj) {
            if (obj == null || obj.GetType() != GetType() ||
                obj.GetHashCode() != GetHashCode()) {
                return false;
            }

            var matcher = (Matcher<TEntity>)obj;
            if (!equalIndices(matcher.allOfIndices, _allOfIndices)) {
                return false;
            }
            if (!equalIndices(matcher.anyOfIndices, _anyOfIndices)) {
                return false;
            }
            if (!equalIndices(matcher.noneOfIndices, _noneOfIndices)) {
                return false;
            }

            return true;
        }

        static bool equalIndices(int[] i1, int[] i2) {
            if ((i1 == null) != (i2 == null)) {
                return false;
            }
            if (i1 == null) {
                return true;
            }
            if (i1.Length != i2.Length) {
                return false;
            }

            for (int i = 0; i < i1.Length; i++) {
                if (i1[i] != i2[i]) {
                    return false;
                }
            }

            return true;
        }

        int _hash;
        bool _isHashCached;

        public override int GetHashCode() {
            if (!_isHashCached) {
                var hash = GetType().GetHashCode();
                hash = applyHash(hash, _allOfIndices, 3, 53);
                hash = applyHash(hash, _anyOfIndices, 307, 367);
                hash = applyHash(hash, _noneOfIndices, 647, 683);
                _hash = hash;
                _isHashCached = true;
            }

            return _hash;
        }

        static int applyHash(int hash, int[] indices, int i1, int i2) {
            if (indices != null) {
                for (int i = 0; i < indices.Length; i++) {
                    hash ^= indices[i] * i1;
                }
                hash ^= indices.Length * i2;
            }

            return hash;
        }
    }
}
