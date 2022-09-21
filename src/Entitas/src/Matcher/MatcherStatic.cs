using System;
using System.Collections.Generic;

namespace Entitas
{
    public partial class Matcher<TEntity>
    {
        [ThreadStatic] static List<int> _indexBufferThreadStatic;
        static List<int> _indexBuffer => _indexBufferThreadStatic ?? (_indexBufferThreadStatic = new List<int>());

        [ThreadStatic] static HashSet<int> _indexSetBufferThreadStatic;
        static HashSet<int> _indexSetBuffer => _indexSetBufferThreadStatic ?? (_indexSetBufferThreadStatic = new HashSet<int>());

        public static IAllOfMatcher<TEntity> AllOf(params int[] indices)
        {
            var matcher = new Matcher<TEntity>();
            matcher._allOfIndices = distinctIndices(indices);
            return matcher;
        }

        public static IAllOfMatcher<TEntity> AllOf(params IMatcher<TEntity>[] matchers)
        {
            var allOfMatcher = (Matcher<TEntity>)AllOf(mergeIndices(matchers));
            setComponentNames(allOfMatcher, matchers);
            return allOfMatcher;
        }

        public static IAnyOfMatcher<TEntity> AnyOf(params int[] indices)
        {
            var matcher = new Matcher<TEntity>();
            matcher._anyOfIndices = distinctIndices(indices);
            return matcher;
        }

        public static IAnyOfMatcher<TEntity> AnyOf(params IMatcher<TEntity>[] matchers)
        {
            var anyOfMatcher = (Matcher<TEntity>)AnyOf(mergeIndices(matchers));
            setComponentNames(anyOfMatcher, matchers);
            return anyOfMatcher;
        }

        static int[] mergeIndices(int[] allOfIndices, int[] anyOfIndices, int[] noneOfIndices)
        {
            if (allOfIndices != null) _indexBuffer.AddRange(allOfIndices);
            if (anyOfIndices != null) _indexBuffer.AddRange(anyOfIndices);
            if (noneOfIndices != null) _indexBuffer.AddRange(noneOfIndices);

            var mergedIndices = distinctIndices(_indexBuffer);
            _indexBuffer.Clear();
            return mergedIndices;
        }

        static int[] mergeIndices(IMatcher<TEntity>[] matchers)
        {
            var indices = new int[matchers.Length];
            for (var i = 0; i < matchers.Length; i++)
            {
                var matcher = matchers[i];
                if (matcher.indices.Length != 1)
                    throw new MatcherException(matcher.indices.Length);

                indices[i] = matcher.indices[0];
            }

            return indices;
        }

        static string[] getComponentNames(IMatcher<TEntity>[] matchers)
        {
            for (var i = 0; i < matchers.Length; i++)
            {
                var matcher = matchers[i] as Matcher<TEntity>;
                if (matcher?.componentNames != null)
                    return matcher.componentNames;
            }

            return null;
        }

        static void setComponentNames(Matcher<TEntity> matcher, IMatcher<TEntity>[] matchers)
        {
            var componentNames = getComponentNames(matchers);
            if (componentNames != null)
                matcher.componentNames = componentNames;
        }

        static int[] distinctIndices(IList<int> indices)
        {
            foreach (var index in indices)
                _indexSetBuffer.Add(index);

            var uniqueIndices = new int[_indexSetBuffer.Count];
            _indexSetBuffer.CopyTo(uniqueIndices);
            Array.Sort(uniqueIndices);

            _indexSetBuffer.Clear();

            return uniqueIndices;
        }
    }
}
