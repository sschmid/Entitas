using System;
using System.Collections.Generic;

namespace Entitas {

    public partial class Matcher<TEntity> {

        public static IAllOfMatcher<TEntity> AllOf(params int[] indices) {
            var matcher = new Matcher<TEntity>();
            matcher._allOfIndices = distinctIndices(indices);
            return matcher;
        }

        public static IAllOfMatcher<TEntity> AllOf(params IMatcher<TEntity>[] matchers) {
            var allOfMatcher = (Matcher<TEntity>)Matcher<TEntity>.AllOf(mergeIndices(matchers));
            setComponentNames(allOfMatcher, matchers);
            return allOfMatcher;
        }

        public static IAnyOfMatcher<TEntity> AnyOf(params int[] indices) {
            var matcher = new Matcher<TEntity>();
            matcher._anyOfIndices = distinctIndices(indices);
            return matcher;
        }

        public static IAnyOfMatcher<TEntity> AnyOf(params IMatcher<TEntity>[] matchers) {
            var anyOfMatcher = (Matcher<TEntity>)Matcher<TEntity>.AnyOf(mergeIndices(matchers));
            setComponentNames(anyOfMatcher, matchers);
            return anyOfMatcher;
        }

        static int[] mergeIndices(int[] allOfIndices, int[] anyOfIndices, int[] noneOfIndices) {
            var indicesList = EntitasCache.GetIntList();

            if (allOfIndices != null) {
                indicesList.AddRange(allOfIndices);
            }
            if (anyOfIndices != null) {
                indicesList.AddRange(anyOfIndices);
            }
            if (noneOfIndices != null) {
                indicesList.AddRange(noneOfIndices);
            }

            var mergedIndices = distinctIndices(indicesList);

            EntitasCache.PushIntList(indicesList);

            return mergedIndices;
        }

        static int[] mergeIndices(IMatcher<TEntity>[] matchers) {
            var indices = new int[matchers.Length];
            for (int i = 0; i < matchers.Length; i++) {
                var matcher = matchers[i];
                if (matcher.indices.Length != 1) {
                    throw new MatcherException(matcher.indices.Length);
                }
                indices[i] = matcher.indices[0];
            }

            return indices;
        }

        static string[] getComponentNames(IMatcher<TEntity>[] matchers) {
            for (int i = 0; i < matchers.Length; i++) {
                var matcher = matchers[i] as Matcher<TEntity>;
                if (matcher != null && matcher.componentNames != null) {
                    return matcher.componentNames;
                }
            }

            return null;
        }

        static void setComponentNames(Matcher<TEntity> matcher, IMatcher<TEntity>[] matchers) {
            var componentNames = getComponentNames(matchers);
            if (componentNames != null) {
                matcher.componentNames = componentNames;
            }
        }

        static int[] distinctIndices(IList<int> indices) {
            var indicesSet = EntitasCache.GetIntHashSet();

            foreach (var index in indices) {
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
