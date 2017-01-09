using System;
using System.Collections.Generic;

namespace Entitas {

    public partial class Matcher {

        public static IAllOfMatcher AllOf(params int[] indices) {
            var matcher = new Matcher();
            matcher._allOfIndices = distinctIndices(indices);
            return matcher;
        }

        public static IAllOfMatcher AllOf(params IMatcher[] matchers) {
            var allOfMatcher = (Matcher)Matcher.AllOf(mergeIndices(matchers));
            setComponentNames(allOfMatcher, matchers);
            return allOfMatcher;
        }

        public static IAnyOfMatcher AnyOf(params int[] indices) {
            var matcher = new Matcher();
            matcher._anyOfIndices = distinctIndices(indices);
            return matcher;
        }

        public static IAnyOfMatcher AnyOf(params IMatcher[] matchers) {
            var anyOfMatcher = (Matcher)Matcher.AnyOf(mergeIndices(matchers));
            setComponentNames(anyOfMatcher, matchers);
            return anyOfMatcher;
        }

        static int[] mergeIndices(IMatcher[] matchers) {
            var indices = new int[matchers.Length];
            for (int i = 0; i < matchers.Length; i++) {
                var matcher = matchers[i];
                if(matcher.indices.Length != 1) {
                    throw new MatcherException(matcher);
                }
                indices[i] = matcher.indices[0];
            }

            return indices;
        }

        static string[] getComponentNames(IMatcher[] matchers) {
            for (int i = 0; i < matchers.Length; i++) {
                var matcher = matchers[i] as Matcher;
                if(matcher != null && matcher.componentNames != null) {
                    return matcher.componentNames;
                }
            }

            return null;
        }

        static void setComponentNames(Matcher matcher, IMatcher[] matchers) {
            var componentNames = getComponentNames(matchers);
            if(componentNames != null) {
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
}
