using System;
using System.Collections.Generic;

namespace Entitas
{
    public partial class Matcher<TEntity>
    {
        [ThreadStatic] static List<int> _indexBufferThreadStatic;
        static List<int> _indexBuffer => _indexBufferThreadStatic ??= new List<int>();

        [ThreadStatic] static HashSet<int> _indexSetBufferThreadStatic;
        static HashSet<int> _indexSetBuffer => _indexSetBufferThreadStatic ??= new HashSet<int>();

        public static IAllOfMatcher<TEntity> AllOf(params int[] indexes)
        {
            var matcher = new Matcher<TEntity>();
            matcher._allOfIndexes = DistinctIndexes(indexes);
            return matcher;
        }

        public static IAllOfMatcher<TEntity> AllOf(params IMatcher<TEntity>[] matchers)
        {
            var allOfMatcher = (Matcher<TEntity>)AllOf(MergeIndexes(matchers));
            SetComponentNames(allOfMatcher, matchers);
            return allOfMatcher;
        }

        public static IAnyOfMatcher<TEntity> AnyOf(params int[] indexes)
        {
            var matcher = new Matcher<TEntity>();
            matcher._anyOfIndexes = DistinctIndexes(indexes);
            return matcher;
        }

        public static IAnyOfMatcher<TEntity> AnyOf(params IMatcher<TEntity>[] matchers)
        {
            var anyOfMatcher = (Matcher<TEntity>)AnyOf(MergeIndexes(matchers));
            SetComponentNames(anyOfMatcher, matchers);
            return anyOfMatcher;
        }

        static int[] MergeIndexes(int[] allOfIndexes, int[] anyOfIndexes, int[] noneOfIndexes)
        {
            if (allOfIndexes != null) _indexBuffer.AddRange(allOfIndexes);
            if (anyOfIndexes != null) _indexBuffer.AddRange(anyOfIndexes);
            if (noneOfIndexes != null) _indexBuffer.AddRange(noneOfIndexes);

            var mergedIndexes = DistinctIndexes(_indexBuffer);
            _indexBuffer.Clear();
            return mergedIndexes;
        }

        static int[] MergeIndexes(IMatcher<TEntity>[] matchers)
        {
            var indexes = new int[matchers.Length];
            for (var i = 0; i < matchers.Length; i++)
            {
                var matcher = matchers[i];
                if (matcher.Indexes.Length != 1)
                    throw new MatcherException(matcher.Indexes.Length);

                indexes[i] = matcher.Indexes[0];
            }

            return indexes;
        }

        static string[] GetComponentNames(IMatcher<TEntity>[] matchers)
        {
            for (var i = 0; i < matchers.Length; i++)
            {
                if (matchers[i] is Matcher<TEntity> { ComponentNames: not null } m)
                    return m.ComponentNames;
            }

            return null;
        }

        static void SetComponentNames(Matcher<TEntity> matcher, IMatcher<TEntity>[] matchers)
        {
            var componentNames = GetComponentNames(matchers);
            if (componentNames != null)
                matcher.ComponentNames = componentNames;
        }

        static int[] DistinctIndexes(IList<int> indexes)
        {
            foreach (var index in indexes)
                _indexSetBuffer.Add(index);

            var uniqueIndexes = new int[_indexSetBuffer.Count];
            _indexSetBuffer.CopyTo(uniqueIndexes);
            Array.Sort(uniqueIndexes);

            _indexSetBuffer.Clear();

            return uniqueIndexes;
        }
    }
}
