namespace Entitas
{
    public partial class Matcher<TEntity> : IAllOfMatcher<TEntity> where TEntity : Entity
    {
        public int[] Indexes => _indexes ??= MergeIndexes(_allOfIndexes, _anyOfIndexes, _noneOfIndexes);
        public int[] AllOfIndexes => _allOfIndexes;
        public int[] AnyOfIndexes => _anyOfIndexes;
        public int[] NoneOfIndexes => _noneOfIndexes;

        public string[] ComponentNames { get; set; }

        int[] _indexes;
        int[] _allOfIndexes;
        int[] _anyOfIndexes;
        int[] _noneOfIndexes;

        Matcher() { }

        IAnyOfMatcher<TEntity> IAllOfMatcher<TEntity>.AnyOf(params int[] indexes)
        {
            _anyOfIndexes = DistinctIndexes(indexes);
            _indexes = null;
            _isHashCached = false;
            return this;
        }

        IAnyOfMatcher<TEntity> IAllOfMatcher<TEntity>.AnyOf(params IMatcher<TEntity>[] matchers) =>
            ((IAllOfMatcher<TEntity>)this).AnyOf(MergeIndexes(matchers));

        public INoneOfMatcher<TEntity> NoneOf(params int[] indexes)
        {
            _noneOfIndexes = DistinctIndexes(indexes);
            _indexes = null;
            _isHashCached = false;
            return this;
        }

        public INoneOfMatcher<TEntity> NoneOf(params IMatcher<TEntity>[] matchers) =>
            NoneOf(MergeIndexes(matchers));

        public bool Matches(TEntity entity) =>
            (_allOfIndexes == null || entity.HasComponents(_allOfIndexes))
            && (_anyOfIndexes == null || entity.HasAnyComponent(_anyOfIndexes))
            && (_noneOfIndexes == null || !entity.HasAnyComponent(_noneOfIndexes));
    }
}
