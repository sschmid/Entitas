namespace Entitas {

    public partial class Matcher<TEntity> : IAllOfMatcher<TEntity> where TEntity : class, IEntity {

        public int[] indices {
            get {
                if (_indices == null) {
                    _indices = mergeIndices(_allOfIndices, _anyOfIndices, _noneOfIndices);
                }
                return _indices;
            }
        }

        public int[] allOfIndices { get { return _allOfIndices; } }
        public int[] anyOfIndices { get { return _anyOfIndices; } }
        public int[] noneOfIndices { get { return _noneOfIndices; } }

        public string[] componentNames { get; set; }

        int[] _indices;
        int[] _allOfIndices;
        int[] _anyOfIndices;
        int[] _noneOfIndices;

        Matcher() {
        }

        IAnyOfMatcher<TEntity> IAllOfMatcher<TEntity>.AnyOf(params int[] indices) {
            _anyOfIndices = distinctIndices(indices);
            _indices = null;
            _isHashCached = false;
            return this;
        }

        IAnyOfMatcher<TEntity> IAllOfMatcher<TEntity>.AnyOf(params IMatcher<TEntity>[] matchers) {
            return ((IAllOfMatcher<TEntity>)this).AnyOf(mergeIndices(matchers));
        }

        public INoneOfMatcher<TEntity> NoneOf(params int[] indices) {
            _noneOfIndices = distinctIndices(indices);
            _indices = null;
            _isHashCached = false;
            return this;
        }

        public INoneOfMatcher<TEntity> NoneOf(params IMatcher<TEntity>[] matchers) {
            return NoneOf(mergeIndices(matchers));
        }

        public bool Matches(TEntity entity) {
            return (_allOfIndices == null || entity.HasComponents(_allOfIndices))
                   && (_anyOfIndices == null || entity.HasAnyComponent(_anyOfIndices))
                   && (_noneOfIndices == null || !entity.HasAnyComponent(_noneOfIndices));
        }
    }
}
