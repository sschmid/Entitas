using System;

namespace Entitas {

    public partial class Matcher : IAllOfMatcher, IAnyOfMatcher, INoneOfMatcher {

        public int[] indices {
            get {
                if(_indices == null) {
                    _indices = mergeIndices();
                }
                return _indices;
            }
        }

        public int[] allOfIndices { get { return _allOfIndices; } }
        public int[] anyOfIndices { get { return _anyOfIndices; } }
        public int[] noneOfIndices { get { return _noneOfIndices; } }

        int[] _indices;
        int[] _allOfIndices;
        int[] _anyOfIndices;
        int[] _noneOfIndices;

        Matcher() {
        }

        IAnyOfMatcher IAllOfMatcher.AnyOf(params int[] indices) {
            _anyOfIndices = distinctIndices(indices);
            _indices = null;
            return this;
        }

        IAnyOfMatcher IAllOfMatcher.AnyOf(params IMatcher[] matchers) {
            return ((IAllOfMatcher)this).AnyOf(mergeIndices(matchers));
        }

        public INoneOfMatcher NoneOf(params int[] indices) {
            _noneOfIndices = distinctIndices(indices);
            _indices = null;
            return this;
        }

        public INoneOfMatcher NoneOf(params IMatcher[] matchers) {
            return NoneOf(mergeIndices(matchers));
        }

        public bool Matches(IEntity entity) {
            return (_allOfIndices == null || entity.HasComponents(_allOfIndices))
                && (_anyOfIndices == null || entity.HasAnyComponent(_anyOfIndices))
                && (_noneOfIndices == null || !entity.HasAnyComponent(_noneOfIndices));
        }

        int[] mergeIndices() {
            var indicesList = EntitasCache.GetIntList();

                if(_allOfIndices != null) {
                    indicesList.AddRange(_allOfIndices);
                }
                if(_anyOfIndices != null) {
                    indicesList.AddRange(_anyOfIndices);
                }
                if(_noneOfIndices != null) {
                    indicesList.AddRange(_noneOfIndices);
                }

                var mergedIndices = distinctIndices(indicesList);

            EntitasCache.PushIntList(indicesList);

            return mergedIndices;
        }
    }

    public class MatcherException : Exception {
        public MatcherException(IMatcher matcher) : base(
            "matcher.indices.Length must be 1 but was " + matcher.indices.Length) {
        }
    }
}
