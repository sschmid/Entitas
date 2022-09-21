namespace Entitas
{
    public partial class Matcher<TEntity> : IAllOfMatcher<TEntity> where TEntity : class, IEntity
    {
        public int[] indices => _indices ?? (_indices = mergeIndices(_allOfIndices, _anyOfIndices, _noneOfIndices));
        public int[] allOfIndices => _allOfIndices;
        public int[] anyOfIndices => _anyOfIndices;
        public int[] noneOfIndices => _noneOfIndices;

        public string[] componentNames { get; set; }

        int[] _indices;
        int[] _allOfIndices;
        int[] _anyOfIndices;
        int[] _noneOfIndices;

        Matcher() { }

        IAnyOfMatcher<TEntity> IAllOfMatcher<TEntity>.AnyOf(params int[] indices)
        {
            _anyOfIndices = distinctIndices(indices);
            _indices = null;
            _isHashCached = false;
            return this;
        }

        IAnyOfMatcher<TEntity> IAllOfMatcher<TEntity>.AnyOf(params IMatcher<TEntity>[] matchers) =>
            ((IAllOfMatcher<TEntity>)this).AnyOf(mergeIndices(matchers));

        public INoneOfMatcher<TEntity> NoneOf(params int[] indices)
        {
            _noneOfIndices = distinctIndices(indices);
            _indices = null;
            _isHashCached = false;
            return this;
        }

        public INoneOfMatcher<TEntity> NoneOf(params IMatcher<TEntity>[] matchers) =>
            NoneOf(mergeIndices(matchers));

        public bool Matches(TEntity entity) =>
            (_allOfIndices == null || entity.HasComponents(_allOfIndices))
            && (_anyOfIndices == null || entity.HasAnyComponent(_anyOfIndices))
            && (_noneOfIndices == null || !entity.HasAnyComponent(_noneOfIndices));
    }
}
