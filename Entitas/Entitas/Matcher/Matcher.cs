using System;
using System.Collections.Generic;

namespace Entitas {

    public partial class Matcher : IAllOfMatcher, IAnyOfMatcher, INoneOfMatcher {

        public int[] indices {
            get {
                if (_indices == null) {
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

        public bool Matches(Entity entity) {
            var matchesAllOf = _allOfIndices == null || entity.HasComponents(_allOfIndices);
            var matchesAnyOf = _anyOfIndices == null || entity.HasAnyComponent(_anyOfIndices);
            var matchesNoneOf = _noneOfIndices == null || !entity.HasAnyComponent(_noneOfIndices);
            return matchesAllOf && matchesAnyOf && matchesNoneOf;
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

        static int[] mergeIndices(IMatcher[] matchers) {
            var indices = new int[matchers.Length];
            for (int i = 0; i < matchers.Length; i++) {
                var matcher = matchers[i];
                if (matcher.indices.Length != 1) {
                    throw new MatcherException(matcher);
                }
                indices[i] = matcher.indices[0];
            }

            return indices;
        }

        static string[] getComponentNames(IMatcher[] matchers) {
            for (int i = 0; i < matchers.Length; i++) {
                var matcher = matchers[i] as Matcher;
                if (matcher != null && matcher.componentNames != null) {
                    return matcher.componentNames;
                }
            }

            return null;
        }

        static void setComponentNames(Matcher matcher, IMatcher[] matchers) {
            var componentNames = getComponentNames(matchers);
            if (componentNames != null) {
                matcher.componentNames = componentNames;
            }
        }

        static int[] distinctIndices(IList<int> indices) {
            var indicesSet = EntitasCache.GetIntHashSet();

                foreach(var index in indices) {
                    indicesSet.Add(index);
                }
                var uniqueIndices = new int[indicesSet.Count];
                indicesSet.CopyTo(uniqueIndices);
                Array.Sort(uniqueIndices);

            EntitasCache.PushIntHashSet(indicesSet);

            return uniqueIndices;
        }
    }

    public class MatcherException : Exception {
        public MatcherException(IMatcher matcher) : base("matcher.indices.Length must be 1 but was " + matcher.indices.Length) {
        }
    }
}
