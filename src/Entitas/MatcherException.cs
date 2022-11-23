using System;

namespace Entitas
{
    public class MatcherException : Exception
    {
        public MatcherException(int indexes) : base(
            $"matcher.indexes.Length must be 1 but was {indexes}") { }
    }
}
