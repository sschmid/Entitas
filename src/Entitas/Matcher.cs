namespace Entitas
{
    public partial class Matcher<TEntity> : IAllOfMatcher<TEntity> where TEntity : class, IEntity
    {
        public int[] Indexes => _indexes ?? (_indexes = mergeIndexes(_allOfIndexes, _anyOfIndexes, _noneOfIndexes));
        public int[] AllOfIndexes => _allOfIndexes;
        public int[] AnyOfIndexes => _anyOfIndexes;
        public int[] NoneOfIndexes => _noneOfIndexes;

        public string[] componentNames { get; set; }

        int[] _indexes;
        int[] _allOfIndexes;
        int[] _anyOfIndexes;
        int[] _noneOfIndexes;

        Matcher() { }

        IAnyOfMatcher<TEntity> IAllOfMatcher<TEntity>.AnyOf(params int[] indexes)
        {
            _anyOfIndexes = distinctIndexes(indexes);
            _indexes = null;
            _isHashCached = false;
            return this;
        }

        IAnyOfMatcher<TEntity> IAllOfMatcher<TEntity>.AnyOf(params IMatcher<TEntity>[] matchers) =>
            ((IAllOfMatcher<TEntity>)this).AnyOf(mergeIndexes(matchers));

        public INoneOfMatcher<TEntity> NoneOf(params int[] indexes)
        {
            _noneOfIndexes = distinctIndexes(indexes);
            _indexes = null;
            _isHashCached = false;
            return this;
        }

        public INoneOfMatcher<TEntity> NoneOf(params IMatcher<TEntity>[] matchers) =>
            NoneOf(mergeIndexes(matchers));

        public bool Matches(TEntity entity) =>
            (_allOfIndexes == null || entity.HasComponents(_allOfIndexes))
            && (_anyOfIndexes == null || entity.HasAnyComponent(_anyOfIndexes))
            && (_noneOfIndexes == null || !entity.HasAnyComponent(_noneOfIndexes));
    }
}
