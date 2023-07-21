namespace Entitas
{
    public partial class Matcher<TEntity>
    {
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType() || obj.GetHashCode() != GetHashCode())
                return false;

            var matcher = (Matcher<TEntity>)obj;
            if (!EqualIndexes(matcher._allOfIndexes, _allOfIndexes))
                return false;

            if (!EqualIndexes(matcher._anyOfIndexes, _anyOfIndexes))
                return false;

            if (!EqualIndexes(matcher._noneOfIndexes, _noneOfIndexes))
                return false;

            return true;
        }

        static bool EqualIndexes(int[] i1, int[] i2)
        {
            if ((i1 == null) != (i2 == null))
                return false;

            if (i1 == null)
                return true;

            if (i1.Length != i2.Length)
                return false;

            for (var i = 0; i < i1.Length; i++)
                if (i1[i] != i2[i])
                    return false;

            return true;
        }

        int _hash;
        bool _isHashCached;

        public override int GetHashCode()
        {
            if (!_isHashCached)
            {
                var hash = GetType().GetHashCode();
                hash = ApplyHash(hash, _allOfIndexes, 3, 53);
                hash = ApplyHash(hash, _anyOfIndexes, 307, 367);
                hash = ApplyHash(hash, _noneOfIndexes, 647, 683);
                _hash = hash;
                _isHashCached = true;
            }

            return _hash;
        }

        static int ApplyHash(int hash, int[] indexes, int i1, int i2)
        {
            if (indexes != null)
            {
                for (var i = 0; i < indexes.Length; i++)
                    hash ^= indexes[i] * i1;

                hash ^= indexes.Length * i2;
            }

            return hash;
        }
    }
}
