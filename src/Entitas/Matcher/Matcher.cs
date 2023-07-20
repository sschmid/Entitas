namespace Entitas
{
    public partial class Matcher<TEntity> : IAllOfMatcher<TEntity> where TEntity : class, IEntity
    {
        public int[] Indices => _indices ??= MergeIndices(_allOfIndices, _anyOfIndices, _noneOfIndices);
        public int[] AllOfIndices => _allOfIndices;
        public int[] AnyOfIndices => _anyOfIndices;
        public int[] NoneOfIndices => _noneOfIndices;

        public string[] ComponentNames { get; set; }

        int[] _indices;
        int[] _allOfIndices;
        int[] _anyOfIndices;
        int[] _noneOfIndices;

        Matcher() { }

        IAnyOfMatcher<TEntity> IAllOfMatcher<TEntity>.AnyOf(params int[] indices)
        {
            _anyOfIndices = DistinctIndices(indices);
            _indices = null;
            _isHashCached = false;
            return this;
        }

        IAnyOfMatcher<TEntity> IAllOfMatcher<TEntity>.AnyOf(params IMatcher<TEntity>[] matchers) =>
            ((IAllOfMatcher<TEntity>)this).AnyOf(MergeIndices(matchers));

        public INoneOfMatcher<TEntity> NoneOf(params int[] indices)
        {
            _noneOfIndices = DistinctIndices(indices);
            _indices = null;
            _isHashCached = false;
            return this;
        }

        public INoneOfMatcher<TEntity> NoneOf(params IMatcher<TEntity>[] matchers) =>
            NoneOf(MergeIndices(matchers));

        public bool Matches(TEntity entity) =>
            (_allOfIndices == null || entity.HasComponents(_allOfIndices))
            && (_anyOfIndices == null || entity.HasAnyComponent(_anyOfIndices))
            && (_noneOfIndices == null || !entity.HasAnyComponent(_noneOfIndices));
    }
}
